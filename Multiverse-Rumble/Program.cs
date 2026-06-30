using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// --- ESTO ES LO MÁS IMPORTANTE ---
// Asegura que todos los archivos en wwwroot sean accesibles
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// MapStaticAssets es nuevo y a veces falla con archivos que no detectó al compilar
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();