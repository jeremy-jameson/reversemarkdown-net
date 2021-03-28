using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Text : InlineElementConverter
    {
        private readonly Dictionary<string, string> _escapedKeyChars = new Dictionary<string, string>();

        public Text(Converter converter) : base(converter)
        {
            // Note the order of items is important -- need to escape double
            // backslashes *before* escaping other items
            _escapedKeyChars.Add(@"\\", @"\\\\");

            _escapedKeyChars.Add(@"\$", @"\\$");
            _escapedKeyChars.Add(@"\%", @"\\%");
            _escapedKeyChars.Add(@"\&", @"\\&");
            _escapedKeyChars.Add(@"\.", @"\\.");
            _escapedKeyChars.Add(@"\[", @"\\[");
            _escapedKeyChars.Add(@"\{", @"\\{");

            _escapedKeyChars.Add("*", @"\*");

            Converter.Register("#text", this);
        }

        public override string Convert(HtmlNode node)
        {
            if (string.IsNullOrWhiteSpace(node.InnerText) == true)
            {
                return TreatEmpty(node);
            }

            return base.Convert(node);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            // Prevent &lt; and &gt; from being converted to < and > as this will be interpreted as HTML by markdown
            string content = node.InnerText
                .Replace("&lt;", "%3C")
                .Replace("&gt;", "%3E");

            content = DecodeHtml(content);

            // Not all renderers support hex encoded characters, so convert back to escaped HTML
            content = content
                .Replace("%3C", "&lt;")
                .Replace("%3E", "&gt;");

            content = EscapeKeyChars(content);

            return content;
        }

        private string EscapeKeyChars(string content)
        {
            // Escape '+' and '-' at beginning of line (to avoid mistaking plain
            // text for a list)
            content = Regex.Replace(content, @"^\+ ", @"\+ ");
            content = Regex.Replace(content, "^- ", @"\- ");

            // Escape '_' that is *not* followed by a word character
            // Note: "[^\w]" is equivalent to [^a-zA-Z0-9_]
            content = Regex.Replace(content, @"(_[^\w])", @"\$1");

            // Escape '_' after a space
            content = Regex.Replace(content, @"( _)", @" \_");

            // Escape '_' at beginning of line
            content = Regex.Replace(content, @"(^_)", @"\$1");

            // Escape double underscores
            content = Regex.Replace(content, @"(__)", @"\_\_");

            foreach (var item in _escapedKeyChars)
            {
                content = content.Replace(item.Key, item.Value);
            }
            
            return content;
        }
        
        private static string TreatEmpty(HtmlNode node)
        {
            var content = "";

            var parent = node.ParentNode;
            
            if (parent.Name == "ol" || parent.Name == "ul")
            {
                content = "";
            }
            else if(node.InnerText == " ")
            {
                content = " ";
            }
            
            return content;
        }
    }
}
