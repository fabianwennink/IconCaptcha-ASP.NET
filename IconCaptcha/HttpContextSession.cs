using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace IconCaptcha
{
    /// <summary>
    /// Acts as a wrapper for the session handler. Put it somewhere in a class if 
    /// you want to use the captcha in multiple controllers.
    /// </summary>
    public class HttpContextSession : ISessionProvider
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public HttpContextSession(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public bool TryGetSession(string key, out CaptchaSession session)
        {
            if (!HttpContextAccessor.HttpContext.Session.TryGetValue(key, out var value))
            {
                session = null;
                return false;
            }

            var sessionString = Encoding.UTF8.GetString(value);

            session = JsonSerializer.Deserialize<CaptchaSession>(sessionString);

            return true;
        }

        public void SetSession(string key, CaptchaSession value)
        {
            HttpContextAccessor.HttpContext.Session.SetString(key, JsonSerializer.Serialize(value));
        }
    }
}