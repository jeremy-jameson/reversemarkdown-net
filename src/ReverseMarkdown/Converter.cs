using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HtmlAgilityPack;
using ReverseMarkdown.Converters;

namespace ReverseMarkdown
{
    public class Converter
    {
        private readonly IDictionary<string, IConverter> _converters = new Dictionary<string, IConverter>();
        private readonly IConverter _passThroughTagsConverter;
        private readonly IConverter _dropTagsConverter;
        private readonly IConverter _byPassTagsConverter;

        // Use the IMarkdownFormatter interface to allow
        // the formatter dependency to be specified using some other
        // mechanism in the future (e.g. dependency injection or perhaps
        // specifying a custom formatter via configuration).
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IMarkdownFormatter _markdownFormatter;

        public Converter() : this(new Config()) {}

        public Converter(Config config)
        {
            Config = config;

            // instantiate all converters excluding the unknown tags converters
            foreach (var ctype in typeof(IConverter).GetTypeInfo().Assembly.GetTypes()
                .Where(t => t.GetTypeInfo().GetInterfaces().Contains(typeof(IConverter)) && 
                !t.GetTypeInfo().IsAbstract
                && t != typeof(PassThrough)
                && t != typeof(Drop)
                && t != typeof(ByPass)))
            {
                Activator.CreateInstance(ctype, this);
            }

            // register the unknown tags converters
            _passThroughTagsConverter = new PassThrough(this);
            _dropTagsConverter = new Drop(this);
            _byPassTagsConverter = new ByPass(this);

            // TODO: Should the formatters be configurable?
            // Consider adding a Config property that callers could use to
            // override with their own behavior (e.g. to adjust whitespace
            // within inline elements -- "<b> foo </b>" --> " <b>foo</b> ").
            _htmlFormatter = new DefaultHtmlFormatter();
            _markdownFormatter = new DefaultMarkdownFormatter();
        }

        public Config Config { get; }

        public string Convert(string html)
        {
            html = Cleaner.PreTidy(html, Config.RemoveComments);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            Cleaner.Tidy(doc);

            var root = doc.DocumentNode;

            // ensure to start from body and ignore head etc
            if (root.Descendants("body").Any())
            {
                root = root.SelectSingleNode("//body");
            }

            _htmlFormatter.NormalizeWhitespace(root);

            var result = Lookup(root.Name).Convert(root);

            if (Config.RemoveMultipleConsecutiveBlankLines == true)
            {
                result = _markdownFormatter
                    .RemoveMultipleConsecutiveBlankLines(result);
            }

            return result;
        }

        public void Register(string tagName, IConverter converter)
        {
            _converters[tagName] = converter;
        }

        public IConverter Lookup(string tagName)
        {
            // if a tag is in the pass through list then use the pass through tags converter
            if (Config.PassThroughTags.Contains(tagName))
            {
                return _passThroughTagsConverter;
            }
            
            return _converters.ContainsKey(tagName) ? _converters[tagName] : GetDefaultConverter(tagName);
        }

        private IConverter GetDefaultConverter(string tagName)
        {
            switch (Config.UnknownTags)
            {
                case Config.UnknownTagsOption.PassThrough:
                    return _passThroughTagsConverter;
                case Config.UnknownTagsOption.Drop:
                    return _dropTagsConverter;
                case Config.UnknownTagsOption.Bypass:
                    return _byPassTagsConverter;
                default:
                    throw new UnknownTagException(tagName);
            }
        }
    }
}
