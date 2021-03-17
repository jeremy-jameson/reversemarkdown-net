using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

            var formatter = Converter.MarkdownFormatterFactory.Create(
                node, Converter.Config);

            var formattingRules = formatter.GetFormattingRules();

            if (formattingRules.CanTrim == true)
            {
                markdown = markdown.Trim();
            }

            if (formattingRules.WrapLineLength < int.MaxValue)
            {
                markdown = formatter.WrapText(
                    markdown,
                    formattingRules.WrapLineLength);
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

            var listIndentation = new StringBuilder();

            node.AncestorsAndSelf("li")
                .Where(x => x.Name == "li")
                .ToList()
                .ForEach(listItemNode =>
                {
                    var listItemPrefix = GetMarkdownPrefixForListItem(listItemNode);

                    listIndentation.Append(new string(' ', listItemPrefix.Length));
                });

            return (80 - blockquoteIndentation - listIndentation.Length);
        }
    }
}
