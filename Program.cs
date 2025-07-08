using Microsoft.Extensions.Options;
using Temperance.Agora.Services.Implementations;
using Temperance.Agora.Services.Interfaces;
using Temperance.Agora.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AlpacaConfigSettings>(builder.Configuration.GetSection("Alpaca"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<AlpacaConfigSettings>>().Value);

builder.Services.AddHttpClient();

builder.Services.AddHttpClient("AlpacaClient", (serviceProvider, client) =>
{
    var alpacaConfig = serviceProvider.GetRequiredService<IOptions<AlpacaConfigSettings>>().Value;
    // Log the values being used for HttpClient configuration
    Console.WriteLine($"AlpacaClient HttpClient: ApiKeyId={alpacaConfig.PaperApiKeyId}, ApiSecretKey={alpacaConfig.PaperApiSecretKey}, PaperBaseUrl={alpacaConfig.PaperBaseUrl}");

    if (string.IsNullOrEmpty(alpacaConfig.PaperBaseUrl))
        throw new InvalidOperationException("Alpaca API base URL must be provided.");

    client.BaseAddress = new Uri(alpacaConfig.PaperBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("APCA-API-KEY-ID", "PK87W17KTFVBOHJJ6WPZ");
    client.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", "nKTnIdP7Iseg6eRoZfWaSd5Ze5tF006J3O2yPgUT");
});


builder.Services.AddHttpClient("AlpacaSandboxClient", (serviceProvider, client) =>
{
    var alpacaConfig = serviceProvider.GetRequiredService<IOptions<AlpacaConfigSettings>>().Value;
    Console.WriteLine($"AlpacaSandboxClient HttpClient: ApiKeyId={alpacaConfig.PaperApiKeyId}, ApiSecretKey={alpacaConfig.PaperApiSecretKey}, SandboxBaseUrl={alpacaConfig.SandboxBaseUrl}");
    if (string.IsNullOrEmpty(alpacaConfig.SandboxBaseUrl))
        throw new InvalidOperationException("Alpaca API base URL must be provided.");
    client.BaseAddress = new Uri(alpacaConfig.SandboxBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("APCA-API-KEY-ID", "PK87W17KTFVBOHJJ6WPZ");
    client.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", "nKTnIdP7Iseg6eRoZfWaSd5Ze5tF006J3O2yPgUT");
});

builder.Services.AddScoped<IAlpacaTradingService, AlpacaTradingService>();

builder.Services.AddGrpc();
builder.Services.AddSingleton<AlpacaLivePriceService>();
builder.Services.AddScoped<LivePriceGrpcService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<LivePriceGrpcService>();

    endpoints.MapControllers();
});

app.Run();
