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
