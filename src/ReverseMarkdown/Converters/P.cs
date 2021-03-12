using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class P : BlockElementConverter
    {
        public P(Converter converter) : base(converter)
        {
            Converter.Register("p", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            var markdown = base.GetMarkdownContent(node).Trim();

            var wrapLineLength = GetWrapLineLength(node);

            markdown = Converter.TextFormatter.WrapText(
                markdown,
                wrapLineLength);

            return markdown;
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            string parentName = node.ParentNode.Name.ToLowerInvariant();

            if (parentName == "ol" || parentName == "ul")
            {
                throw new InvalidOperationException("Malformed list.");
            }

            // If p follows a list item, add newline and indent it
            var length = node.Ancestors("ol").Count() + node.Ancestors("ul").Count();
            bool parentIsList = parentName == "li";
            if (parentIsList && node.ParentNode.FirstChild != node)
                return Environment.NewLine + (new string(' ', length * 4));

            // If p is at the start of a table cell, no leading newline
            return Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Td.LastNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        private int GetWrapLineLength(HtmlNode node)
        {
            var blockquoteIndentation = node.Ancestors("blockquote").Count()
                * "> ".Length;

            return (80 - blockquoteIndentation);
        }
    }
}
