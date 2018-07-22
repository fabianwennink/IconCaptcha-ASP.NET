/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace IconCaptcha_ASP
{
    public class IconCaptchaSession
    {
        /// <summary>
        /// The array containing all the image hashes used by this captcha.
        /// </summary>
        public List<string> Hashes { get; set; }

        /// <summary>
        /// The amount of times the images have been requested by the captcha.
        /// </summary>
        public int IconRequests { get; set; }

        /// <summary>
        /// The captcha's theme name.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// The last icon number that was clicked (1-5)
        /// </summary>
        public int LastClicked { get; set; }

        /// <summary>
        /// The ID of the correct icon hash.
        /// </summary>
        public int CorrectId { get; set; }

        /// <summary>
        /// The ID of the correct icon hash.
        /// </summary>
        public int IncorrectId { get; set; }

        /// <summary>
        /// The correct icon's hash.
        /// </summary>
        public string CorrectHash { get; set; }

        /// <summary>
        /// If the captcha was completed (correct icon selected) or not.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Creates a new IconCaptchaSession object. Session data regarding the 
        /// captcha will be stored and can be retrieved when necessary.
        /// </summary>
        /// <param name="theme"></param>
        public IconCaptchaSession(string theme = "light")
        {
            Theme = theme;
            Hashes = new List<string>();
            IconRequests = 0;
            LastClicked = -1;
            CorrectId = 0;
            IncorrectId = 0;
            CorrectHash = String.Empty;
            Completed = false;
        }

        /// <summary>
        /// This will clear the set hashes, and reset the icon 
        /// request counter and last clicked icon.
        /// </summary>
        public void Clear()
        {
            CorrectId = 0;
            IncorrectId = 0;
            Hashes = new List<string>();
            IconRequests = -1;
            LastClicked = 0;
        }
    }
}
