using System;
using Xunit;

namespace ReverseMarkdown.Test
{
    public class DefaultMarkdownFormatterTests
    {
        #region RemoveMultipleConsecutiveBlankLines

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_ThrowsWhenMarkdownIsNull()
        {
            string markdown = null;
            var expectedExceptionText = "Value cannot be null.";

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                formatter.RemoveMultipleConsecutiveBlankLines(markdown));

            Assert.Equal("markdown", ex.ParamName);
            Assert.StartsWith(expectedExceptionText, ex.Message);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithEmptyString()
        {
            var markdown = string.Empty;
            var expected = markdown;

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithOneBlankLine()
        {
            var markdown =
@"Paragraph 1

Paragraph 2";

            var expected = markdown;

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithTwoBlankLines()
        {
            var markdown =
@"Paragraph 1


Paragraph 2";

            var expected =
@"Paragraph 1

Paragraph 2";

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithThreeBlankLines()
        {
            var markdown =
@"Paragraph 1



Paragraph 2";

            var expected =
@"Paragraph 1

Paragraph 2";

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithTwoBlankLinesAtEnd()
        {
            var markdown =
@"Paragraph 1

";

            var expected =
@"Paragraph 1
";

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_WithTwoBlankLinesAtStart()
        {
            var markdown =
@"

Paragraph 1";

            var expected =
@"
Paragraph 1";

            IMarkdownFormatter formatter = new DefaultMarkdownFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
