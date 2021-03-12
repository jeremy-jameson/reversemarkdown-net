using System.Collections.Generic;

namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for formatting text -- such as wrapping a long line of
    /// text onto multiple lines.
    /// </summary>
    public interface ITextFormatter
    {
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
        string IndentLines(
            string text,
            string indentation = "\t",
            bool indentBlankLines = true);

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
        /// <see cref="WrapTextLine(string, int)"/> when the text that may
        /// already contain multiple lines.
        /// </remarks>
        /// <param name="text">The single line of text to parse.</param>
        IEnumerable<string> ParseChunks(string text);

        /// <summary>
        /// Wraps the specified text onto multiple lines, as necessary, with
        /// each line ideally being no longer than the specified line length.
        /// </summary>
        /// <remarks>
        /// The text is first divided into "chunks" -- typically words -- using
        /// <see cref="ParseChunks(string)"/>. If the length of any "chunk"
        /// within the text is longer than <paramref name="wrapLineLength"/>,
        /// the "chunk" is written on a separate line without being altered
        /// (e.g. hyphenated or truncated). Consequently, there is no guarantee
        /// the length of each line in the formatted string is always less than
        /// <paramref name="wrapLineLength"/>.
        /// </remarks>
        /// <param name="text">The text to format.</param>
        /// <param name="wrapLineLength">The preferred maximum length of each
        /// line in the formatted text. Defaults to 80.</param>
        /// <returns>
        /// <c>null</c> if <see cref="text"/> is <c>null</c>; otherwise the
        /// formatted text with each line separated by
        /// <see cref="Environment.NewLine"/>.</returns>
        string WrapText(string text, int wrapLineLength = 80);

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
        string WrapTextLine(string text, int wrapLineLength = 80);
    }
}
