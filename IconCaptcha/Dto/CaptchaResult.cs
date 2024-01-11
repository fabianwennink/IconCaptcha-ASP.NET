/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

#nullable enable
namespace IconCaptcha.Dto
{
    public class CaptchaResult
    {
        /// <summary>
        /// The identifier of the widget.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// An error code, in case the response should trigger a widget error.
        /// </summary>
        public int ?Error { get; set; }
        
        /// <summary>
        /// Any additional data to include in the response.
        /// </summary>
        public object ?Data { get; set; }
    }
}
