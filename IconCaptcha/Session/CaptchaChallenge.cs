/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Collections.Generic;

namespace IconCaptcha.Session
{
    public class CaptchaChallenge
    {
        /// <summary>
        /// Positions of every icon on the challenge image.
        /// </summary>
        public List<int> Icons { get; set; } = new();

        /// <summary>
        /// List of icon identifiers used in the challenge.
        /// </summary>
        public List<int> IconIds { get; set; } = new();

        /// <summary>
        /// The icon identifier of the correct answer/icon.
        /// </summary>
        public int CorrectId { get; set; }

        /// <summary>
        /// The name of the theme used by the captcha widget. The default is theme "light".
        /// </summary>
        public string Mode { get; set; } = "light";

        /// <summary>
        /// Indicates whether the captcha challenge has been requested.
        /// </summary>
        public bool Requested { get; set; }

        /// <summary>
        /// Indicates whether the captcha challenge was successfully completed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Tracks the number of times an incorrect answer was given.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// The time at which the timeout, given to the visitor after too many incorrect
        /// tries, expires. When the value is null, no timeout is currently active.
        /// </summary>
        public DateTime? AttemptsTimeout { get; set; }

        /// <summary>
        /// Resets all data stored in the session.
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
