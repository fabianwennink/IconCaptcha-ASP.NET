﻿/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace IconCaptcha
{
    public class CaptchaSessionData
    {
        /// <summary>
        /// The positions of the icon on the generated image.
        /// </summary>
        public List<int> Icons { get; set; } = new();

        /// <summary>
        /// List of used icon IDs.
        /// </summary>
        public List<int> IconIds { get; set; } = new();

        /// <summary>
        /// The icon ID of the correct answer/icon.
        /// </summary>
        public int CorrectId { get; set; }

        /// <summary>
        /// The name of the theme used by the captcha instance.
        /// </summary>
        public string Mode { get; set; } = "light";

        /// <summary>
        /// If the captcha image has been requested yet.
        /// </summary>
        public bool Requested { get; set; }

        /// <summary>
        /// If the captcha was completed (correct icon selected) or not.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// If the captcha was completed (correct icon selected) or not.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Attempts timeout.
        /// </summary>
        public DateTime? AttemptsTimeout { get; set; }

        /// <summary>
        /// This will clear the set hashes, and reset the icon request counter and last clicked icon.
        /// </summary>
        public void Clear()
        {
            Icons.Clear();
            IconIds.Clear();
            CorrectId = 0;
            Requested = false;
            Completed = false;
            Attempts = 0;
            AttemptsTimeout = null;
        }
    }
}