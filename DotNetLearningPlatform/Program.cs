using DotNetLearningPlatform.Client.Pages;
using DotNetLearningPlatform.Components;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DotNetLearningPlatform.Client._Imports).Assembly);

app.Run();
