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
            // If p is at the start of a table cell, no leading newline
            return Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Td.LastNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        private int GetWrapLineLength(HtmlNode node)
        {
            if (node.Ancestors("table").Any() == true)
            {
                return int.MaxValue;
            }

            var blockquoteIndentation = node.Ancestors("blockquote").Count()
                * "> ".Length;

            var listIndentation = node.Ancestors("li").Count() * 4;

            return (80 - blockquoteIndentation - listIndentation);
        }
    }
}
