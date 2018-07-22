/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IconCaptcha_ASP
{
    public class Utils
    {
        /// <summary>
        /// Random object.
        /// </summary>
        public static readonly Random Random = new Random();

        /// <summary>
        /// Returns a SHA256 hash of the given input string.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <returns>SHA256 hash of the input string.</returns>
        public static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        /// <summary>
        /// Sanitizes the given input string, removes invalid file name characters.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>A sanitizes file name.</returns>
        public static string SanitizeString(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format( @"[{0}]+", invalidChars );
            return Regex.Replace(name, invalidReStr, "_" );
        }

        /// <summary>
        /// Converts a string and returns the int value, if it's an int.
        /// </summary>
        /// <param name="value">The string to convert into an integer.</param>
        /// <returns></returns>
        public static int ConvertToInt(string value)
        {
            int v = -1;
            int.TryParse(value, out v);

            return v;
        }
    }
}
