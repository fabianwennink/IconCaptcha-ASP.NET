/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace IconCaptcha.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Mode
    {
        light,
        dark,
    }
}
