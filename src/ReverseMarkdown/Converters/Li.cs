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

            var content = base.GetMarkdownContent(node);
            
            content = content.TrimStart();

            var indentation = GetListItemIndentation(node);
            Debug.Assert(indentation.Length >= 2);

            var formatter = Converter.MarkdownFormatterFactory.Create(
                node, Converter.Config);

            content = formatter.IndentLines(
                content, indentation, indentBlankLines: false);

            return content.TrimStart();
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            return base.GetMarkdownPrefixForListItem(node);
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
