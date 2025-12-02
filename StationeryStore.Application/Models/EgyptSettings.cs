namespace StationeryStore.Application.Models;

public class EgyptSettings
{
    public string CountryCode { get; set; } = "EG";
    public string DefaultLanguage { get; set; } = "ar";
    public string DefaultCurrency { get; set; } = "EGP";
    public decimal DefaultVatRate { get; set; } = 14.0m;
    public string TimeZone { get; set; } = "Egypt Standard Time";
    public int DecimalPlaces { get; set; } = 3;
    public string[] SupportedLanguages { get; set; } = { "ar", "en" };
}