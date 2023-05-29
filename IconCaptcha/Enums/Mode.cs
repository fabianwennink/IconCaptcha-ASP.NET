/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

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
