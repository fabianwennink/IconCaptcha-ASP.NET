/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace IconCaptcha.Exceptions
{
    public class IconCaptchaException : Exception
    {
        public int? Code { get; }

        public IconCaptchaException(string message, int? code = null) : base(message)
        {
            Code = code;
        }
    }
}
