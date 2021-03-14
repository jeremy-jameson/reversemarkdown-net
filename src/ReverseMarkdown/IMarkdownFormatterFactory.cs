using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for creating new instances of
    /// <see cref="IMarkdownFormatter"/> objects.
    /// </summary>
    public interface IMarkdownFormatterFactory
    {
        /// <summary>
        /// Creates a new instance of an <see cref="IMarkdownFormatter"/>
        /// object with the specified "reference" <see cref="HtmlNode"/>.
        /// </summary>
        /// <remarks>
        /// When inheriting from <see cref="IMarkdownFormatterFactory"/>,
        /// a factory class implements a strongly-typed version of
        /// <see cref="Create(HtmlNode)"/>.
        /// </remarks>
        /// <param name="referenceNode">An <see cref="HtmlNode"/> that
        /// represents the HTML element used for "reference" purposes when
        /// formatting Markdown text.</param>
        /// <returns>An <see cref="IMarkdownFormatter"/> object.</returns>
        IMarkdownFormatter Create(HtmlNode referenceNode);
    }
}
