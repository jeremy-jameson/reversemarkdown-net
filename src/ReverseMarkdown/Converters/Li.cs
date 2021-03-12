using System;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Li : BlockElementConverter
    {
        public Li(Converter converter) : base(converter)
        {
            Converter.Register("li", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            // Standardize whitespace before inner lists so that the following are equivalent
            //   <li>Foo<ul><li>...
            //   <li>Foo\n    <ul><li>...
            foreach (var innerList in node.SelectNodes("//ul|//ol") ?? Enumerable.Empty<HtmlNode>())
            {
                if (innerList.PreviousSibling?.NodeType == HtmlNodeType.Text)
                {
                    innerList.PreviousSibling.InnerHtml = innerList.PreviousSibling.InnerHtml.Chomp();
                }
            }

            RemoveInsignificantWhitespace(node);

            var content = TreatChildren(node).TrimStart();

            // Note: Indentation level is *always* 1.
            //
            // Indentation for "nested" lists is achieved by having each
            // containing <li> element indent all of its children. For example,
            // consider the following HTML:
            //
            //   <ul>
            //     <li>Outer list item
            //       <ul><li>Inner list item</li></ul>
            //     </li>
            //   </ul>
            //
            // The "Inner list item" is indented the first time when converting
            // the "inner" <li> element and subsequently indented a second
            // time when converting the "outer" <li> element.

            var indentation = GetIndentation(1);

            content = Converter.TextFormatter.IndentLines(
                content, indentation, indentBlankLines: false);

            return content.Chomp();
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            if (node.ParentNode != null && node.ParentNode.Name == "ol")
            {
                // index are zero based hence add one
                var index = node.ParentNode.SelectNodes("./li").IndexOf(node) + 1;
                return $"{index}. ";
            }
            else
            {
                return $"{Converter.Config.ListBulletChar} ";
            }
        }

        private static void RemoveInsignificantWhitespace(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            // Check if the list item contains multiple <p> elements separated
            // by whitespace
            node.ChildNodes.ToList().ForEach(child =>
            {
                if (child.Name == "p")
                {
                    if (child.PreviousSibling != null
                        && child.PreviousSibling.Name == "#text")
                    {
                        var textNode = child.PreviousSibling;
                        Debug.Assert(textNode.Name == "#text");

                        var isWhitespace = string.IsNullOrWhiteSpace(
                            textNode.InnerText);

                        if (isWhitespace == true
                            && textNode.PreviousSibling != null
                            && textNode.PreviousSibling.Name == "p")
                        {
                            // Found content like:
                            //
                            //   <li><p>...</p> <p>...</p></li>
                            //
                            // The whitespace between the paragraphs is
                            // considered to be "insignificant whitespace"
                            // because the paragraphs are written on separate
                            // lines in the Markdown (with indentation).
                            //
                            // Remove the insignificant whitespace so that
                            // the second paragraph is not indented by an
                            // extra space.
                            textNode.Remove();
                        }
                    }
                }
            });
        }
    }
}
