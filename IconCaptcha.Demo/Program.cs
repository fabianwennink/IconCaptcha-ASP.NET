namespace IconCaptcha.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

            builder.Services.AddControllersWithViews();

            // IconCaptcha: Required, the configuration section can be renamed.
            // The configuration key must match the one set in the 'appsettings.json' file.
            builder.Services.AddIconCaptcha(builder.Configuration.GetSection("IconCaptcha"));

            // IconCaptcha: Optional, can be used to programmatically alter the configuration.
            builder.Services.Configure<IconCaptchaOptions>(options =>
            {
                // options.IconPath = "assets/icons";
            });

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

            // IconCaptcha: Required, the endpoint can be changed to your liking.
            app.MapIconCaptcha("/iconcaptcha");

            app.Run();
        }
    }
}
