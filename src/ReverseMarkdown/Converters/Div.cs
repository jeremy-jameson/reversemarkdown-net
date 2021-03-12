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

        public override string Convert(HtmlNode node)
        {
            // if there is a block child then ignore adding the newlines for div
            if ((node.ChildNodes.Count == 1 && IsBlockTag(node.FirstChild.Name)))
            {
                return GetMarkdownContent(node);
            }

            return base.Convert(node);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            var content = string.Empty;

            do
            {
                if (node.ChildNodes.Count == 1 && node.FirstChild.Name == "div")
                {
                    node = node.FirstChild;
                    continue;
                }

                content = base.GetMarkdownContent(node);
                break;
            } while (true);

            return content;
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return $"{(Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine)}";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return $"{(Td.LastNodeWithinCell(node) ? "" : Environment.NewLine)}";
        }

        private static bool IsBlockTag(string name)
        {
            switch (name)
            {
                // Note: The following list is identical to what was in the code
                // before
                //
                // TODO: Clean this up and add other HTML block elements as
                // necessary (e.g. "ul")
                case "pre":
                case "p":
                case "ol":
                case "oi": // ??????????
                case "table":
                    return true;

                default:
                    return false;
            }
        }
    }
}
