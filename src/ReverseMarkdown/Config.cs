using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseMarkdown
{
    public class Config
    {
        public UnknownTagsOption UnknownTags { get; set; } = UnknownTagsOption.PassThrough;

        public bool GithubFlavored { get; set; } = false;

        /// <summary>
        /// Option to specify Markdown text for converting horizontal rule
        /// <c>&lt;hr&gt;</c> elements. Defaults to <c>"* * *"</c>.
        /// </summary>
        public string HorizontalRuleString { get; set; } = "* * *";

        public bool RemoveComments { get; set; } = false;

        /// <summary>
        /// If <c>true</c>, excess spaces at the start of each line in code
        /// blocks are removed from the Markdown. Defaults to <c>false</c>.
        /// </summary>
        public bool RemoveExcessIndentationFromCode { get; set; } = false;

        /// <summary>
        /// If <c>true</c>, trailing whitespace in code blocks is removed from
        /// the Markdown. Defaults to <c>false</c>.
        /// </summary>
        public bool RemoveTrailingWhitespaceFromCode { get; set; } = false;

        /// <summary>
        /// If <c>true</c>, multiple consecutive blank lines are removed from
        /// the Markdown. Defaults to <c>true</c>.
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
        public bool RemoveMultipleConsecutiveBlankLines { get; set; } = true;

        /// <summary>
        /// Specify which schemes (without trailing colon) are to be allowed for &lt;a&gt; and &lt;img&gt; tags. Others will be bypassed. By default allows everything.
        /// <para>If <see cref="string.Empty" /> provided and when href schema couldn't be determined - whitelists</para>
        /// </summary>
        public string[] WhitelistUriSchemes { get; set; }

        /// <summary>
        /// How to handle &lt;a&gt; tag href attribute
        /// <para>false - Outputs [{name}]({href}{title}) even if name and href is identical. This is the default option.</para>
        /// true - If name and href equals, outputs just the `name`. Note that if Uri is not well formed as per <see cref="Uri.IsWellFormedUriString"/> (i.e string is not correctly escaped like `http://example.com/path/file name.docx`) then markdown syntax will be used anyway.
        /// <para>If href contains http/https protocol, and name doesn't but otherwise are the same, output href only</para>
        /// If tel: or mailto: scheme, but afterwards identical with name, output name only.
        /// </summary>
        public bool SmartHrefHandling { get; set; } = false;

        public TableWithoutHeaderRowHandlingOption TableWithoutHeaderRowHandling { get; set; } =
            TableWithoutHeaderRowHandlingOption.Default;

        /// <summary>
        /// Option to set a different emphasis character. Defaults to <c>'*'</c>.
        /// </summary>
        public char EmphasisChar { get; set; } = '*';

        /// <summary>
        /// Option to set a different bullet character for un-ordered lists
        /// </summary>
        public char ListBulletChar { get; set; } = '-';

        /// <summary>
        /// Option to specify numbering style for ordered lists. Defaults to
        /// <see cref="ListNumberingStyle.Increment"/>.
        /// </summary>
        /// <remarks>
        /// Set this to <see cref="ListNumberingStyle.AlwaysOne"/> to always
        /// prefix numbered list items with <c>"1. "</c>. This avoids having to
        /// renumber list items when subsequently inserting new items into the
        /// list (and also simplifies version history when making changes to
        /// Markdown lists over time).
        /// </remarks>
        public ListNumberingStyle ListNumberingStyle { get; set; } =
            ListNumberingStyle.Increment;

        /// <summary>
        /// Option to set a default GFM code block language if class based language markers are not available
        /// </summary>
        public string DefaultCodeBlockLanguage { get; set; }

        /// <summary>
        /// Specifies the <see cref="ICodeBlockLanguageMapper"> to use for
        /// mapping class attribute language names (e.g. <c>"vbnet"</c> in
        /// <c>&lt;pre class="language-vbnet"&gt;</c>) to the preferred language
        /// names for fenced code blocks in Markdown (e.g.
        /// <c>"Visual Basic .NET"</c> in </c>"```Visual Basic .NET"</c>). By
        /// default, no mapping is performed.
        /// </summary>
        public ICodeBlockLanguageMapper CodeBlockLanguageMapper { get; set; }

        /// <summary>
        /// Option to pass a list of tags to pass through as is without any processing
        /// </summary>
        public string[] PassThroughTags { get; set; } = { };

        public enum UnknownTagsOption
        {
            /// <summary>
            /// Include the unknown tag completely into the result. That is, the tag along with the text will be left in output.
            /// </summary>
            PassThrough,
            /// <summary>
            ///  Drop the unknown tag and its content
            /// </summary>
            Drop,
            /// <summary>
            /// Ignore the unknown tag but try to convert its content
            /// </summary>
            Bypass,
            /// <summary>
            /// Raise an error to let you know
            /// </summary>
            Raise
        }

        public enum TableWithoutHeaderRowHandlingOption
        {
            /// <summary>
            /// By default, first row will be used as header row
            /// </summary>
            Default,
            /// <summary>
            /// An empty row will be added as the header row
            /// </summary>
            EmptyRow
        }

        /// <summary>
        /// Specifies the sequence of regular expression pattern replacements to
        /// apply to <c>#text</c> nodes in the HTML source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Replacements are performed in the order in which they are added to
        /// the collection -- in other words, the order of the items is
        /// important (if subsequent replacements match the result of a previous
        /// replacement).
        /// </para>
        /// <para>
        /// The default items include patterns for replacing Markdown special
        /// characters. For example, asterisks (<c>'*'</c>) in the source HTML
        /// are escaped when converting to Markdown by inserting a backslash
        /// before the asterisk (<c>@"\*"</c>).
        /// </para>
        /// <para>
        /// Note that special characters in regular expressions must be escaped
        /// in the "pattern" (i.e. the dictionary key). For example, <c>'+'</c>
        /// and <c>'*'</c> have special meaning in regular expressions and are
        /// therefore escaped in patterns using a backslash (<c>'\'</c>).
        /// Similarly, a backslash must also be escaped in patterns
        /// (<c>"\\"</c>) -- but it is not escaped when specifying the
        /// replacement. For example, a replacement of <c>@"\$1"</c> simply
        /// inserts a backslash before the group matched by the pattern -- where
        /// the "group" is specified using parentheses.
        /// </para>
        /// </remarks>
        public List<TextReplacementPattern> TextReplacementPatterns =
            new List<TextReplacementPattern>()
        {
            // Escape '+' and '-' at beginning of line (to avoid mistaking plain
            // text for a list)
            new TextReplacementPattern(@"^(\+ )", @"\$1"),
            new TextReplacementPattern("^(- )", @"\$1"),

            // Escape '_' that is *not* followed by a word character
            // Note: "[^\w]" is equivalent to [^a-zA-Z0-9_]
            new TextReplacementPattern(@"(_[^\w])", @"\$1"),

            // Escape '_' after a space
            new TextReplacementPattern(@" _", @" \_"),

            // Escape '_' at beginning of line
            new TextReplacementPattern(@"(^_)", @"\$1"),

            // Escape double underscores
            new TextReplacementPattern(@"__", @"\_\_"),

            // Important: Escape double backslashes *before* escaping other
            // items
            new TextReplacementPattern(@"(\\\\)", @"\\$1"), // replace two backslashes with four (\\\\)

            // Escape '\' that is followed by specific characters
            new TextReplacementPattern(@"(\\\$)", @"\$1"), // escape '\' followed by '$'
            new TextReplacementPattern(@"(\\\%)", @"\$1"), // escape '\' followed by '%'
            new TextReplacementPattern(@"(\\\&)", @"\$1"), // escape '\' followed by '&'
            new TextReplacementPattern(@"(\\\.)", @"\$1"), // escape '\' followed by '.'
            new TextReplacementPattern(@"(\\\[)", @"\$1"), // escape '\' followed by '['
            new TextReplacementPattern(@"(\\\{)", @"\$1"), // escape '\' followed by '{'
            
            // Escape all asterisks
            new TextReplacementPattern(@"(\*)", @"\$1")
        };

        /// <summary>
        /// Determines whether url is allowed: WhitelistUriSchemes contains no elements or contains passed url.
        /// </summary>
        /// <param name="scheme">Scheme name without trailing colon</param>
        internal bool IsSchemeWhitelisted(string scheme) {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            var isSchemeAllowed = WhitelistUriSchemes == null || WhitelistUriSchemes.Length == 0 || WhitelistUriSchemes.Contains(scheme, StringComparer.OrdinalIgnoreCase);
            return isSchemeAllowed;
        }
    }
}
