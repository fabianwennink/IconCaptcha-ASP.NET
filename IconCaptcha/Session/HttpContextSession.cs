/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace IconCaptcha.Session
{
    public class HttpContextSession : ISessionProvider
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public HttpContextSession(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc cref="ISessionProvider.TryGetSession"/>
        public bool TryGetSession(string key, out CaptchaSession session)
        {
            if (!HttpContextAccessor.HttpContext.Session.TryGetValue(key, out var value))
            {
                session = null;
                return false;
            }

            // Deserialize the session data.
            var sessionString = Encoding.UTF8.GetString(value);
            session = JsonSerializer.Deserialize<CaptchaSession>(sessionString);

            return true;
        }

        /// <inheritdoc cref="ISessionProvider.SetSession"/>
        public void SetSession(string key, CaptchaSession value)
        {
            HttpContextAccessor.HttpContext.Session.SetString(key, JsonSerializer.Serialize(value));
        }
    }
}
