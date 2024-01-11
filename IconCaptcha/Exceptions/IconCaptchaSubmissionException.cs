/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;

namespace IconCaptcha.Exceptions
{
    public class IconCaptchaSubmissionException : Exception
    {
        public int Code { get; set; }
        
        public IconCaptchaSubmissionException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
