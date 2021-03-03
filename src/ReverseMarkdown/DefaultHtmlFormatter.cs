using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        private static readonly HashSet<string> __blockTags =
            new HashSet<string>( new string[] { "address", "article", "aside",
                "blockquote", "canvas", "dd", "div", "dl", "dt", "fieldset",
                "figcaption", "figure", "footer", "form", "h1", "h2", "h3",
                "h4", "h5", "h6", "header", "hr", "li", "main", "nav",
                "noscript", "ol", "p", "pre", "section", "table", "tfoot",
                "ul", "video" });

        private static readonly HashSet<string> __inlineTags =
            new HashSet<string>(new string[] { "a", "abbr", "acronym", "b",
                "bdo", "big", "br", "button", "cite", "code", "dfn", "em", "i",
                "img", "input", "kbd", "label", "map", "object", "output", "q",
                "samp", "script", "select", "small", "span", "strong", "sub",
                "sup", "textarea", "time", "tt", "var" });

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
        public bool IsBlockElement(HtmlNode node)
        {
            return __blockTags.Contains(node?.Name);
        }

        /// <summary>
        /// Returns <c>true</c> if the name is a recognized block element (e.g.
        /// "div").
        /// </summary>
        /// <param name="name">The HTML element name.</param>
        /// <returns>
        /// <c>true</c> if the name is a recognized block element; otherwise
        /// <c>false</c>.
        /// </returns>
        public bool IsBlockElement(string name)
        {
            return __blockTags.Contains(name);
        }

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
        public bool IsInlineElement(HtmlNode node)
        {
            return __inlineTags.Contains(node?.Name);
        }

        /// <summary>
        /// Returns <c>true</c> if the name is a recognized inline tag (e.g.
        /// "span").
        /// </summary>
        /// <param name="name">The HTML element name.</param>
        /// <returns>
        /// <c>true</c> if the name is a recognized inline element; otherwise
        /// <c>false</c>.
        /// </returns>
        public bool IsInlineElement(string name)
        {
            return __inlineTags.Contains(name);
        }

        private bool IsLeadingWhitespaceSignificant(HtmlNode node)
        {
            Debug.Assert(node.Name == "#text");

            // Whitespace is always significant in preformatted (<pre>) content
            if (node.Ancestors("pre").Any() == true)
            {
                return true;
            }

            if (IsBlockElement(node.PreviousSibling) == true)
            {
                return false;
            }
            
            if (IsBlockElement(node.ParentNode) == true
                && node.ParentNode?.FirstChild == node)
            {
                return false;
            }

            return true;
        }

        private bool IsTrailingWhitespaceSignificant(HtmlNode node)
        {
            Debug.Assert(node.Name == "#text");

            // Whitespace is always significant in preformatted (<pre>) content
            if (node.Ancestors("pre").Any() == true)
            {
                return true;
            }

            if (IsBlockElement(node.NextSibling) == true)
            {
                return false;
            }

            if (IsBlockElement(node.ParentNode) == true
                && node.ParentNode?.LastChild == node)
            {
                return false;
            }

            return true;
        }

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
        public void RemoveInsignificantWhitespace(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            // Never change the whitespace in preformatted (<pre>) content
            if (node.Ancestors("pre").Any() == true)
            {
                return;
            }

            node.ChildNodes.ToList().ForEach(child =>
                RemoveInsignificantWhitespace(child));

            if (node.Name == "#text")
            {
                NormalizeWhitespace(node);

                var text = node.InnerText;

                bool hasLeadingWhitespace =
                    (text != node.InnerText.TrimStart());

                bool hasTrailingWhitespace =
                    (text != node.InnerText.TrimEnd());

                if (hasLeadingWhitespace == true
                    && IsLeadingWhitespaceSignificant(node) == false)
                {
                    node.InnerHtml = node.InnerHtml.TrimStart();
                }

                if (hasTrailingWhitespace == true
                    && IsTrailingWhitespaceSignificant(node) == false)
                {
                    node.InnerHtml = node.InnerHtml.TrimEnd();
                }
            }
        }
    }
}
