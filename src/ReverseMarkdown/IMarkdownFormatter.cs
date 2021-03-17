using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for formatting Markdown -- such as indenting text,
    /// wrapping a long line of Markdown onto multiple lines, and removing
    /// multiple consecutive blank lines.
    /// </summary>
    public interface IMarkdownFormatter : ITextFormatter
    {
        /// <summary>
        /// Gets an <see cref="HtmlNode"/> that represents the HTML element used
        /// for "reference" purposes when formatting Markdown text.
        /// </summary>
        /// <remarks>
        /// Referencing the source HTML element allows Markdown formatting rules
        /// to vary based on the structure of the HTML content converted to
        /// Markdown.
        /// </remarks>
        HtmlNode ReferenceNode { get;  }

        /// <summary>
        /// Gets the rules for formatting Markdown -- such as the line length
        /// for wrapping text -- by inferring information about the structure of
        /// the source HTML content used in the Markdown conversion.
        /// </summary>
        /// <returns>A <see cref="MarkdownFormattingRules"/> object that
        /// contains the rules for formatting the converted Markdown.</returns>
        MarkdownFormattingRules GetFormattingRules();

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
        string GetMarkdownPrefixForListItem(HtmlNode node);

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
        string RemoveMultipleConsecutiveBlankLines(string markdown);
    }
}
