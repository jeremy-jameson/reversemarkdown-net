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
                NormalizeWhitespaceInAllDescendantTextNodes(node);
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
            if (textNode.Ancestors("pre").Any() == true)
            {
                return;
            }

            // Theoretically, we should be able to normalize whitespace in every
            // text node within the <body> element (or even the root <html>
            // element) -- provided we skip preformatted (<pre>) content.
            //
            // However, doing so causes a number of existing unit tests to fail
            // -- in particular the ones that test "funky" content embedded in
            // tables. For example, converting a "plain" line break
            // (i.e. "\r\n") to an HTML <br> element.
            //
            // For the time being, do *not* normalize whitespace in tables.
            //
            // TODO: Normalize whitespace in tables and "fix" the tests
            // accordingly (e.g. figure out why converting "plain" line breaks
            // to HTML line breaks by default was considered necessary.
            //
            // For reference purposes, I believe the table in the following blog
            // post is corrupted by the "plain" line break to HTML line break
            // behavior in ReverseMarkdown:
            //
            // https://www.technologytoolbox.com/blog/jjameson/archive/2012/02/19/html-to-pdf-converters.aspx
            //
            // Related commit (essentially my hack around the ReverseMarkdown
            // issue):
            //
            // https://github.com/jeremy-jameson/BlogML2Hugo/commit/0576f9809f39a622079c3bf0d21c8c1fef2a4cae

            if (textNode.Ancestors("table").Any() == true)
            {
                return;
            }

            string normalizedText = NormalizeWhitespace(textNode.InnerHtml);

            textNode.InnerHtml = normalizedText;
        }
    }
}
