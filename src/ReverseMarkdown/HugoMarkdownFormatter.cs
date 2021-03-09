using System;
using System.Collections.Generic;
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
            // - whenever a Hugo shortcode (e.g. "gist spf13 7896402") is
            //     encountered
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

            var pattern =
                " " // space --> regular word "chunk"
                + "|" // or
                // "![...](...)" --> Markdown image "chunk"
                // "[...](...)" --> Markdown link "chunk"
                + @"(!?\[(?:.*?)\]\((?:.*?)\)[\S]*)"
                + "|" // or
                // "{{< ... >}}" --> Hugo shortcode "chunk"
                + @"([\S]*(?=\{\{<).*?(?>\}\})[\S]*)"
                ;

            var chunks =
                Regex.Split(
                    text,
                    pattern,
                    RegexOptions.Compiled)
                .Where(x => x != string.Empty)
                .ToArray();

            return chunks;
        }
    }
}

