/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

namespace IconCaptcha.Dto
{
    public class CaptchaResult
    {
        public long Id { get; set; }
        public int ?Error { get; set; }
        public double ?Data { get; set; }
    }
}
