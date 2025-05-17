namespace Temperance.Agora.Settings
{
    public interface IAlpacaConfigSettings
    {
        string PaperApiKeyId { get; set; }

        string PaperApiSecretKey { get; set; }

        string PaperBaseUrl { get; set; }
    }
}
