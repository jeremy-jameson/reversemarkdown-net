namespace ReverseMarkdown
{
    /// <summary>
    /// Defines methods for mapping between class attribute language names (e.g.
    /// <c>"vbnet"</c> in <c>&lt;pre class="language-vbnet"&gt;</c>) and the
    /// preferred language names for fenced code blocks in Markdown (e.g.
    /// <c>"Visual Basic .NET"</c> in </c>"```Visual Basic .NET"</c>).
    /// </summary>
    public interface ICodeBlockLanguageMapper
    {
        /// <summary>
        /// Returns the class attribute language name (e.g. "vbnet") for the
        /// specified Markdown language name ("Visual Basic .NET"). Note that
        /// the "Visual Basic .NET" to "vbnet" mapping is only an example --
        /// this mapping does not exist by default.
        /// </summary>
        /// <param name="markdownLanguage">The Markdown language name.</param>
        /// <returns>The class attribute language name.</returns>
        string GetClassAttributeLanguage(string markdownLanguage);

        /// <summary>
        /// Returns the Markdown language name (e.g. "Visual Basic .NET") for
        /// the specified class attribute language name ("vbnet"). Note that
        /// the "vbnet" to "Visual Basic .NET" mapping is only an example --
        /// this mapping does not exist by default.
        /// </summary>
        /// <param name="classAttributeLanguage">The class attribute language
        /// name.</param>
        /// <returns>The Markdown language name.</returns>
        string GetMarkdownLanguage(string classAttributeLanguage);
    }
}
