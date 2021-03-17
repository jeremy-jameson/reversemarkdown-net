using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="IMarkdownFormatterFactory"/>
    /// interface.
    /// </summary>
    public class DefaultMarkdownFormatterFactory : IMarkdownFormatterFactory
    {
        /// <summary>
        /// Creates a new instance of a <see cref="DefaultMarkdownFormatter"/>
        /// object with the specified "reference" <see cref="HtmlNode"/>.
        /// </summary>
        /// <param name="referenceNode">An <see cref="HtmlNode"/> that
        /// represents the HTML element used for "reference" purposes when
        /// formatting Markdown text.</param>
        /// <returns>An <see cref="IMarkdownFormatter"/> object that supports
        /// "basic" formatting of Markdown text.</returns>
        public IMarkdownFormatter Create(HtmlNode referenceNode, Config config)
        {
            return new DefaultMarkdownFormatter(referenceNode, config);
        }
    }
}
