using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for formatting HTML -- such as normalizing whitespace.
    /// </summary>
    public interface IHtmlFormatter
    {
        /// <summary>
        /// Replaces sequences of whitespace characters with a single space in
        /// all "inner text" for the specified HTML node -- except any
        /// preformatted (&lt;pre&gt;) text.
        /// </summary>
        /// <remarks>
        /// Unlike the <code>normalize-space</code> XPath function,
        /// <code>NormalizeWhitespace</code> does not strip leading and trailing
        /// whitespace.
        /// </remarks>
        /// <param name="node">An <see cref="HtmlNode"/> that represents the
        /// portion of an HTML document to normalize whitespace in -- ranging
        /// anywhere from a specific text node (#text), a short inline element
        /// (e.g. &lt;strong&gt;some strong text&lt;/strong&gt;), a block
        /// element (e.g. an entire paragraph of formatted text), or a
        /// complete HTML document (i.e. &lt;html&gt;...&lt;/html&gt;).</param>
        void NormalizeWhitespace(HtmlNode node);
    }
}
