using System;
using System.Text.RegularExpressions;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="IMarkdownFormatter"/>
    /// interface.
    /// </summary>
    public class DefaultMarkdownFormatter : IMarkdownFormatter
    {
        /// <summary>
        /// Removes multiple consecutive blank lines from the Markdown.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This ensures the generated Markdown does not violate the following
        /// <a href="https://github.com/DavidAnson/markdownlint">markdownlint</a>
        /// rule:
        /// 
        /// <a href="https://github.com/DavidAnson/markdownlint/blob/v0.23.1/doc/Rules.md#md012">
        ///     MD012 - Multiple consecutive blank lines</a>
        /// </para>
        /// </remarks>
        /// <param name="markdown">The Markdown content to format.</param>
        public string RemoveMultipleConsecutiveBlankLines(string markdown)
        {
            if (markdown == null)
            {
                throw new ArgumentNullException("markdown");
            }

            if (markdown.StartsWith(
                Environment.NewLine + Environment.NewLine) == true)
            {
                markdown = markdown.Substring(Environment.NewLine.Length + 1);
            }

            if (markdown.EndsWith(
                Environment.NewLine + Environment.NewLine) == true)
            {
                markdown = markdown.Remove(markdown.Length - Environment.NewLine.Length);
            }

            const string pattern = @"(\r?\n){3,}";
            string replacement = Environment.NewLine + Environment.NewLine;

            return Regex.Replace(markdown, pattern, replacement);
        }
    }
}
