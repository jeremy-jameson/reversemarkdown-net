using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="ITextFormatter"/>
    /// interface.
    /// </summary>
    public class DefaultTextFormatter : ITextFormatter
    {
        private string IndentLine(
            string line,
            string indentation,
            bool indentBlankLine)
        {
            if (indentBlankLine == false
                && IsBlankLine(line) == true)
            {
                return line;
            }

            return indentation + line;
        }

        /// <summary>
        /// Indents the specified text.
        /// </summary>
        /// <param name="text">The text to indent.</param>
        /// <param name="indentation">The indentation to use for each line of
        /// text. Defaults to a single tab (<c>'\t'</c>).</param>
        /// <param name="indentBlankLines">If <c>true</c>, blank lines in the
        /// text are indented; if <c>false</c>, blank lines remain empty in the
        /// indented result. Defaults to <c>true</c>.</param>
        /// <returns>
        /// <c>null</c> if <see cref="text"/> is <c>null</c>; otherwise the
        /// indentated text with each line separated by
        /// <see cref="Environment.NewLine"/>.
        /// </returns>
        public string IndentLines(
            string text,
            string indentation,
            bool indentBlankLines)
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                return text;
            }

            var lines = text.ReadLines().Select(item =>
                IndentLine(item, indentation, indentBlankLines));

            var indentedText = string.Join(Environment.NewLine, lines);

            return indentedText;
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

        /// <summary>
        /// Splits the specified line of text into individual "chunks" --
        /// typically single words -- which can then be used to wrap the text
        /// onto multiple lines, as necessary, using
        /// <see cref="WrapTextLine(string, int)"/>.
        /// </summary>
        /// <remarks>
        /// An <see cref="ArgumentException"/> is thrown if
        /// <paramref name="text"/> contains a line feed character (<c>\n</c>).
        /// <see cref="WrapText(string, int)"/> should be used instead of
        /// <see cref="WrapTextLine(string, int)"/> when the text may already
        /// contain multiple lines.
        /// </remarks>
        /// <param name="text">The single line of text to parse.</param>
        public virtual IEnumerable<string> ParseChunks(string text)
        {
            if (text == null)
            {
                return Enumerable.Empty<string>();
            }
            else if (text.IndexOf('\n') != -1)
            {
                throw new ArgumentException(
                    "Cannot parse chunks from text because the text contains a"
                        + " line feed.",
                    "text");
            }

            return text.Split(' ');
        }

        /// <summary>
        /// Wraps the specified text onto multiple lines, as necessary, with
        /// each line ideally being no longer than the specified line length.
        /// </summary>
        /// <remarks>
        /// If the length of any word within the text is longer than
        /// <paramref name="wrapLineLength"/>, the word is written on a separate
        /// line without being altered (e.g. hyphenated or truncated). In other
        /// words, there is no guarantee the length of each line in the
        /// formatted string is always less than
        /// <paramref name="wrapLineLength"/>.
        /// </remarks>
        /// <param name="text">The text to format.</param>
        /// <param name="wrapLineLength">The preferred maximum length of each
        /// line in the formatted text. Defaults to 80.</param>
        /// <returns>
        /// <c>null</c> if <see cref="text"/> is <c>null</c>; otherwise the
        /// formatted text with each line separated by
        /// <see cref="Environment.NewLine"/>.</returns>
        public virtual string WrapText(string text, int wrapLineLength)
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                return text;
            }
            else if (text.Length < wrapLineLength)
            {
                return text;
            }

            StringBuilder wrappedLines = new StringBuilder(text.Length);

            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.Length > 0 && line[line.Length - 1] == '\r')
                {
                    line = line.Remove(line.Length - 1);
                }

                var wrappedLine = WrapTextLine(line, wrapLineLength);

                wrappedLines.AppendLine(wrappedLine);
            }

            // Remove trailing space
            Debug.Assert(wrappedLines[wrappedLines.Length - 2] == '\r');
            Debug.Assert(wrappedLines[wrappedLines.Length - 1] == '\n');
            wrappedLines.Remove(wrappedLines.Length - 2, 2);

            return wrappedLines.ToString();
        }

        /// <summary>
        /// Wraps the specified line of text onto multiple lines, as necessary,
        /// with each line ideally being no longer than the specified line
        /// length.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the length of any word within the text is longer than
        /// <paramref name="wrapLineLength"/>, the word is written on a separate
        /// line without being altered (e.g. hyphenated or truncated). In other
        /// words, there is no guarantee the length of each line in the
        /// formatted string is always less than
        /// <paramref name="wrapLineLength"/>.
        /// </para>
        /// <para>
        /// An <see cref="ArgumentException"/> is thrown if
        /// <paramref name="text"/> contains a line feed character (<c>\n</c>).
        /// <see cref="WrapText(string, int)"/> should be used instead whenever
        /// the text might already contain multiple lines of text.
        /// </para>
        /// </remarks>
        /// <param name="text">The single line of text to format.</param>
        /// <param name="wrapLineLength">The preferred maximum length of each
        /// line in the formatted text. Defaults to 80.</param>
        /// <returns>
        /// <c>null</c> if <see cref="text"/> is <c>null</c>; otherwise the
        /// formatted text with each line separated by
        /// <see cref="Environment.NewLine"/>.</returns>
        public virtual string WrapTextLine(string text, int wrapLineLength)
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                return text;
            }
            else if (text.IndexOf('\n') != -1)
            {
                throw new ArgumentException(
                    "Cannot wrap text line because the text already contains a"
                        + " line feed.",
                    "text");
            }
            else if (text.Length < wrapLineLength)
            {
                return text;
            }

            var lines = new List<string>();
            var chunks = ParseChunks(text);

            var capacity = Math.Min(wrapLineLength, 200);

            var line = new StringBuilder(capacity);

            foreach (var chunk in chunks)
            {
                if ((line.Length + chunk.Length) > wrapLineLength)
                {
                    if (line.Length > 0)
                    {
                        // Remove trailing space
                        Debug.Assert(line[line.Length - 1] == ' ');
                        line.Remove(line.Length - 1, 1);

                        lines.Add(line.ToString());
                        line.Clear();
                    }
                }

                line.Append(chunk);
                line.Append(' ');
            }

            // Remove trailing space
            Debug.Assert(line[line.Length - 1] == ' ');
            line.Remove(line.Length - 1, 1);

            if (line.Length > 0)
            {
                lines.Add(line.ToString());
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
