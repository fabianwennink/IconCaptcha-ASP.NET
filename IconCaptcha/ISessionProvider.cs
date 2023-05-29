/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace IconCaptcha
{
    public interface ISessionProvider
    {
        void SetSession(string key, CaptchaSession value);
        bool TryGetSession(string key, out CaptchaSession session);
    }
}
