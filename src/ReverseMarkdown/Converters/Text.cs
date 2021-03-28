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

            content = ReplaceTextPatterns(content);

            return content;
        }

        private string ReplaceTextPatterns(string content)
        {
            var replacementPatterns =
                Converter.Config.TextReplacementPatterns;

            foreach (var replacementPattern in replacementPatterns)
            {
                var regex = replacementPattern.RegularExpression;
                var replacement = replacementPattern.Replacement;

                content = regex.Replace(content, replacement);
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
