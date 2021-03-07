using System;
using System.Linq;

using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Blockquote : ConverterBase
    {
        public Blockquote(Converter converter) : base(converter)
        {
            Converter.Register("blockquote", this);
        }

        public override string Convert(HtmlNode node)
        {
            var content = TreatChildren(node).Trim();

            var prefix = IndentationFor(node) + "> ";

            // get the lines based on carriage return and prefix each line
            var lines = content.ReadLines().Select(item => prefix + item + Environment.NewLine);

            // join all the lines to a single line
            var result = lines.Aggregate(string.Empty, (curr, next) => curr + next);

            return $"{Environment.NewLine}{Environment.NewLine}{result}{Environment.NewLine}";
        }
    }
}
