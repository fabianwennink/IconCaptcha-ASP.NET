/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

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

            // IconCaptcha : Required, parameter optional
            builder.Services.AddIconCaptcha(builder.Configuration.GetSection("IconCaptcha"));
            
            // IconCaptcha : Optional, if you want to set configuration programmatically
            builder.Services.Configure<IconCaptchaOptions>(options =>
            {
                // options.IconPath = "assets/icons";
            });

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // IconCaptcha : Required 
            app.MapIconCaptcha();

            app.Run();
        }
    }
}
