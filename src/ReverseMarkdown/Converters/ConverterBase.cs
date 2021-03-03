using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public abstract class ConverterBase : IConverter
    {
        protected ConverterBase(Converter converter) 
        {
            Converter = converter;
        }

        protected Converter Converter { get; }

        protected string TreatChildren(HtmlNode node)
        {
            var result = string.Empty;

            return !node.HasChildNodes 
                ? result 
                : node.ChildNodes.Aggregate(result, (current, nd) => current + Treat(nd));
        }

        private string Treat(HtmlNode node) {
            return Converter.Lookup(node.Name).Convert(node);
        }

        protected string ExtractTitle(HtmlNode node)
        {
            var title = node.GetAttributeValue("title", "");

            return title;
        }

        protected string DecodeHtml(string html)
        {
            return System.Net.WebUtility.HtmlDecode(html);
        }

        protected virtual int GetIndentationLevel(HtmlNode node)
        {
            var indentationLevel = node.Ancestors("ol").Count()
                + node.Ancestors("ul").Count();

            return indentationLevel;
        }

        protected virtual string GetIndentation(int indentationLevel)
        {
            return new string(' ', indentationLevel * 4);
        }

        protected virtual string IndentationFor(HtmlNode node)
        {
            var indentationLevel = GetIndentationLevel(node);

            return GetIndentation(indentationLevel);
        }

        public virtual string Convert(HtmlNode node)
        {
            var prefix = GetMarkdownPrefix(node);
            var content = GetMarkdownContent(node);
            var suffix = GetMarkdownSuffix(node);

            return $"{prefix}{content}{suffix}";
        }

        public virtual string GetMarkdownContent(HtmlNode node)
        {
            return TreatChildren(node);
        }

        public virtual string GetMarkdownPrefix(HtmlNode node)
        {
            return string.Empty;
        }

        public virtual string GetMarkdownSuffix(HtmlNode node)
        {
            return string.Empty;
        }
    }
}
