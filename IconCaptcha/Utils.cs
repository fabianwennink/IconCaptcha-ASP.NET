/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Text;

namespace IconCaptcha
{
    public static class Utils
    {
        /// <summary>
        /// Encodes the given string using base64.
        /// </summary>
        /// <param name="value">A string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static string Base64Encode(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            
            return Convert.ToBase64String(bytes);
        }
        
        /// <summary>
        /// Decodes the given base64 encoded string.
        /// </summary>
        /// <param name="value">A base64 encoded string.</param>
        /// <returns>The decoded string.</returns>
        public static string Base64Decode(string value)
        {
            var bytes = Convert.FromBase64String(value);
            
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
