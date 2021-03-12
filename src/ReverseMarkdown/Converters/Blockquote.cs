using System;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Blockquote : BlockElementConverter
    {
        public Blockquote(Converter converter) : base(converter)
        {
            Converter.Register("blockquote", this);
        }

        public override string GetMarkdownContent(HtmlNode node)
        {
            var content = base.GetMarkdownContent(node).Trim();

            const string linePrefix = "> ";

            // get the lines based on carriage return and prefix each line
            var lines = content.ReadLines().Select(
                item => linePrefix + item + Environment.NewLine);

            // join all the lines to a single line
            var result = lines.Aggregate(string.Empty, (curr, next) => curr + next);

            return result;
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return Environment.NewLine + Environment.NewLine;
        }
    }
}
