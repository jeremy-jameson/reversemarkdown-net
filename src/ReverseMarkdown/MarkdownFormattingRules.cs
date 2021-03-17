namespace ReverseMarkdown
{
    /// <summary>
    /// Specifies the rules for formatting Markdown -- such as the line length
    /// for wrapping text.
    /// </summary>
    public class MarkdownFormattingRules
    {
        /// <summary>
        /// Gets or sets a value indicating if whitespace can be trimmed from
        /// the start and end of the Markdown.
        /// </summary>
        public bool CanTrim { get; set; }

        /// <summary>
        /// Gets or sets the preferred maximum length of each line in the
        /// formatted Markdown.
        /// </summary>
        /// <remarks>
        /// Set this property to a very large number (e.g.
        /// <see cref="System.Int32.MaxValue"/>) to indicate the Markdown should
        /// not be wrapped onto multiple lines during the formatting process.
        /// </remarks>
        public int WrapLineLength { get; set; }
    }
}
