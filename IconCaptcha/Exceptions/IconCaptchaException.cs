/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
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
