using HtmlAgilityPack;
using System;
using Xunit;

namespace ReverseMarkdown.Test
{
    public class DefaultHtmlFormatterTests
    {
        #region NormalizeWhitespace

        [Fact]
        public void NormalizeWhitespace_ThrowsWhenMarkdownIsNull()
        {
            HtmlNode node = null;
            var expectedExceptionText = "Value cannot be null.";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                formatter.NormalizeWhitespace(node));

            Assert.Equal("node", ex.ParamName);
            Assert.StartsWith(expectedExceptionText, ex.Message);
        }

        [Fact]
        public void NormalizeWhitespace_WithEmbeddedWhitespace()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<b>some\n\tbold text</b>");

            var expected = "<b>some bold text</b>";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.NormalizeWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region RemoveInsignificantWhitespace

        [Fact]
        public void RemoveInsignificantWhitespace_ThrowsWhenNodeIsNull()
        {
            HtmlNode node = null;
            var expectedExceptionText = "Value cannot be null.";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                formatter.RemoveInsignificantWhitespace(node));

            Assert.Equal("node", ex.ParamName);
            Assert.StartsWith(expectedExceptionText, ex.Message);
        }

        [Fact]
        public void RemoveInsignificantWhitespace_WithEmbeddedWhitespace()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<p>This is  \t \n a paragraph.</p>");

            var expected = "<p>This is a paragraph.</p>";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.RemoveInsignificantWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveInsignificantWhitespace_WithLeadingWhitespace()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<p>  \t \n This is a paragraph.</p>");

            var expected = "<p>This is a paragraph.</p>";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.RemoveInsignificantWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveInsignificantWhitespace_WithTrailingWhitespace()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<p>This is a paragraph.  \t \n </p>");

            var expected = "<p>This is a paragraph.</p>";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.RemoveInsignificantWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveInsignificantWhitespace_WithListItem()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<li>   This is a list item"
                + "      <p>This is a paragraph.  \t \n </p>"
                + Environment.NewLine
                + "   This is some text after the paragraph.     </li>");

            var expected = "<li>This is a list item"
                + "<p>This is a paragraph.</p>"
                + "This is some text after the paragraph.</li>";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.RemoveInsignificantWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveInsignificantWhitespace_WithInlineContent()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("     <b>   some    bold  text </b>"
                + "      <i>   some italic text   </i>"
                + "   some   plain    text        ");

            var expected = " <b> some bold text </b>"
                + " <i> some italic text </i>"
                + " some plain text ";

            IHtmlFormatter formatter = new DefaultHtmlFormatter();

            formatter.RemoveInsignificantWhitespace(doc.DocumentNode);
            var actual = doc.DocumentNode.OuterHtml;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
