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
            else if (markdown.Contains("{{<") == true
                && (node.Name == "div" || node.Name == "p")
                && node.Ancestors("div").Any() == true)
            {
                // Process only the "outermost" <div> element to avoid issues
                // where wrapping text multiple times would cause undesired
                // results. For example, wrapping lengthy Hugo shortcodes twice
                // will often result in "corruption" due to quoted parameter
                // values being split across multiple lines (during the second
                // call to ITextFormatter.WrapText).
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
