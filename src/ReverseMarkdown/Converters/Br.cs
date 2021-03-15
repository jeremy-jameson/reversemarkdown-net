using System.Diagnostics;
using System.Linq;
using System.Text;
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
            if (Converter.Config.GithubFlavored == true)
            {
                if (IsBackslashPermittedForLineBreak(node) == true)
                {
                    return @"\";
                }
                else
                {
                    return string.Empty;
                }
            }

            return "  ";
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

        private bool IsBackslashPermittedForLineBreak(HtmlNode node)
        {
            Debug.Assert(node.Name == "br");

            if (node.Ancestors("td").Any() == true)
            {
                // Do not include backslash for <br> within <td>
                // (pipe-delimited Markdown tables)

                return false;
            }

            // Note: In Markdown, hard line breaks using backslashes
            // render as literal backslashes where there is no
            // additional text after the line break.

            if (node.NextSibling?.Name == "br")
            {
                return IsBackslashPermittedForLineBreak(node.NextSibling);
            }
            else if (node.NextSibling != null)
            {
                var nextSibling = node.NextSibling;

                while (nextSibling != null)
                {
                    if (base.Converter.HtmlFormatter.IsBlockElement(
                        nextSibling) == true)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(
                        nextSibling.InnerText) == false)
                    {
                        return true;
                    }

                    nextSibling = nextSibling.NextSibling;
                }
            }

            // Omit the backslash when there is no content after the
            // line break

            return false;
        }
    }
}
