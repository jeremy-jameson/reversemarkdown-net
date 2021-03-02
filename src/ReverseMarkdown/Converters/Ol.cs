using System;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Ol : BlockElementConverter
    {
        public Ol(Converter converter) : base(converter)
        {
            var elements = new[] { "ol", "ul" };

            foreach (var element in elements)
            {
                Converter.Register(element, this);
            }
        }

        public override string Convert(HtmlNode node)
        {
            // Lists inside tables are not supported as markdown, so leave as HTML
            if (node.Ancestors("table").Any())
            {
                return node.OuterHtml;
            }

            return base.Convert(node);
        }

        private bool IsNestedList(HtmlNode node)
        {
            Debug.Assert(node.Name == "ol" || node.Name == "ul");

            // Return true if the specified list is a child of another list
            string parentName = node.ParentNode.Name.ToLowerInvariant();

            return (parentName == "ol" || parentName == "ul");
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            // Prevent blank lines being inserted in nested lists
            return IsNestedList(node) ? string.Empty : Environment.NewLine;
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            // Prevent blank lines being inserted in nested lists
            return IsNestedList(node) ? string.Empty : Environment.NewLine;
        }
    }
}
