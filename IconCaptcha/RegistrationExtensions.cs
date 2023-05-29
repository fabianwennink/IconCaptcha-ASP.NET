using Microsoft.AspNetCore.Builder;

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
    }
}
