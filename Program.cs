using Alpaca.Markets;
using Microsoft.Extensions.Options;
using TradingBot.Agora.Services.Implementations;
using TradingBot.Agora.Services.Interfaces;
using TradingBot.Agora.Settings;
using Environments = Alpaca.Markets.Environments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AlpacaConfigSettings>(builder.Configuration.GetSection("Alpaca"));

builder.Services.AddHttpClient();

builder.Services.AddHttpClient("AlpacaClient", (serviceProvider, client) =>
{
    var alpacaConfig = serviceProvider.GetRequiredService<IOptions<AlpacaConfigSettings>>().Value;

    if (string.IsNullOrEmpty(alpacaConfig.PaperBaseUrl))
        throw new InvalidOperationException("Alpaca API base URL must be provided.");

    client.BaseAddress = new Uri(alpacaConfig.PaperBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

//builder.Services.AddSingleton<IAlpacaTradingClient>(sp =>
//{
//    var alpacaConfig = sp.GetRequiredService<IOptions<AlpacaConfigSettings>>().Value;

//    if (string.IsNullOrEmpty(alpacaConfig.PaperApiKeyId) || string.IsNullOrEmpty(alpacaConfig.PaperApiSecretKey))
//    {
//        throw new InvalidOperationException("Alpaca API key and secret must be provided.");
//    }
//    var secretKey = new SecretKey(alpacaConfig.PaperApiKeyId, alpacaConfig.PaperApiSecretKey);

//    return Environments.Paper.GetAlpacaTradingClient(secretKey);
//});

builder.Services.AddScoped<IAlpacaTradingService, AlpacaTradingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
