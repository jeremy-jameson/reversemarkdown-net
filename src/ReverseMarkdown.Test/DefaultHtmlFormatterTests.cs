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
    }
}
