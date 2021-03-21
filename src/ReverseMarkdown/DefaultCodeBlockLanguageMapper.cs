using System;
using System.Collections.Generic;

namespace ReverseMarkdown
{
    /// <summary>
    /// Default implementation for the <see cref="ICodeBlockLanguageMapper"/>
    /// interface.
    /// </summary>
    public class DefaultCodeBlockLanguageMapper : ICodeBlockLanguageMapper
    {
        private readonly Dictionary<string, string> _classAttributeLanguages =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> _markdownLanguages =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DefaultCodeBlockLanguageMapper"/> class, optionally
        /// specifying whether to allow unmapped languages
        /// (<see cref="AllowUnmappedLanguages"/>). By default, "unmapped"
        /// languages are allowed.
        /// </summary>
        /// <param name="allowUnmappedLanguages"><c>true</c> -- the default --
        /// to pass through "unmapped" languages; <c>false</c> to throw an
        /// exception when no mapping exists for the specified language.</param>
        public DefaultCodeBlockLanguageMapper(
            bool allowUnmappedLanguages = true)
        {
            AllowUnmappedLanguages = allowUnmappedLanguages;
        }

        /// <summary>
        /// Specifies whether to pass through "unmapped" languages. If this is
        /// <c>false</c>, an exception will be thrown when no mapping exists for
        /// the specified language.
        /// </summary>
        public virtual bool AllowUnmappedLanguages { get; private set; }

        /// <summary>
        /// Adds a mapping between the specified Markdown language name and the
        /// class attribute language name.
        /// </summary>
        /// <param name="markdownLanguage">The Markdown language name.</param>
        /// <param name="classAttributeLanguage">The class attribute language
        /// name.</param>
        public virtual void AddMapping(
            string markdownLanguage,
            string classAttributeLanguage)
        {
            _classAttributeLanguages.Add(classAttributeLanguage, markdownLanguage);
            _markdownLanguages.Add(markdownLanguage, classAttributeLanguage);
        }

        /// <summary>
        /// Returns the class attribute language name (e.g. "vbnet") for the
        /// specified Markdown language name ("Visual Basic .NET"). Note that
        /// the "Visual Basic .NET" to "vbnet" mapping is only an example --
        /// this mapping does not exist by default.
        /// </summary>
        /// <param name="markdownLanguage">The Markdown language name.</param>
        /// <returns>The class attribute language name.</returns>
        public virtual string GetClassAttributeLanguage(string markdownLanguage)
        {
            string classAttributeLanguage = null;

            bool success = _markdownLanguages.TryGetValue(
                markdownLanguage,
                out classAttributeLanguage);

            if (success == false
                && AllowUnmappedLanguages == true)
            {
                classAttributeLanguage = markdownLanguage;
            }
            else if (success == false
                && AllowUnmappedLanguages == false)
            {
                throw new InvalidOperationException(
                    $"No mapping found for language ({markdownLanguage}).");
            }

            return classAttributeLanguage;
        }

        /// <summary>
        /// Returns the Markdown language name (e.g. "Visual Basic .NET") for
        /// the specified class attribute language name ("vbnet"). Note that
        /// the "vbnet" to "Visual Basic .NET" mapping is only an example --
        /// this mapping does not exist by default.
        /// </summary>
        /// <param name="classAttributeLanguage">The class attribute language
        /// name.</param>
        /// <returns>The Markdown language name.</returns>
        public virtual string GetMarkdownLanguage(string classAttributeLanguage)
        {
            string markdownLanguage = null;

            bool success = _classAttributeLanguages.TryGetValue(
                classAttributeLanguage,
                out markdownLanguage);

            if (success == false
                && AllowUnmappedLanguages == true)
            {
                markdownLanguage = classAttributeLanguage;
            }
            else if (success == false
                && AllowUnmappedLanguages == false)
            {
                throw new InvalidOperationException(
                    $"No mapping found for language ({classAttributeLanguage}).");
            }

            return markdownLanguage;
        }
    }
}
