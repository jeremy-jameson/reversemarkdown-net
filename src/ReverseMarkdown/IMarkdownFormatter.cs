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
