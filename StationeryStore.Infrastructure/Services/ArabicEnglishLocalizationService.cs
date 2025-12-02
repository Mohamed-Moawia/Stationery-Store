using System.Collections.Generic;
using StationeryStore.Application.Interfaces;

namespace StationeryStore.Infrastructure.Services;

public class ArabicEnglishLocalizationService : ILocalizationService
{
    private static readonly Dictionary<string, Dictionary<string, string>> _translations =
        new()
        {
            ["ar"] = new Dictionary<string, string>
            {
                ["Hello"] = "مرحبا",
                ["Goodbye"] = "مع السلامة"
            },
            ["en"] = new Dictionary<string, string>
            {
                ["Hello"] = "Hello",
                ["Goodbye"] = "Goodbye"
            }
        };

    public string Localize(string key, string language)
    {
        if (string.IsNullOrWhiteSpace(language) || !_translations.ContainsKey(language))
            language = "en";
        var dict = _translations[language];
        return dict.TryGetValue(key, out var val) ? val : key;
    }

    public IDictionary<string, string> GetTranslations(string language)
    {
        language ??= "en";
        if (_translations.TryGetValue(language, out var dict))
            return dict;
        return _translations["en"];
    }
}