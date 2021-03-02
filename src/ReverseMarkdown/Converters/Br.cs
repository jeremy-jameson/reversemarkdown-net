using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Br : BlockElementConverter
    {
        public Br(Converter converter) : base(converter)
        {
            Converter.Register("br", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            return Converter.Config.GithubFlavored ? string.Empty : "  ";
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            // Do not prefix the line break with any content (i.e. set prefix to
            // empty string).
            //
            // This is necessary since the default prefix for block elements
            // is Environment.NewLine
            return string.Empty;
        }
    }
}
