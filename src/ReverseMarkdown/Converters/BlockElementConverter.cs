using HtmlAgilityPack;
using System;
using System.Linq;

namespace ReverseMarkdown.Converters
{
    public abstract class BlockElementConverter : ConverterBase
    {
        public BlockElementConverter(Converter converter) : base(converter)
        {
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            var markdown = base.GetMarkdownContent(node);

            if (node.Name == "blockquote"
                || node.Name == "p")
            {
                var wrapLineLength = GetWrapLineLength(node);

                markdown = markdown.Trim();
                markdown = Converter.TextFormatter.WrapText(
                    markdown,
                    wrapLineLength);
            }

            return markdown;
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Environment.NewLine;
        }

        private int GetWrapLineLength(HtmlNode node)
        {
            if (node.Ancestors("table").Any() == true)
            {
                return int.MaxValue;
            }

            var blockquoteIndentationLevel =
                node.AncestorsAndSelf("blockquote")
                    .Where(x => x.Name == "blockquote")
                    .Count();

            var blockquoteIndentation =
                blockquoteIndentationLevel * ("> ".Length);

            var listIndentation = node.Ancestors("li").Count() * 4;

            return (80 - blockquoteIndentation - listIndentation);
        }
    }
}
