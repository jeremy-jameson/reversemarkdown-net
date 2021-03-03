using HtmlAgilityPack;

namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for formatting HTML -- such as normalizing whitespace.
    /// </summary>
    public interface IHtmlFormatter
    {
        /// <summary>
        /// Returns <c>true</c> if the node is not <c>null</c> and the name of
        /// the node is a recognized block element (e.g. "div").
        /// </summary>
        /// <param name="node">An <see cref="HtmlNode"/> that represents an
        /// element in an HTML document.</param>
        /// <returns>
        /// <c>true</c> if the node is not <c>null</c> and is a recognized block
        /// element; otherwise <c>false</c>.
        /// </returns>
        bool IsBlockElement(HtmlNode node);

        /// <summary>
        /// Returns <c>true</c> if the name is a recognized block element (e.g.
        /// "div").
        /// </summary>
        /// <param name="name">The HTML element name.</param>
        /// <returns>
        /// <c>true</c> if the name is a recognized block element; otherwise
        /// <c>false</c>.
        /// </returns>
        bool IsBlockElement(string name);

        /// <summary>
        /// Returns <c>true</c> if the node is not <c>null</c> and the name of
        /// the node is a recognized inline element (e.g. "span").
        /// </summary>
        /// <param name="node">An <see cref="HtmlNode"/> that represents an
        /// element in an HTML document.</param>
        /// <returns>
        /// <c>true</c> if the node is not <c>null</c> and is a recognized inline
        /// element; otherwise <c>false</c>.
        /// </returns>
        bool IsInlineElement(HtmlNode node);

        /// <summary>
        /// Returns <c>true</c> if the name is a recognized inline tag (e.g.
        /// "span").
        /// </summary>
        /// <param name="name">The HTML element name.</param>
        /// <returns>
        /// <c>true</c> if the name is a recognized inline element; otherwise
        /// <c>false</c>.
        /// </returns>
        bool IsInlineElement(string name);

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

        /// <summary>
        /// Normalizes whitespace in all "inner text" for the specified HTML
        /// node -- except any preformatted (&lt;pre&gt;) text -- and removes
        /// any insignificant whitespace (for example, leading and trailing
        /// whitespace within a block element).
        /// </summary>
        /// <remarks>
        /// Insignificant whitespace has no effect on the rendering of the HTML
        /// and therefore can be removed. A simple example is whitespace at the
        /// beginning and end of an &lt;h1&gt; element.
        /// </remarks>
        /// <param name="node">An <see cref="HtmlNode"/> that represents the
        /// portion of an HTML document to normalize and remove whitespace in
        /// -- ranging anywhere from a specific text node (#text), a short
        /// inline element (e.g. &lt;strong&gt;some strong text&lt;/strong&gt;),
        /// a block element (e.g. an entire paragraph of formatted text), or a
        /// complete HTML document (i.e. &lt;html&gt;...&lt;/html&gt;).</param>
        void RemoveInsignificantWhitespace(HtmlNode root);
    }
}
