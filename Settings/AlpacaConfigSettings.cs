namespace Temperance.Agora.Settings
{
    public class AlpacaConfigSettings : IAlpacaConfigSettings
    {
        public string PaperApiKeyId { get; set; } = string.Empty;
        public string PaperApiSecretKey { get; set; } = string.Empty;
        public string PaperBaseUrl { get; set; } = string.Empty;
        public string PaperWebSocketUrl { get; set; } = string.Empty;
    }
}
