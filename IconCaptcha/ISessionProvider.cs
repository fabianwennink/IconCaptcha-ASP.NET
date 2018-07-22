/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace IconCaptcha_ASP
{
    public interface ISessionProvider
    {
        void SetSession(string key, IconCaptchaSession value);
        IconCaptchaSession GetSession(string key);
    }
}
