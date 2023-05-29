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
