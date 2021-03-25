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

            content = RemoveMultipleConsecutiveBlankLines(node, content);

            return PrefixBlockquoteLines(content);
        }

        public override string GetMarkdownPrefix(HtmlNode node)
        {
            return Environment.NewLine + Environment.NewLine;
        }
        public override string GetMarkdownSuffix(HtmlNode node)
        {
            return Environment.NewLine + Environment.NewLine;
        }

        private bool IsBlankLine(string line)
        {
            if (line == null)
            {
                return false;
            }
            else if (line == string.Empty
                || (line.Length == 1 && line == "\n")
                || (line.Length == 2 && line == "\r\n"))
            {
                return true;
            }

            return false;
        }

        private string PrefixBlockquoteLine(string line)
        {
            if (IsBlankLine(line) == true)
            {
                return ">"; // omit trailing space
            }

            return "> " + line;
        }

        private string PrefixBlockquoteLines(string text)
        {
            // get the lines based on carriage return and prefix each line
            var lines = text.ReadLines().Select(item =>
                PrefixBlockquoteLine(item));

            // join all the lines to a single line
            var prefixedText = string.Join(Environment.NewLine, lines);

            return prefixedText;
        }

        private string RemoveMultipleConsecutiveBlankLines(
            HtmlNode node,
            string markdown)
        {
            var formatter = Converter.MarkdownFormatterFactory.Create(
                node,
                Converter.Config);

            return formatter.RemoveMultipleConsecutiveBlankLines(markdown);
        }
    }
}
