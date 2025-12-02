using System.Collections.Generic;

namespace StationeryStore.Application.Interfaces;

public interface ILocalizationService
{
    string Localize(string key, string language);
    IDictionary<string, string> GetTranslations(string language);
}