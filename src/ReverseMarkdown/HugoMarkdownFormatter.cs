using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    /// <summary>
    /// Custom <see cref="IMarkdownFormatter"/> with special handling for Hugo
    /// shortcodes in Markdown.
    /// </summary>
    public class HugoMarkdownFormatter : DefaultMarkdownFormatter,
        IMarkdownFormatter, ITextFormatter
    {
        /// <summary>
        /// Splits the specified line of text into individual "chunks" --
        /// typically single words -- which can then be used to wrap the text
        /// onto multiple lines, as necessary, using
        /// <see cref="WrapTextLine(string, int)"/>.
        /// </summary>
        /// <remarks>
        /// An <see cref="ArgumentException"/> is thrown if
        /// <paramref name="text"/> contains a line feed character (<c>\n</c>).
        /// <see cref="WrapText(string, int)"/> should be used instead of
        /// <see cref="WrapTextLine(string, int)"/> when the text may already
        /// contain multiple lines.
        /// </remarks>
        /// <param name="text">The single line of text to parse.</param>
        public override IEnumerable<string> ParseChunks(string text)
        {
            if (text == null)
            {
                return Enumerable.Empty<string>();
            }
            else if (text.IndexOf('\n') != -1)
            {
                throw new ArgumentException(
                    "Cannot parse chunks from text because the text contains a"
                        + " line feed.",
                    "text");
            }
            else if (text == string.Empty)
            {
                return new string[] { string.Empty };
            }

            // The following regular expression splits the string:
            //
            // - at each space character
            // - or
            // - whenever a Markdown image (e.g.
            //     "![Example image](http://example.com/img.png)") or link
            //     (e.g. "[Example link](http://example.com)") is encountered
            // - or
            // - whenever a Hugo shortcode (e.g. "{{< gist spf13 7896402 >}}")
            //     is encountered
            //
            // This creates "chunks" from:
            //
            // - typical words (separated by spaces)
            // - or
            // - entire Markdown images/links (identified by the combination of
            //     square brackets and parentheses and optional preceeding
            //     exclamation mark -- in the case of an image)
            // - or
            // - entire Hugo shortcodes (identified by the opening "{{<" and
            //     closing ">}}")
            //
            // There might be a better way, but this works for the purpose
            // of keeping Markdown images/links (with spaces in the image/link
            // text) and Hugo shortcodes from wrapping across multiple lines.
            //
            // The "Where" filter is required to ignore empty strings included
            // as a result of the Markdown image/link and Hugo shortcode
            // portions of the regular expression pattern

            const string patternForHugoShortcodeStart = @"[\S]*\{\{< ";
            const string patternForHugoShortcodeEnd = @" >\}\}[\S]*";

            const string patternForHugoShortcodes =
                "(?:("
                    + patternForHugoShortcodeStart
                    + ".*?"
                    + patternForHugoShortcodeEnd
                + "))";

            var pattern =
                " " // space --> regular word "chunk"
                + "|" // or
                // "![...](...)" --> Markdown image "chunk"
                // "[...](...)" --> Markdown link "chunk"
                + @"(!?\[(?:.*?)\]\((?:.*?)\)[\S]*)"
                + "|" // or
                // "{{< ... >}}" --> Hugo shortcode "chunk"
                + patternForHugoShortcodes
                ;

            var chunks =
                Regex.Split(
                    text,
                    pattern,
                    RegexOptions.Compiled)
                .Where(x => x != string.Empty)
                .ToList();

            // At this point, each Hugo shortcode is a separate chunk
            // (e.g. "{{< gist spf13 7896402 >}}") but in many cases
            // the shortcodes can be parsed into fine-grained chunks
            // (e.g. "{{<", "gist", "spf13", and "7896402 >}}").
            ParseHugoShortcodes(chunks);

            return chunks.ToArray();
        }

        private List<string> ParseHugoShortcode(string shortcode)
        {
            Debug.Assert(shortcode.StartsWith("{{<"));
            Debug.Assert(shortcode.EndsWith(">}}"));

            // Reference:
            // https://www.metaltoad.com/blog/regex-quoted-string-escapable-quotes

            const string patternForQuotedStringWithEscapedQuotes =
                "((?<![\\\\])['\"])((?:.(?!(?<![\\\\])\\1))*.?)\\1";

            const string patternForStringWithoutQuotes =
                "[^\\s\"']+";

            var pattern =
                patternForStringWithoutQuotes
                + "|"
                + patternForQuotedStringWithEscapedQuotes;
            
            var chunks =
                Regex.Matches(
                    shortcode,
                    pattern,
                    RegexOptions.Compiled)
                .Cast<Match>()
                .Select(x => x.Value)
                .ToList();

            // To avoid even more complex regular expression patterns, the
            // matches may consist of separate "chunks" for parameter names and
            // values.
            //
            // For example:
            //
            //   {{< figure src='http://example.com/img.png' >}}
            //
            // is parsed as the following five chunks: "{{<", "figure", "src=",
            // "'http://example.com/img.png'", and ">}}".
            //
            // Note the parameter name ("src") is separate from the parameter
            // value ("'http://example.com/img.png'").
            //
            // Combine "chunks" for named parameters (e.g.
            // append "'http://example.com/img.png'" to "src=" in order to form
            // a single "src='http://example.com/img.png'" chunk).
            //
            // While Hugo may be capable of handling shortcodes where the
            // parameter name and value are on separate lines, e.g.
            //
            //   {{< figure src=
            //   'http://example.com/img.png' >}}
            //
            // ...the following form is preferred for better readability:
            //
            //   {{< figure
            //   src='http://example.com/img.png' >}}

            for (var i = 0; i < chunks.Count(); i++)
            {
                var chunk = chunks[i];

                if (chunk.EndsWith("=") == true
                    && i < (chunks.Count() - 2))
                {
                    chunks[i] += chunks[i + 1];

                    chunks.RemoveAt(i + 1);
                }
            }

            // When splitting Hugo shortcodes over multiple lines, care must be
            // taken to avoid having the closing ">}}" on a separate line --
            // since this would be interepreted in Markdown as a blockquote.
            //
            // To avoid this, combine the shortcode end characters (i.e. ">}}")
            // with the last parameter -- or shortcode name, if no parameters
            // are specified.
            //
            // Continuing the previous example, this results in the following
            // three chunks: "{{<", "figure", and
            // "src='http://example.com/img.png' >}}"

            var chunkCount = chunks.Count();

            if (chunkCount < 3)
            {
                throw new ArgumentException(
                    $"The Hugo shortcode ({shortcode}) is invalid.",
                    "shortcode");
            }

            Debug.Assert(chunks[chunkCount - 1] == ">}}");

            chunks[chunkCount - 2] =
                string.Concat(
                    chunks[chunkCount - 2], // last parameter (or shortcode name)
                    " ",
                    chunks[chunkCount - 1]); // i.e. ">}}"

            chunks.RemoveAt(chunkCount - 1); // Remove ">}}"

            return chunks;
        }

        private void ParseHugoShortcodes(List<string> chunks)
        {
            // Parse any Hugo shortcode "chunks" (e.g.
            // "{{< gist spf13 7896402 >}}") into fine-grained chunks
            // (e.g. "{{<", "gist", "spf13", and "7896402 >}}").

            for (var i = 0; i < chunks.Count(); i++)
            {
                var chunk = chunks[i];

                if (chunk.Contains("{{< ") == true)
                {
                    // There may be additional characters surrounding a
                    // shortcode (typically punctuation marks like quotes,
                    // parentheses, commas, etc.). To simplify the
                    // processing of just the shortcode, strip off any
                    // "prefix" and "suffix" and then add them back after
                    // the shortcode has been parsed into fine-grained
                    // chunks.

                    var shortcodeStartIndex = chunk.IndexOf("{{<");
                    var shortcodePrefix = string.Empty;

                    if (shortcodeStartIndex > 0)
                    {
                        shortcodePrefix = chunk.Substring(
                            0,
                            shortcodeStartIndex);

                        chunk = chunk.Substring(shortcodeStartIndex);
                    }

                    var shortcodeEndIndex = chunk.IndexOf(">}}");

                    var shortcodeSuffix = chunk.Substring(
                        shortcodeEndIndex + ">}}".Length);

                    if (shortcodeSuffix.Length > 0)
                    {
                        chunk = chunk.Remove(
                            chunk.Length - shortcodeSuffix.Length);
                    }

                    var shortcodeChunks = ParseHugoShortcode(chunk);

                    // Add back any "prefix" to the first chunk
                    shortcodeChunks[0] = shortcodePrefix + shortcodeChunks[0];

                    // Add back any "suffix" to the last chunk
                    shortcodeChunks[shortcodeChunks.Count() - 1] +=
                        shortcodeSuffix;

                    // Replace the chunk consisting of the entire shortcode with
                    // the "fine-grained" chunks.
                    chunks.RemoveAt(i);
                    chunks.InsertRange(i, shortcodeChunks);
                }
            }
        }
    }
}

