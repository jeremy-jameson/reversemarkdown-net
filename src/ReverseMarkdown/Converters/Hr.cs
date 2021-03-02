using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Hr : BlockElementConverter
    {
        public Hr(Converter converter) : base(converter)
        {
            Converter.Register("hr", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            return "* * *";
        }
    }
}
