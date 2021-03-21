using System;
using System.Linq;

namespace ReverseMarkdown
{
    public class Config
    {
        public UnknownTagsOption UnknownTags { get; set; } = UnknownTagsOption.PassThrough;

        public bool GithubFlavored { get; set; } = false;

        public bool RemoveComments { get; set; } = false;

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
        /// Option to set a different bullet character for un-ordered lists
        /// </summary>
        public char ListBulletChar { get; set; } = '-';

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
