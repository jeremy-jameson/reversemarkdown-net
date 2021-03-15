using HtmlAgilityPack;
using System;
using System.Linq;
using Xunit;

namespace ReverseMarkdown.Test
{
    public class DefaultMarkdownFormatterTests
    {
        private IMarkdownFormatter CreateTestFormatter()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml("<body/>");

            return new DefaultMarkdownFormatter(doc.DocumentNode.FirstChild);
        }

        #region ParseChunks

        [Fact]
        public void ParseChunks_ReturnsEmptyEnumerableWhenTextIsNull()
        {
            string text = null;

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.True(actual.Any() == false);
        }

        [Fact]
        public void ParseChunks_ThrowsWhenTextContainsLineFeed()
        {
            var text = "\n";
            var expectedExceptionText =
                "Cannot parse chunks from text because the text contains a"
                    + " line feed.";

            ITextFormatter formatter = CreateTestFormatter();

            var ex = Assert.Throws<ArgumentException>(() =>
                formatter.ParseChunks(text));

            Assert.Equal("text", ex.ParamName);
            Assert.StartsWith(expectedExceptionText, ex.Message);
        }

        [Fact]
        public void ParseChunks_WithEmptyText()
        {
            var text = string.Empty;
            var expected = new string[] { string.Empty };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithTwoWords()
        {
            var text = "foo bar";
            var expected = new string[] { "foo", "bar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithTabs_PreservesTabs()
        {
            var text = "foo\tbar \tfoobar";
            var expected = new string[] { "foo\tbar", "\tfoobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineImage()
        {
            var text = "foo bar"
                + " ![Example image](http://example.com/img.png) foobar";

            var expected = new string[] { "foo", "bar",
                "![Example image](http://example.com/img.png)", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineImageFollowedByComma()
        {
            var text = "foo bar"
                + " ![Example image](http://example.com/img.png), foobar";

            var expected = new string[] { "foo", "bar",
                "![Example image](http://example.com/img.png),", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineImageFollowedByPeriod()
        {
            var text = "foo bar"
                + " ![Example image](http://example.com/img.png). foobar";

            var expected = new string[] { "foo", "bar",
                "![Example image](http://example.com/img.png).", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineImageInAngleBrackets()
        {
            var text = "foo bar"
                + " <![Example image](http://example.com/img.png)> foobar";

            var expected = new string[] { "foo", "bar",
                "<![Example image](http://example.com/img.png)>", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineImageAtEndOfLine()
        {
            var text = "foo bar"
                + " ![Example image](http://example.com/img.png)";

            var expected = new string[] { "foo", "bar",
                "![Example image](http://example.com/img.png)" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineLink()
        {
            var text = "foo bar [Example link](http://example.com) foobar";
            var expected = new string[] { "foo", "bar",
                "[Example link](http://example.com)", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        #endregion

        #region RemoveMultipleConsecutiveBlankLines

        [Fact]
        public void RemoveMultipleConsecutiveBlankLines_ThrowsWhenMarkdownIsNull()
        {
            string markdown = null;
            var expectedExceptionText = "Value cannot be null.";

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

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

            IMarkdownFormatter formatter = CreateTestFormatter();

            var actual = formatter.RemoveMultipleConsecutiveBlankLines(
                markdown);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
