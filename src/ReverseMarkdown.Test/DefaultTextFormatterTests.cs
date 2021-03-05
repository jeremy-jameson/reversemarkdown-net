using System;
using System.Linq;
using Xunit;

namespace ReverseMarkdown.Test
{
    public class DefaultTextFormatterTests
    {
        #region ParseChunks

        [Fact]
        public void ParseChunks_ReturnsEmptyEnumerableWhenTextIsNull()
        {
            string text = null;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.True(actual.Any() == false);
        }

        [Fact]
        public void ParseChunks_WithEmptyText()
        {
            var text = string.Empty;
            var expected = new string[] { string.Empty };

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithTwoWords()
        {
            var text = "foo bar";
            var expected = new string[] { "foo", "bar" };

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public void ParseChunks_WithThreeWords()
        {
            var text = "foo bar foobar";
            var expected = new string[] { "foo", "bar", "foobar" };

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.ParseChunks(text);

            Assert.Equal<string>(expected, actual);
        }

        #endregion

        #region WrapText

        [Fact]
        public void WrapText_ReturnsNullWhenTextIsNull()
        {
            string text = null;
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapText_WithEmptyText()
        {
            var text = string.Empty;
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapText_WithTwoWordsAndVeryShortWrapLineLength()
        {
            var text = "foo bar";
            var expected = "foo" + Environment.NewLine + "bar";

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapText_WithSomeLengthyText()
        {
            var text =
"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam ornare purus eu"
+ " scelerisque lobortis. Integer dignissim, urna interdum auctor ultricies,"
+ " justo nibh imperdiet tellus, et porta ex eros sit amet sapien.";

            var expected =
"Lorem ipsum dolor sit amet, consectetur adipiscing" + Environment.NewLine
+ "elit. Nam ornare purus eu scelerisque lobortis." + Environment.NewLine
+ "Integer dignissim, urna interdum auctor ultricies," + Environment.NewLine
+ "justo nibh imperdiet tellus, et porta ex eros sit" + Environment.NewLine
+ "amet sapien.";

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text, 50);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapText_WithTabsAndLineBreaks_PreservesTabsAndLineBreaks()
        {
            var text = "foo\tbar" + Environment.NewLine + "foobar";
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapText_WithBlockquoteContainingTwoParagraphs_ShouldNotWrap()
        {
            var text =
@"**Note**


You can download these scripts from my Toolbox repository on GitHub:

[https://github.com/jeremy-jameson/Toolbox](https://github.com/jeremy-jameson/Toolbox)";

            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapText(text);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region WrapTextLine

        [Fact]
        public void WrapTextLine_ReturnsNullWhenTextIsNull()
        {
            string text = null;
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapTextLine(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapTextLine_WithEmptyText()
        {
            var text = string.Empty;
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapTextLine(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapTextLine_WithTwoWordsAndVeryShortWrapLineLength()
        {
            var text = "foo bar";
            var expected = "foo" + Environment.NewLine + "bar";

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapTextLine(text, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapTextLine_WithSomeLengthyText()
        {
            var text =
"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam ornare purus eu"
+ " scelerisque lobortis. Integer dignissim, urna interdum auctor ultricies,"
+ " justo nibh imperdiet tellus, et porta ex eros sit amet sapien.";

            var expected =
"Lorem ipsum dolor sit amet, consectetur adipiscing" + Environment.NewLine
+ "elit. Nam ornare purus eu scelerisque lobortis." + Environment.NewLine
+ "Integer dignissim, urna interdum auctor ultricies," + Environment.NewLine
+ "justo nibh imperdiet tellus, et porta ex eros sit" + Environment.NewLine
+ "amet sapien.";

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapTextLine(text, 50);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WrapTextLine_ThrowsWhenTextContainsLineFeed()
        {
            var text = "foo" + Environment.NewLine + "bar";

            var expectedExceptionText =
                "Cannot wrap text line because the text already contains a line"
                    + " feed.";

            ITextFormatter formatter = new DefaultTextFormatter();
            
            var ex = Assert.Throws<ArgumentException>(() =>
                formatter.WrapTextLine(text));

            Assert.Equal("text", ex.ParamName);
            Assert.StartsWith(expectedExceptionText, ex.Message);
        }

        [Fact]
        public void WrapTextLine_WithTabs_PreservesTabs()
        {
            var text = "foo\tbar \tfoobar";
            var expected = text;

            ITextFormatter formatter = new DefaultTextFormatter();

            var actual = formatter.WrapTextLine(text);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
