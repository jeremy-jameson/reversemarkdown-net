using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Aside : BlockElementConverter
    {
        public Aside(Converter converter)
            : base(converter)
        {
            Converter.Register("aside", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            return base.GetMarkdownContent(node).Trim();
        }
    }
}
