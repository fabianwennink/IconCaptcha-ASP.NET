/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace IconCaptcha_ASP
{
    public class IconCaptchaException : Exception
    {
        public IconCaptchaException()
        {
        }

        public IconCaptchaException(string message)
            : base(message)
        {
        }

        public IconCaptchaException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}