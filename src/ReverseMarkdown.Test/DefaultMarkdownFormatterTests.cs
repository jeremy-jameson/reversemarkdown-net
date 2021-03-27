using HtmlAgilityPack;
using System;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace ReverseMarkdown.Test
{
    public class DefaultMarkdownFormatterTests
    {

        private IMarkdownFormatter CreateTestFormatter(
            HtmlNode referenceNode = null)
        {
            if (referenceNode == null)
            {
                referenceNode = CreateTestHtmlNode();
            }

            return new DefaultMarkdownFormatter(referenceNode, new Config());
        }

        private HtmlNode CreateTestHtmlNode(
            string html = "<body/>",
            string xpath = "/*[1]")
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var node = doc.DocumentNode.SelectSingleNode(xpath);

            return node;
        }

        #region GetFormattingRules

        [Fact]
        public void GetFormattingRules_ForBlockquote()
        {
            var node = CreateTestHtmlNode("<blockquote/>");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = true,
                WrapLineLength = 78 // 80 - "> ".Length
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForDiv()
        {
            var node = CreateTestHtmlNode("<div/>");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = 80
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForNestedDivWithHugoShortcodeInBlockquote()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "inner <div>" (because wrapping Hugo shortcodes
            // in a <blockquote> can lead to corrupt Markdown -- due to the "> "
            // prefix used for blockquotes)

            var node = CreateTestHtmlNode(
                "<blockquote>"
                    + "<div>{{< gist spf13 7896402 >}}</div>"
                + "</blockquote>",
                "/blockquote/div");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForOuterDivWithHugoShortcodeInBlockquote()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "outer <div>" because a Hugo shortcode within a
            // <blockquote> must be parsed as a single "chunk" (in other words,
            // it must be parsed exactly once -- in the context of the "inner
            // <blockquote>")

            var node = CreateTestHtmlNode(
                "<div>"
                    + "<blockquote>{{< gist spf13 7896402 >}}</blockquote>"
                + "</div>",
                "/div");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForDivWithNestedPre()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "outer <div>" because it contains preformatted
            // text

            var node = CreateTestHtmlNode(
                "<div><blockquote><pre/></blockquote></div>",
                "/div");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForDivWithNestedTable()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "outer <div>" because it contains a table (i.e.
            // pipe-delimited content in Markdown)

            var node = CreateTestHtmlNode(
                "<div><blockquote><table/></blockquote></div>",
                "/div");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForDivInsidePreformattedText()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "inner <div>" because it is inside preformattted
            // content

            var node = CreateTestHtmlNode("<pre><div/></pre>", "/pre/div");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForListItem()
        {
            var node = CreateTestHtmlNode("<li/>");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = true,
                WrapLineLength = 78 // 80 - "- ".Length
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForParagraph()
        {
            var node = CreateTestHtmlNode("<p/>");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = true,
                WrapLineLength = 80
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForNestedParagraphWithHugoShortcodeInBlockquote()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "inner <p>" (because wrapping Hugo shortcodes
            // in a <blockquote> can lead to corrupt Markdown -- due to the "> "
            // prefix used for blockquotes)

            var node = CreateTestHtmlNode(
                "<blockquote>"
                    + "<p>{{< gist spf13 7896402 >}}</p>"
                + "</blockquote>",
                "/blockquote/p");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForNestedParagraphWithHugoShortcodeInDiv()
        {
            // For content similar to the following, no line wrapping should be
            // performed on the "inner <p>" (because it is more efficient to wrap
            // the content when formatting the Markdown for the "outer <div>")

            var node = CreateTestHtmlNode(
                "<div>"
                    + "<p>{{< gist spf13 7896402 >}}</p>"
                + "</div>",
                "/div/p");

            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetFormattingRules_ForPreformattedText()
        {
            var node = CreateTestHtmlNode("<pre/>");
            var expected = new MarkdownFormattingRules()
            {
                CanTrim = false,
                WrapLineLength = int.MaxValue
            };

            IMarkdownFormatter formatter = CreateTestFormatter(node);

            var actual = formatter.GetFormattingRules();

            actual.Should().BeEquivalentTo(expected);
        }

        #endregion

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
        public void ParseChunks_WithIndentedInlineImage()
        {
            // Image within a list item that has already been wrapped will be
            // indented by the length of the list item prefix (e.g. "1. ")
            var text = "   ![Example image](http://example.com/img.png)";

            var expected = new string[] { text };

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
        public void ParseChunks_WithInlineCode()
        {
            var text = "use `stsadm.exe -o execadmsvcjobs` to wait";
            var expected = new string[] {
                "use", "`stsadm.exe -o execadmsvcjobs`", "to", "wait" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithTwoInlineCodeSegments()
        {
            var text = "First: `var i = 1;`, second: `if (i == 1)`";

            var expected = new string[] {
                "First:", "`var i = 1;`,", "second:", "`if (i == 1)`" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithInlineCodeInQuotesAndParenthenses()
        {
            var text = "foo (\"`var i = 1;`\") bar";

            var expected = new string[] {
                "foo", "(\"`var i = 1;`\")", "bar" };

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithIndentedInlineCode()
        {
            // Inline code within a list item that has already been wrapped will
            // be indented by the length of the list item prefix (e.g. "1. ")
            var text = "   `stsadm.exe -o execadmsvcjobs`";

            var expected = new string[] { text };

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

        #region WrapText

        [Fact]
        public void WrapText_WithIndentedTextAndExactWrapLineLength_DoesNotWrap()
        {
            var text =
"    Lorem ipsum dolor sit amet, consectetur adipiscing elit." + Environment.NewLine;

            var wrapLineLength = text.TrimEnd().Length;

            var expected = text;

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.WrapText(text, wrapLineLength);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region WrapTextLine

        [Fact]
        public void WrapTextLine_WithIndentedTextAndExactWrapLineLength_DoesNotWrap()
        {
            var text =
"    Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            var wrapLineLength = text.Length;

            var expected = text;

            ITextFormatter formatter = CreateTestFormatter();

            var actual = formatter.WrapTextLine(text, wrapLineLength);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
