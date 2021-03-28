using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Em : InlineElementConverter
    {
        public Em(Converter converter) : base(converter)
        {
            var elements = new [] { "em", "i" };
            
            foreach (var element in elements)
            {
                Converter.Register(element, this);
            }
        }

        public override string Convert(HtmlNode node)
        {
            var content = GetMarkdownContent(node);
            
            if (string.IsNullOrWhiteSpace(content) || AlreadyItalic(node))
            {
                return content;
            }

            return base.Convert(node);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            var content = TreatChildren(node);

            // Check if the content ends with a backslash
            //
            // If the content ends with a single backslash, then escape that
            // backslash -- otherwise, the first character from the suffix ('*')
            // would be escaped in the Markdown by the trailing backslash
            if (content != null
                && content.EndsWith(@"\") == true
                && content.EndsWith(@"\\") == false)
            {
                content += @"\";
            }

            return content;
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return $"{Converter.Config.EmphasisChar}";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            var spaceSuffix = (node.NextSibling?.Name == "i" || node.NextSibling?.Name == "em")
                ? " "
                : "";

            return $"{Converter.Config.EmphasisChar}{spaceSuffix}";
        }

        private bool AlreadyItalic(HtmlNode node)
        {
            return node.Ancestors("i").Any() || node.Ancestors("em").Any();
        }
    }
}
