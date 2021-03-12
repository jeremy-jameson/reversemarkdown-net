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

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            // If p is at the start of a table cell, no leading newline
            return Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Td.LastNodeWithinCell(node) ? "" : Environment.NewLine;
        }
    }
}
