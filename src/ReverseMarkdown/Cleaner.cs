using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    public static class Cleaner
    {
        private static string CleanTagBorders(string content)
        {
            // content from some htl editors such as CKEditor emits newline and tab between tags, clean that up

            // Original approach to cleaning up the newline/tab issue.
            // This could be problematic because it could potentially remove
            // "significant whitespace" (e.g. "<b>some\n\tbold text</b>"
            // or "<pre>function foo {\n\t...</pre>"
            //
            //content = content.Replace("\n\t", "");
            //content = content.Replace(Environment.NewLine + "\t", "");

            // A slightly safer approach to cleaning up the newline/tab issue
            // would be to remove "\n\t" only when it appears between the
            // '>' and '<' characters.
            // 
            // This could be problematic because it could still remove
            // "significant whitespace" in preformatted text. Also, it might not
            // address the original issue that CleanTagBorders was intended to
            // resolve.
            //
            //content = content.Replace(">\n\t<", "><");
            //content = content.Replace(">" + Environment.NewLine + "\t<", "><");

            // Another approach to cleaning up the newline/tab issue would be
            // to "normalize" the whitespace by replacing with a single space.
            // This could also be problematic because it could introduce
            // "significant whitespace" in the case of preformatted text.
            //
            //content = content.Replace("\n\t", " ");
            //content = content.Replace(Environment.NewLine + "\t", " ");

            return content;
        }

        private static string NormalizeSpaceChars(string content)
        {
            // replace unicode and non-breaking spaces to normal space
            content = Regex.Replace(content, @"[\u0020\u00A0]", " ");
            return content;
        }

        /// <remarks>
        /// <c>PreTidy</c> manipulates the raw HTML content (before parsing into
        /// an <c>HtmlDocument</c>).
        /// </remarks>
        public static string PreTidy(string content, bool removeComments)
        {
            content = NormalizeSpaceChars(content);
            content = CleanTagBorders(content);

            return content;
        }

        /// <remarks>
        /// <c>Tidy</c> manipulates the HTML content after parsing into
        /// an <c>HtmlDocument</c>.
        /// </remarks>
        public static void Tidy(HtmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            MoveUnexpectedListChildrenToListItems(doc);
        }

        private static void MoveUnexpectedListChildrenToListItems(
            HtmlDocument doc)
        {
            // HTML lists (<ol> and <ul>) should only contain <li> elements
            // -- although according to Mozilla they can technically have
            // <script> and <template> elements (but those are very rare).
            //
            // However, ReverseMarkdown handles some scenarios where lists
            // contain other elements (e.g. <ol><p>...</p></ol>).
            //
            // This "cleaner" method fixes the lists so they can be processed
            // in a typical fashion.

            var elements = doc.DocumentNode.SelectNodes(
                "//ol/*[name() != 'li'] | //ul/*[name() != 'li']");

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    // ReverseMarkdown v3.18.0 only handled <p> elements
                    // embedded directly in lists (i.e. not within a <li>
                    // element). To preserve the same functionality going
                    // forward, only process paragraphs (not other elements
                    // incorrectly nested directly within lists).
                    if (element.Name == "p")
                    {
                        MoveParagraphToListItem(element);
                    }
                }
            }
        }

        private static void MoveParagraphToListItem(HtmlNode node)
        {
            Debug.Assert(node.Name == "p");
            Debug.Assert(node.ParentNode.Name == "ol"
                || node.ParentNode.Name == "ul");

            var doc = node.OwnerDocument;
            var paragraphNode = node;
            var previousSibling = node.PreviousSibling;

            // Remove any insignificant whitespace before the <p> element
            if (previousSibling.Name == "#text"
                && string.IsNullOrWhiteSpace(
                    previousSibling.InnerText) == true)
            {
                var textNode = previousSibling;

                previousSibling = textNode.PreviousSibling;
                textNode.Remove();
            }

            // Try to find an <li> element immediately before the <p> element
            HtmlNode listItemNode = null;

            if (previousSibling?.Name == "li")
            {
                listItemNode = previousSibling;

                // Content resembles something like:
                //
                //   <li>Item 1</li>
                //   <p>Paragraph</p>
                //
                // In order to preserve the formatting in ReverseMarkdown
                // v3.18.0, the content needs to be structured like this:
                //
                //   <li><p>Item 1<p>
                //   <p>Paragraph</p></li>

                var lastChild = listItemNode.LastChild;

                HtmlNode previousParagraph = null;

                if (lastChild.Name == "p")
                {
                    previousParagraph = lastChild;
                }

                // If previous <p> was not found, create a new one
                if (previousParagraph == null)
                {
                    previousParagraph = doc.CreateElement("p");

                    lastChild.ParentNode.InsertBefore(
                        previousParagraph,
                        lastChild);

                    lastChild.Remove();
                    previousParagraph.AppendChild(lastChild);
                }
            }

            // If no <li> was found, create a new one
            if (listItemNode == null)
            {
                listItemNode = doc.CreateElement("li");
                paragraphNode.ParentNode.InsertBefore(
                    listItemNode,
                    paragraphNode);
            }

            // Move the <p> element inside the <li> element
            paragraphNode.Remove();
            listItemNode.AppendChild(paragraphNode);
        }
    }
}
