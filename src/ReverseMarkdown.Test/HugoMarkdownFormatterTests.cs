using HtmlAgilityPack;
using System;
using System.Linq;
using Xunit;

namespace ReverseMarkdown.Test
{
    public class HugoMarkdownFormatterTests
    {
        private IMarkdownFormatter CreateTestFormatter(string html = "<body/>")
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return new HugoMarkdownFormatter(
                doc.DocumentNode.FirstChild,
                new Config());
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

        [Fact]
        public void ParseChunks_WithHugoShortcode()
        {
            var text = "foo bar {{< gist spf13 7896402 >}} foobar";
            var expected = new string[] { "foo", "bar",
                "{{<", "gist", "spf13", "7896402 >}}", "foobar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithHugoShortcodeWithQuotes()
        {
            var text = "{{< kbd \"stsadm -o enumsites\" >}}";
            var expected = new string[] {
                "{{<", "kbd", "\"stsadm -o enumsites\" >}}" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithHugoShortcodeWithQuotesInsideQuotes()
        {
            var text = "\"{{< kbd \"stsadm -o enumsites\" >}}\"";
            var expected = new string[] {
                "\"{{<", "kbd", "\"stsadm -o enumsites\" >}}\"" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithHugoShortcodeWithNamedParameter()
        {
            var text = "{{< figure src='http://example.com/img.png' >}}";

            var expected = new string[] {
                "{{<", "figure", "src='http://example.com/img.png' >}}" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithHugoShortcodeWrappedInPunctuation()
        {
            var text = "\"({{< gist spf13 7896402 >}})\"";
            var expected = new string[] {
                "\"({{<", "gist", "spf13", "7896402 >}})\"" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithMultipleHugoShortcodes()
        {
            var text = "foo bar"
                + " {{< gist spf13 7896402 >}}"
                + " {{< instagram BWNjjyYFxVx hidecaption >}}";

            var expected = new string[] { "foo", "bar",
                "{{<", "gist", "spf13", "7896402 >}}",
                "{{<", "instagram", "BWNjjyYFxVx", "hidecaption >}}"};

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithMarkdownImageAndHugoShortcode()
        {
            var text = "foo bar"
                + " ![Example image](http://example.com/img.png)"
                + " {{< gist spf13 7896402 >}}";

            var expected = new string[] { "foo", "bar",
                "![Example image](http://example.com/img.png)",
                "{{<", "gist", "spf13", "7896402 >}}" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithHugoShortcodeInBlockquote()
        {
            var text = "foo bar {{< gist spf13 7896402 >}} foobar";

            // Due to the "prefix" added to each line in a blockquote ("> "), a
            // Hugo shortcode within a <blockquote> element must be processed as
            // a separate "chunk".
            var expected = new string[] { "foo", "bar",
                "{{< gist spf13 7896402 >}}", "foobar" };

            ITextFormatter formatter = CreateTestFormatter("<blockquote/>");

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        #endregion
    }
}
