namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for formatting Markdown -- such as removing multiple
    /// consecutive blank lines.
    /// </summary>
    public interface IMarkdownFormatter
    {
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
