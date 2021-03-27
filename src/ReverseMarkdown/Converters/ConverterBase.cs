using System.Diagnostics;
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

        protected virtual string EncodeUrlForMarkdown(string url)
        {
            if (url.StartsWith("#") == true)
            {
                var encodedUrl = url
                    .Trim()
                    .Replace("(", "-")
                    .Replace(")", "-")
                    .Replace(" ", "-");

                while (encodedUrl.Contains("--") == true)
                {
                    encodedUrl = encodedUrl.Replace("--", "-");
                }

                return encodedUrl.TrimEnd(new char[] { '-' });
            }

            return url
                .Trim()
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace(" ", "%20");
        }

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

        protected virtual string GetListItemIndentation(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            // Note: Indentation is only for the specified <li> element.
            //
            // Indentation for "nested" lists is achieved by having each
            // containing <li> element indent all of its children. For example,
            // consider the following HTML:
            //
            //   <ul>
            //     <li>Outer list item
            //       <ul><li>Inner list item</li></ul>
            //     </li>
            //   </ul>
            //
            // The "Inner list item" is indented the first time when converting
            // the "inner" <li> element and subsequently indented a second
            // time when converting the "outer" <li> element.

            // For items in unordered lists (<ul>), child content within list
            // items is indented by two spaces (because "- ".Length == 2),
            // for example:
            //
            // - Outer item
            //   - Inner item
            //   - Inner item
            // - Outer item
            //
            // For items in ordered lists (<ol>), child content within list
            // items is typically indented by three spaces (since
            // "1. ".Length == 2) -- unless the number is greater than 10, in
            // which case, child content is indented by four spaces (since
            // "10. ".Length == 4). For example:
            //
            // 1. Item 1
            // 2. Item 2
            // 3. Item 3
            // 4. Item 4
            // 5. Item 5
            // 6. Item 6
            // 7. Item 7
            // 8. Item 8
            // 9. Item 9
            //    1. Item 9.1
            // 10. Item 10
            //     1. Item 10.1
            //
            // The general rule is that child content in <li> elements is
            // indented by the length of the "prefix" used for the list item.

            string listItemPrefix = GetMarkdownPrefixForListItem(node);

            return new string(' ', listItemPrefix.Length);
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

        protected virtual string GetMarkdownPrefixForListItem(HtmlNode node)
        {
            Debug.Assert(node.Name == "li");

            var formatter = Converter.MarkdownFormatterFactory.Create(
                node,
                Converter.Config);

            return formatter.GetMarkdownPrefixForListItem(node);
        }

        public virtual string GetMarkdownSuffix(HtmlNode node)
        {
            return string.Empty;
        }
    }
}
