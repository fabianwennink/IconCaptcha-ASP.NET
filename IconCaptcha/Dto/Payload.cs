/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Text.Json.Serialization;
using IconCaptcha.Enums;

namespace IconCaptcha.Dto
{
    public class Payload
    {
        /// <summary>
        /// Identifier of the widget.
        /// </summary>
        [JsonPropertyName("i")]
        public long CaptchaId { get; set; }

        /// <summary>
        /// X-coordinate of the position clicked on the challenge image.
        /// </summary>
        [JsonPropertyName("x")]
        public int? XPos { get; set; }

        /// <summary>
        /// Y-coordinate of the position clicked on the challenge image.
        /// </summary>
        [JsonPropertyName("y")]
        public int? YPos { get; set; }

        /// <summary>
        /// Raw, non-rounded width of the challenge image.
        /// </summary>
        [JsonPropertyName("w")]
        public double? _width { get; set; }
        
        /// <summary>
        /// Rounded width of the challenge image.
        /// </summary>
        public int? Width
        {
            get => _width != null ? (int)Math.Ceiling(_width.Value) : null;
            set => _width = value;
        }

        /// <summary>
        /// Action performed by the visitor/widget.
        /// </summary>
        [JsonPropertyName("a")]
        public ActionType Action { get; set; }

        /// <summary>
        /// Theme of the widget.
        /// </summary>
        [JsonPropertyName("t")]
        public string Mode { get; set; }

        /// <summary>
        /// Security token associated with the session.
        /// </summary>
        [JsonPropertyName("tk")]
        public string Token { get; set; }
    }
}
