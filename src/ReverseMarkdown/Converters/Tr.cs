﻿using System;
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
            return "|";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            var underline = string.Empty;

            if (IsTableHeaderRow(node) || UseFirstRowAsHeaderRow(node))
            {
                underline = UnderlineFor(node);
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

        private string UnderlineFor(HtmlNode node)
        {
            var colCount = node.ChildNodes.Count(child => child.Name == "th" || child.Name == "td");

            var cols = new List<string>();

            for (var i = 0; i < colCount; i++)
            {
                cols.Add("---");
            }

            var colsAggregated = string.Join(" | ", cols);

            return $"| {colsAggregated} |{Environment.NewLine}";
        }
    }
}
