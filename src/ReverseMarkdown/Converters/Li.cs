using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Li : BlockElementConverter
    {
        public Li(Converter converter) : base(converter)
        {
            Converter.Register("li", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            // Standardize whitespace before inner lists so that the following are equivalent
            //   <li>Foo<ul><li>...
            //   <li>Foo\n    <ul><li>...
            foreach (var innerList in node.SelectNodes("//ul|//ol") ?? Enumerable.Empty<HtmlNode>())
            {
                if (innerList.PreviousSibling?.NodeType == HtmlNodeType.Text)
                {
                    innerList.PreviousSibling.InnerHtml = innerList.PreviousSibling.InnerHtml.Chomp();
                }
            }

            var content = TreatChildren(node);

            return content.Chomp();
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            var indentationLevel = GetIndentationLevel(node);
            Debug.Assert(indentationLevel > 0);
            indentationLevel--; // no indentation for "first level" list items

            var indentation = GetIndentation(indentationLevel);
            var prefix = GetPrefixForListItem(node);

            return $"{indentation}{prefix}";
        }

        private string GetPrefixForListItem(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            if (node.ParentNode != null && node.ParentNode.Name == "ol")
            {
                // index are zero based hence add one
                var index = node.ParentNode.SelectNodes("./li").IndexOf(node) + 1;
                return $"{index}. ";
            }
            else
            {
                return $"{Converter.Config.ListBulletChar} ";
            }
        }
    }
}
