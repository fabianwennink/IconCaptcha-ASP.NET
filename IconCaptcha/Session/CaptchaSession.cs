/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace IconCaptcha.Session
{
    public class CaptchaSession
    {
        /// <summary>
        /// The captcha token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The sessions by idenfier.
        /// </summary>
        public Dictionary<int, CaptchaSessionData> Sessions { get; set; } = new();

        public void Add(int key, CaptchaSessionData value)
        {
            Sessions.Add(key, value);
        }

        public bool ContainsKey(int key)
        {
            return Sessions.ContainsKey(key);
        }

        public bool TryGetValue(int key, out CaptchaSessionData value)
        {
            return Sessions.TryGetValue(key, out value);
        }

        public bool Remove(int key)
        {
            return Sessions.Remove(key);
        }
    }
}
