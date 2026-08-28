using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

await builder.Build().RunAsync();
