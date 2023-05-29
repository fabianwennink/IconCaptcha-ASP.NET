/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Text.Json.Serialization;

namespace IconCaptcha
{
    public class Payload
    {
        [JsonPropertyName("i")]
        public int CaptchaId { get; set; }

        [JsonPropertyName("x")]
        public int? XPos { get; set; }

        [JsonPropertyName("y")]
        public int? YPos { get; set; }

        [JsonPropertyName("w")]
        public int? Width { get; set; }

        [JsonPropertyName("a")]
        public ActionType Action { get; set; }

        [JsonPropertyName("t")]
        public string Mode { get; set; }

        [JsonPropertyName("tk")]
        public string Token { get; set; }
    }
}
