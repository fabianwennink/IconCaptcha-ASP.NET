/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

namespace IconCaptcha.Enums
{
    public enum ActionType
    {
        /// <summary>
        /// Indicates that a challenge was requested.
        /// </summary>
        InitiateChallenge = 1,

        /// <summary>
        /// Indicates that the user made an icon selection.
        /// </summary>
        SetSelectedIcon = 2,

        /// <summary>
        /// Indicates that the challenge has expired.
        /// </summary>
        TimeExpired = 3
    }
}
