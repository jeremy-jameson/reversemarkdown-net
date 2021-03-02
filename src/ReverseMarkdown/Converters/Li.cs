using System;
using System.Diagnostics;
using System.Linq;

using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Li : ConverterBase
    {
        public Li(Converter converter) : base(converter)
        {
            Converter.Register("li", this);
        }

        public override string Convert(HtmlNode node)
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
            var indentationLevel = GetIndentationLevel(node);
            Debug.Assert(indentationLevel > 0);
            indentationLevel--; // no indentation for "first level" list items

            var indentation = GetIndentation(indentationLevel);
            var prefix = PrefixFor(node);

            return $"{indentation}{prefix}{content.Chomp()}{Environment.NewLine}";
        }

        private string PrefixFor(HtmlNode node)
        {
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
