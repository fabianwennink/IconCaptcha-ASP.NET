/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
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
