using HtmlAgilityPack;
using System;

namespace ReverseMarkdown.Converters
{
    public abstract class BlockElementConverter : ConverterBase
    {
        public BlockElementConverter(Converter converter) : base(converter)
        {
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Environment.NewLine;
        }
    }
}
