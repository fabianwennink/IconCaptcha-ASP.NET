/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;

namespace IconCaptcha.Exceptions
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
