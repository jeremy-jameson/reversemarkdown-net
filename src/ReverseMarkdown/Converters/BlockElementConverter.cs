using HtmlAgilityPack;
using System;
using System.Diagnostics;
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

            if (node.Name == "pre"
                || node.Ancestors("pre").Any() == true
                || node.Descendants("pre").Any() == true)
            {
                // Never trim or wrap text in preformatted content
            }
            else if (node.Descendants("table").Any() == true)
            {
                // Never trim or wrap text in block element that contains a
                // table (e.g. "<div>...<table>...</table></div>")
            }
            else if (node.Name == "blockquote"
                || node.Name == "div"
                || node.Name == "li"
                || node.Name == "p")
            {
                var wrapLineLength = GetWrapLineLength(node);

                if (node.Name != "div")
                {
                    markdown = markdown.Trim();
                }

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

            var listIndentationLevel =
                node.AncestorsAndSelf("li")
                    .Where(x => x.Name == "li")
                    .Count();

            var listIndentation = listIndentationLevel * 4;

            return (80 - blockquoteIndentation - listIndentation);
        }
    }
}
