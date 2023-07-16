/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using IconCaptcha.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IconCaptcha
{
    public static class RegistrationExtensions
    {
        public static IApplicationBuilder MapIconCaptcha(
            this IApplicationBuilder applicationBuilder,
            string pattern = "/iconcaptcha"
        )
        {
            return applicationBuilder.Map(pattern, _ => _.UseMiddleware<IconCaptchaMiddleware>());
        }
    
        public static IServiceCollection AddIconCaptcha(this IServiceCollection services, IConfigurationSection config = null)
        {
            return services.AddIconCaptcha<HttpContextSession>(config);
        }
    
        public static IServiceCollection AddIconCaptcha<TSessionProvider>(this IServiceCollection services, IConfigurationSection config = null) where TSessionProvider : class, ISessionProvider
        {
            // Add require services
            services.AddHttpContextAccessor();
            services.AddSession();

            // Services and middleware
            services.AddScoped<IconCaptchaMiddleware>();
            services.AddScoped<IconCaptchaService>();
            services.AddScoped<ISessionProvider, TSessionProvider>();
            
            if (config != null)
            {
                services.Configure<IconCaptchaOptions>(config);
            }

            return services;
        }
    }
}
