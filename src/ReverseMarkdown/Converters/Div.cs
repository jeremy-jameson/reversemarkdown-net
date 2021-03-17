using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Div : BlockElementConverter
    {
        public Div(Converter converter) : base(converter)
        {
            Converter.Register("div", this);
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return $"{(Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine)}";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return $"{(Td.LastNodeWithinCell(node) ? "" : Environment.NewLine)}";
        }
    }
}
