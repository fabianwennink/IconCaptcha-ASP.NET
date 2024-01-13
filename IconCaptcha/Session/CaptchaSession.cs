/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System.Collections.Generic;

namespace IconCaptcha.Session
{
    public class CaptchaSession
    {
        /// <summary>
        /// The captcha session token.
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// A collection of captcha challenges, indexed by the captcha identifier.
        /// </summary>
        public Dictionary<long, CaptchaChallenge> Challenges { get; set; } = new();

        /// <summary>
        /// Store a new captcha challenges.
        /// </summary>
        /// <param name="key">The identifier of the captcha.</param>
        /// <param name="value">The captcha challenges to be added.</param>
        public void Add(long key, CaptchaChallenge value)
        {
            Challenges.Add(key, value);
        }
        
        /// <summary>
        /// Checks whether the captcha challenges with the specified identifier exists.
        /// </summary>
        /// <param name="key">The captcha identifier to check for.</param>
        /// <returns>True if the captcha challenges exists, otherwise false.</returns>
        public bool ContainsKey(long key)
        {
            return Challenges.ContainsKey(key);
        }
        
        /// <summary>
        /// Tries to get the captcha challenges associated with the specified identifier.
        /// </summary>
        /// <param name="key">The captcha identifier to search for.</param>
        /// <param name="value">The captcha challenges associated with the specified identifier, if it exists.</param>
        /// <returns>True if the captcha challenges exists, otherwise false.</returns>
        public bool TryGetValue(long key, out CaptchaChallenge value)
        {
            return Challenges.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the captcha challenges with the specified identifier.
        /// </summary>
        /// <param name="key">The identifier of the captcha challenge to remove.</param>
        /// <returns>True if the captcha challenges was removed, otherwise false.</returns>
        public bool Remove(long key)
        {
            return Challenges.Remove(key);
        }
    }
}
