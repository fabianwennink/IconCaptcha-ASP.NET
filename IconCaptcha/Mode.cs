using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace IconCaptcha
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Mode
    {
        light,
        dark,
    }
}
