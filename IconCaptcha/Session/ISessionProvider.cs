/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

namespace IconCaptcha.Session
{
    public interface ISessionProvider
    {
        void SetSession(string key, CaptchaSession value);
        bool TryGetSession(string key, out CaptchaSession session);
    }
}
