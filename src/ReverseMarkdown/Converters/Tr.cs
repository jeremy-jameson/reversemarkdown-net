using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Tr : BlockElementConverter
    {
        public Tr(Converter converter) : base(converter)
        {
            Converter.Register("tr", this);
        }

        public override string Convert(HtmlNode node)
        {
            var content = GetMarkdownContent(node);

            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }

            return base.Convert(node);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            return base.GetMarkdownContent(node).TrimEnd();
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            // if parent is an ordered or unordered list
            // then table need to be indented as well
            var indent = IndentationFor(node);

            return $"{indent}|";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            var underline = string.Empty;

            // if parent is an ordered or unordered list
            // then table need to be indented as well
            var indent = IndentationFor(node);

            if (IsTableHeaderRow(node) || UseFirstRowAsHeaderRow(node))
            {
                underline = UnderlineFor(node, indent);
            }

            return $"{Environment.NewLine}{underline}";
        }

        private bool UseFirstRowAsHeaderRow(HtmlNode node)
        {
            var tableNode = node.Ancestors("table").FirstOrDefault();
            var firstRow = tableNode?.SelectSingleNode(".//tr");

            if (firstRow == null)
            {
                return false;
            }

            var isFirstRow = firstRow == node;
            var hasNoHeaderRow = tableNode.SelectNodes(".//th")?.FirstOrDefault() == null;

            return isFirstRow
                   && hasNoHeaderRow
                   && Converter.Config.TableWithoutHeaderRowHandling ==
                   Config.TableWithoutHeaderRowHandlingOption.Default;
        }

        private static bool IsTableHeaderRow(HtmlNode node)
        {
            return node.ChildNodes.FindFirst("th") != null;
        }

        private string UnderlineFor(HtmlNode node, string indent)
        {
            var colCount = node.ChildNodes.Count(child => child.Name == "th" || child.Name == "td");

            var cols = new List<string>();

            for (var i = 0; i < colCount; i++ )
            {
                cols.Add("---");
            }

            var colsAggregated = string.Join(" | ", cols);

            return $"{indent}| {colsAggregated} |{Environment.NewLine}";
        }
    }
}
