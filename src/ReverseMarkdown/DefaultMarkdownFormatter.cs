using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="IMarkdownFormatter"/>
    /// interface.
    /// </summary>
    public class DefaultMarkdownFormatter : DefaultTextFormatter, IMarkdownFormatter,
        ITextFormatter
    {
        private readonly HtmlNode _referenceNode;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DefaultMarkdownFormatter"/> class with the specified
        /// "reference" <see cref="HtmlNode"/>.
        /// </summary>
        /// <param name="referenceNode">An <see cref="HtmlNode"/> that
        /// represents the HTML element used for "reference" purposes when
        /// formatting Markdown text.</param>
        public DefaultMarkdownFormatter(HtmlNode referenceNode)
        {
            if (referenceNode == null)
            {
                throw new ArgumentNullException("referenceNode");
            }

            _referenceNode = referenceNode;
        }

        /// <summary>
        /// Gets an <see cref="HtmlNode"/> that represents the HTML element used
        /// for "reference" purposes when formatting Markdown text.
        /// </summary>
        /// <remarks>
        /// Referencing the source HTML element allows Markdown formatting rules
        /// to vary based on the structure of the HTML content converted to
        /// Markdown.
        /// </remarks>
        public HtmlNode ReferenceNode => _referenceNode;

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

            // The following regular expression splits the string at each space
            // character or whenever a Markdown image (e.g.
            // "![Example image](http://example.com/img.png)") or link (e.g.
            // "[Example link](http://example.com)") is encountered.
            //
            // This creates "chunks" from typical words (separated by spaces)
            // as well as entire Markdown images/links (identified by the
            // combination of square brackets and parentheses and optional
            // preceeding exclamation mark -- in the case of an image).
            //
            // There might be a better way, but this works for the purpose
            // of keeping Markdown images/links (with spaces in the image/link
            // text) from wrapping across multiple lines.
            //
            // The "Where" filter is required to ignore empty strings included
            // as a result of the Markdown image/link portion of the regular
            // expression pattern.

            var chunks =
                Regex.Split(
                    text,
                    @" |(!?\[(?:.*?)\]\((?:.*?)\)[\S]*)",
                    RegexOptions.Compiled)
                .Where(x => x != string.Empty)
                .ToArray();

            return chunks;
        }

        /// <summary>
        /// Removes multiple consecutive blank lines from the Markdown.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This ensures the generated Markdown does not violate the following
        /// <a href="https://github.com/DavidAnson/markdownlint">markdownlint</a>
        /// rule:
        /// 
        /// <a href="https://github.com/DavidAnson/markdownlint/blob/v0.23.1/doc/Rules.md#md012">
        ///     MD012 - Multiple consecutive blank lines</a>
        /// </para>
        /// </remarks>
        /// <param name="markdown">The Markdown content to format.</param>
        public string RemoveMultipleConsecutiveBlankLines(string markdown)
        {
            if (markdown == null)
            {
                throw new ArgumentNullException("markdown");
            }

            if (markdown.StartsWith(
                Environment.NewLine + Environment.NewLine) == true)
            {
                markdown = markdown.Substring(Environment.NewLine.Length);
            }

            if (markdown.EndsWith(
                Environment.NewLine + Environment.NewLine) == true)
            {
                markdown = markdown.Remove(markdown.Length - Environment.NewLine.Length);
            }

            const string pattern = @"(\r?\n){3,}";
            string replacement = Environment.NewLine + Environment.NewLine;

            return Regex.Replace(markdown, pattern, replacement);
        }
    }
}
