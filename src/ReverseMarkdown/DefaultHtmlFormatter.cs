using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="IHtmlFormatter"/>
    /// interface.
    /// </summary>
    public class DefaultHtmlFormatter : IHtmlFormatter
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
        public void NormalizeWhitespace(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.Name == "#text")
            {
                NormalizeWhitespaceInTextNode(node);
            }
            else
            {
                // Theoretically, we should be able to normalize whitespace in
                // every text node within the <body> element (or even the root
                // <html> element). However, doing so causes a number of
                // existing unit tests to fail -- in particular the ones that
                // validate block elements embedded in tables are treated as
                // expected when converting to pipe tables in Markdown.
                //
                //NormalizeWhitespaceInAllDescendantTextNodes(node);
                //
                // To avoid these issues (at least for the time being), only
                // normalize whitespace in "specific" text nodes -- for example,
                // starting out with simple, inline elements like <b>.

                NormalizeWhitespaceInSpecificDescendantTextNodes(node);
            }
        }

        private static string NormalizeWhitespace(string html)
        {
            if (string.IsNullOrEmpty(html) == true)
            {
                return html;
            }

            var normalizedText = html
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Replace('\t', ' ');

            const string twoSpaces = "  ";
            const string oneSpace = " ";

            while (normalizedText.IndexOf(twoSpaces) != -1)
            {
                normalizedText = normalizedText.Replace(twoSpaces, oneSpace);
            }

            Debug.Assert(normalizedText.IndexOf(twoSpaces) == -1);
            return normalizedText;
        }

        private static void NormalizeWhitespaceInAllDescendantTextNodes(HtmlNode node)
        {
            node.Descendants("#text").ToList().ForEach(textNode =>
            {
                NormalizeWhitespaceInTextNode(textNode);
            });
        }

        private static void NormalizeWhitespaceInTextNode(HtmlNode textNode)
        {
            if (textNode.Name != "#text")
            {
                throw new ArgumentException(
                    $"Invalid node encountered ({textNode.Name})"
                        + " -- expected (#text).");
            }

            // Never change the whitespace in preformatted (<pre>) content
            if (textNode.Ancestors("pre").Any() == false)
            {
                string normalizedText = NormalizeWhitespace(textNode.InnerHtml);

                textNode.InnerHtml = normalizedText;
            }
        }

        private static void NormalizeWhitespaceInSpecificDescendantTextNodes(
            HtmlNode node)
        {
            var doc = node.OwnerDocument;

            // Normalize whitespace in "inline" elements
            //
            // Start with just one inline element type to prove the approach
            //
            // TODO: Add other "inline" elements and "block" elements (with
            // corresponding tests)

            // Note that the normalize-space XPath function will trim leading
            // and trailing whitespace. Consequently we will almost certainly
            // process more text nodes than are technically necessary, but this
            // approach is probably better than just "brute force" iterating
            // over all elements (since this will exclude simple content like
            // "<b>some bold text</b>").

            NormalizeWhitespaceInTextNodes(
                doc,
                "//b[text() != normalize-space()]");
        }

        private static void NormalizeWhitespaceInTextNodes(
            HtmlDocument doc,
            string xpath)
        {
            var nodes = doc.DocumentNode.SelectNodes(xpath);

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    Debug.Assert(node.Name == "b");

                    NormalizeWhitespaceInAllDescendantTextNodes(node);
                }
            }
        }
    }
}
