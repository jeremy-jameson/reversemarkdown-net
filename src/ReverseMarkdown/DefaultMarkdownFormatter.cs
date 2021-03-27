using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private readonly Config _config;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DefaultMarkdownFormatter"/> class with the specified
        /// "reference" <see cref="HtmlNode"/>.
        /// </summary>
        /// <param name="referenceNode">An <see cref="HtmlNode"/> that
        /// represents the HTML element used for "reference" purposes when
        /// formatting Markdown text.</param>
        public DefaultMarkdownFormatter(HtmlNode referenceNode, Config config)
        {
            if (referenceNode == null)
            {
                throw new ArgumentNullException("referenceNode");
            }

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _referenceNode = referenceNode;
            _config = config;
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
        /// Gets the rules for formatting Markdown -- such as the line length
        /// for wrapping text -- by inferring information about the structure of
        /// the source HTML content used in the Markdown conversion.
        /// </summary>
        /// <returns>A <see cref="MarkdownFormattingRules"/> object that
        /// contains the rules for formatting the converted Markdown.</returns>
        public MarkdownFormattingRules GetFormattingRules()
        {
            var node = _referenceNode;

            var formattingRules = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            if (node.Name == "pre"
                || node.Ancestors("pre").Any() == true
                || node.Descendants("pre").Any() == true)
            {
                // Never trim or wrap text in preformatted content
                Debug.Assert(formattingRules.CanTrim == false);
                Debug.Assert(formattingRules.WrapLineLength == int.MaxValue);
            }
            else if (node.Descendants("table").Any() == true)
            {
                // Never trim or wrap text in block element that contains a
                // table (e.g. "<div>...<table>...</table></div>")
                Debug.Assert(formattingRules.CanTrim == false);
                Debug.Assert(formattingRules.WrapLineLength == int.MaxValue);
            }
            else if (node.InnerText.Contains("{{<") == true
                && node.Descendants("blockquote").Any() == true)
            {
                // Avoid "double-wrapping" in <blockquote>, for example:
                //
                //   <div><blockquote>{{< ... >}}</blockquote><div>
                //
                // When processing the <blockquote> element, line wrapping
                // within the Hugo shortcode is prevented because the entire
                // shortcode is parsed as a single "chunk." Consequently, when
                // processing the <div> element, line wrapping must also be
                // prevented.

                Debug.Assert(formattingRules.CanTrim == false);
                Debug.Assert(formattingRules.WrapLineLength == int.MaxValue);
            }
            else if (node.InnerText.Contains("{{<") == true
                && (node.Name == "div" || node.Name == "p")
                && (node.Ancestors("blockquote").Any() == true
                    || node.Ancestors("div").Any() == true))
            {
                // Process only the "outermost" block element to avoid issues
                // where wrapping text multiple times would cause undesired
                // results. For example, wrapping lengthy Hugo shortcodes twice
                // will often result in "corruption" due to quoted parameter
                // values being split across multiple lines (during the second
                // call to ITextFormatter.WrapText).

                Debug.Assert(formattingRules.CanTrim == false);
                Debug.Assert(formattingRules.WrapLineLength == int.MaxValue);
            }
            else if (node.Name == "blockquote"
                || node.Name == "div"
                || node.Name == "li"
                || node.Name == "p")
            {
                formattingRules.WrapLineLength = GetWrapLineLength(node);

                if (node.Name != "div")
                {
                    formattingRules.CanTrim = true;
                }
            }

            return formattingRules;
        }

        /// <summary>
        /// Gets the Markdown "prefix" for the specified list item (such as "- ",
        /// "1. ", "2. ", etc.).
        /// </summary>
        /// <remarks>
        /// For list items in an unordered list, the "prefix" is determined by
        /// <see cref="Config.ListBulletChar"/>. For list items in an ordered
        /// list, the "prefix" is determined by the position of the list item in
        /// the list (e.g. the first item will have a prefix of "1. " and the
        /// second item will have a prefix of "2. ").
        /// </remarks>
        /// <param name="node">An <see cref="HtmlNode"/> that represents a list
        /// item element (<c>&lt;li&gt;</c>) in an HTML document.</param>
        /// <returns>The Markdown "prefix" for the specified list item.</returns>
        public string GetMarkdownPrefixForListItem(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            else if (node.Name != "li")
            {
                throw new ArgumentException(
                    $"The specified node ({node.Name}) is not a list item.",
                    "node");
            }

            if (node.ParentNode != null && node.ParentNode.Name == "ol")
            {
                // index are zero based hence add one
                var index = node.ParentNode.SelectNodes("./li").IndexOf(node) + 1;
                return $"{index}. ";
            }
            else
            {
                return $"{_config.ListBulletChar} ";
            }
        }

        private int GetWrapLineLength(HtmlNode node)
        {
            if (node.Ancestors("table").Any() == true)
            {
                return int.MaxValue;
            }

            var blockquoteIndentationLevel =
                node.AncestorsAndSelf("blockquote")
                    .Where(x => x.Name == "blockquote")
                    .Count();

            var blockquoteIndentation =
                blockquoteIndentationLevel * ("> ".Length);

            var listIndentation = new StringBuilder();

            node.AncestorsAndSelf("li")
                .Where(x => x.Name == "li")
                .ToList()
                .ForEach(listItemNode =>
                {
                    var listItemPrefix = GetMarkdownPrefixForListItem(listItemNode);

                    listIndentation.Append(new string(' ', listItemPrefix.Length));
                });

            return (80 - blockquoteIndentation - listIndentation.Length);
        }

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
            // - whenever a Markdown image (e.g.
            //     "![Example image](http://example.com/img.png)") or link
            //     (e.g. "[Example link](http://example.com)") is encountered
            // - or
            // - whenever inline code is encountered (e.g. "`var i = 1;`")
            // - or
            // - at each space character
            //
            // This creates "chunks" from:
            //
            // - entire Markdown images/links (identified by the combination of
            //     square brackets and parentheses and optional preceeding
            //     exclamation mark -- in the case of an image)
            // - or
            // - entire inline code segments (identified by surrounding
            //     backquotes)
            // - or
            // - typical words (separated by spaces)

            const string patternForItemsSeparatedBySpaces = "[^ ]+";

            const string patternForAnyNonWhitespaceCharacters = @"[\S]*";

            const string patternForMarkdownImagesAndLinks =
                patternForAnyNonWhitespaceCharacters
                + @"!?\[(?:.*?)\]\((?:.*?)\)"
                + patternForAnyNonWhitespaceCharacters;

            const string patternForInlineCode =
                @"[\S]*?`.*?`"
                + patternForAnyNonWhitespaceCharacters;

            const string pattern =
                patternForMarkdownImagesAndLinks
                + "|"
                + patternForInlineCode
                + "|"
                + patternForItemsSeparatedBySpaces;

            var chunks = Regex.Matches(text, pattern, RegexOptions.Compiled)
                .Cast<Match>()
                .Select(x => x.Value)
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

            while (markdown.StartsWith(
                Environment.NewLine + Environment.NewLine) == true)
            {
                markdown = markdown.Substring(Environment.NewLine.Length);
            }

            while (markdown.EndsWith(
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
