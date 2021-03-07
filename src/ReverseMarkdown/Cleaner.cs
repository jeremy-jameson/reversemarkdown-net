
using System;
using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    public static class Cleaner
    {
        private static string CleanTagBorders(string content)
        {
            // content from some htl editors such as CKEditor emits newline and tab between tags, clean that up

            // Original approach to cleaning up the newline/tab issue.
            // This could be problematic because it could potentially remove
            // "significant whitespace" (e.g. "<b>some\n\tbold text</b>"
            // or "<pre>function foo {\n\t...</pre>"
            //
            //content = content.Replace("\n\t", "");
            //content = content.Replace(Environment.NewLine + "\t", "");

            // A slightly safer approach to cleaning up the newline/tab issue
            // would be to remove "\n\t" only when it appears between the
            // '>' and '<' characters.
            // 
            // This could be problematic because it could still remove
            // "significant whitespace" in preformatted text. Also, it might not
            // address the original issue that CleanTagBorders was intended to
            // resolve.
            //
            //content = content.Replace(">\n\t<", "><");
            //content = content.Replace(">" + Environment.NewLine + "\t<", "><");

            // Another approach to cleaning up the newline/tab issue would be
            // to "normalize" the whitespace by replacing with a single space.
            // This could also be problematic because it could introduce
            // "significant whitespace" in the case of preformatted text.
            //
            //content = content.Replace("\n\t", " ");
            //content = content.Replace(Environment.NewLine + "\t", " ");

            return content;
        }

        private static string NormalizeSpaceChars(string content)
        {
            // replace unicode and non-breaking spaces to normal space
            content = Regex.Replace(content, @"[\u0020\u00A0]", " ");
            return content;
        }

        public static string PreTidy(string content, bool removeComments)
        {
            content = NormalizeSpaceChars(content);
            content = CleanTagBorders(content);

            return content;
        }
    }
}
