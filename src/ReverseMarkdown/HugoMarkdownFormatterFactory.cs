using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Implementation of <see cref="IMarkdownFormatterFactory"/> used to
    /// support formatting Markdown text containing Hugo shortcodes.
    /// </summary>
    public class HugoMarkdownFormatterFactory : IMarkdownFormatterFactory
    {
        /// <summary>
        /// Creates a new instance of a <see cref="HugoMarkdownFormatter"/>
        /// object with the specified "reference" <see cref="HtmlNode"/>.
        /// </summary>
        /// <param name="referenceNode">An <see cref="HtmlNode"/> that
        /// represents the HTML element used for "reference" purposes when
        /// formatting Markdown text.</param>
        /// <returns>An <see cref="IMarkdownFormatter"/> object that supports
        /// formatting Markdown text containing Hugo shortcodes.</returns>
        public IMarkdownFormatter Create(HtmlNode referenceNode, Config config)
        {
            return new HugoMarkdownFormatter(referenceNode, config);
        }
    }
}
