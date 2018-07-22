/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace IconCaptcha_ASP
{
    public class IconCaptchaResult
    {
        public IconCaptchaState CaptchaState { get; set; }
        public Object CaptchaResult { get; set; }

        public IconCaptchaResult(IconCaptchaState state, Object result)
        {
            CaptchaState = state;
            CaptchaResult = result;
        }
    }
}