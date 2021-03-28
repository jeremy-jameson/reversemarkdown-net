using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    public class TextReplacementPattern
    {
        public Regex RegularExpression { get; set; }

        public string Replacement { get; set; }

        public TextReplacementPattern(
            string pattern,
            string replacement)
        {
            RegularExpression = new Regex(pattern, RegexOptions.Compiled);
            Replacement = replacement;
        }
    }
}
