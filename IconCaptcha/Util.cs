/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Text;

namespace IconCaptcha
{
    public static class Util
    {
        public static string Base64Encode(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        
        public static string Base64Decode(string value)
        {
            var valueBytes = Convert.FromBase64String(value);
            
            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}
