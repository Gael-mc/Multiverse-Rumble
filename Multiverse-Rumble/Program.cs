using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Hosts como Render/Heroku/Railway asignan el puerto en runtime via la
// variable de entorno PORT y esperan que la app escuche ahi. En local esta
// variable no existe, asi que se respeta el puerto de launchSettings.json.
var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(renderPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
}

var app = builder.Build();

// Detras de un proxy (Render, etc.) la conexion real del navegador es HTTPS,
// pero el proxy le habla a la app por HTTP. Sin esto, UseHttpsRedirection()
// puede terminar en un bucle de redirecciones.
// KnownNetworks/KnownProxies se limpian porque la IP del proxy de Render no es
// fija ni se puede listar de antemano (a partir de .NET 8.0.17/9.0.6 el
// middleware ignora los headers de proxies "desconocidos" por defecto).
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
