/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

namespace IconCaptcha.Session
{
    public interface ISessionProvider
    {
        /// <summary>
        /// Stores the given captcha session at the specified key in the browser session.
        /// </summary>
        /// <param name="key">The key at which the captcha session will be saved in the browser session.</param>
        /// <param name="value">The captcha session data to store.</param>
        void SetSession(string key, CaptchaSession value);
        
        /// <summary>
        /// Tries to retrieve the captcha session from the browser session at the specified key.
        /// </summary>
        /// <param name="key">The key at which the captcha session is saved in the browser session.</param>
        /// <param name="session">When this method returns, contains the captcha session data or null.</param>
        /// <returns>True if the session was successfully retrieved otherwise false.</returns>
        bool TryGetSession(string key, out CaptchaSession session);
    }
}
