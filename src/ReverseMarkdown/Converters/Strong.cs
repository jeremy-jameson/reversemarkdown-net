using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Strong : InlineElementConverter
    {
        public Strong(Converter converter) : base(converter)
        {
            var elements = new [] { "strong", "b" };
            
            foreach (var element in elements)
            {
                Converter.Register(element, this);
            }
        }

        public override string Convert(HtmlNode node)
        {
            var content = GetMarkdownContent(node);

            if (string.IsNullOrWhiteSpace(content) || AlreadyBold(node))
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
            return "**";
        }

        public override string GetMarkdownSuffix(HtmlNode node)
        {
            var spaceSuffix = (node.NextSibling?.Name == "strong" || node.NextSibling?.Name == "b")
                ? " "
                : "";

            return $"**{spaceSuffix}";
        }

        private static bool AlreadyBold(HtmlNode node)
        {
            return node.Ancestors("strong").Any() || node.Ancestors("b").Any();
        }
    }
}
