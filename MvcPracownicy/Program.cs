using MvcPracownicy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MovieContext") ?? throw new InvalidOperationException("Connection string 'MovieContext' not found.")));
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MovieContext>();
    Db.Init(context);
}

app.Use(async (ctx, next) =>
{
    await next();

    if(ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted && !ctx.Request.Path.StartsWithSegments("/api"))
    {
        string originalPath = ctx.Request.Path.Value;
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = "/Logowanie";
        await next();
    }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "Logowanie",
    defaults: new { controller = "Account", action = "Logowanie" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.MapRazorPages();

app.Run();



