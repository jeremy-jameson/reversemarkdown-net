using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Table : BlockElementConverter
    {
        public Table(Converter converter) : base(converter)
        {
            Converter.Register("table", this);
        }

        public override string Convert(HtmlNode node)
        {
            // "colspan" and "rowspan" are not supported in pipe-delimited
            // Markdown tables, so leave as HTML
            if (node.Descendants()
                .Where(x =>
                    x.GetAttributeValue("colspan", null) != null
                    || x.GetAttributeValue("rowspan", null) != null)
                .Any() == true)
            {
                return node.OuterHtml;
            }

            return base.Convert(node);
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            // if table does not have a header row , add empty header row if set in config
            var useEmptyRowForHeader = this.Converter.Config.TableWithoutHeaderRowHandling ==
                                       Config.TableWithoutHeaderRowHandlingOption.EmptyRow;

            var emptyHeaderRow = HasNoTableHeaderRow(node) && useEmptyRowForHeader
                ? EmptyHeader(node)
                : string.Empty;

            return $"{Environment.NewLine}{Environment.NewLine}{emptyHeaderRow}";
        }

        private static bool HasNoTableHeaderRow(HtmlNode node)
        {
            var thNode = node.SelectNodes("//th")?.FirstOrDefault();
            return thNode == null;
        }

        private static string EmptyHeader(HtmlNode node)
        {
            var firstRow = node.SelectNodes("//tr")?.FirstOrDefault();

            if (firstRow == null)
            {
                return string.Empty;
            }

            var colCount = firstRow.ChildNodes.Count(n => n.Name.Contains("td"));

            var headerRowItems = new List<string>();
            var underlineRowItems = new List<string>();

            for (var i = 0; i < colCount; i++ )
            {
                headerRowItems.Add("<!---->");
                underlineRowItems.Add("---");
            }

            var headerRow = $"| {string.Join(" | ", headerRowItems)} |{Environment.NewLine}";
            var underlineRow = $"| {string.Join(" | ", underlineRowItems)} |{Environment.NewLine}";

            return headerRow + underlineRow;
        }
    }
}
