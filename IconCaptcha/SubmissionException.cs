/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace IconCaptcha
{
    public class SubmissionException : Exception
    {
        public int Code { get; set; }
        
        public SubmissionException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
