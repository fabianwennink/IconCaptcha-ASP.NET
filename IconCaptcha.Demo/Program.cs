using IconCaptcha;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IconCaptcha.IconCaptcha>();
builder.Services.AddScoped<ISessionProvider, HttpContextSession>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.Configure<Options>(options => options.IconPath = "assets/icons");

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapIconCaptcha();

app.Run();
