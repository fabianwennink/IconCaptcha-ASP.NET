/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System.Text.Json.Serialization;
using IconCaptcha.Enums;

namespace IconCaptcha.Dto
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
