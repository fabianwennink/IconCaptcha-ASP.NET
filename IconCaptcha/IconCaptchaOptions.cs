/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Collections.Generic;
using IconCaptcha.Enums;

namespace IconCaptcha
{
    /// <summary>
    /// Default values for all the server-side options.
    /// </summary>
    public class IconCaptchaOptions
    {
        /// <summary>
        /// The absolute path to the icons folder.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// A collection of all available themes.
        /// </summary>
        public IDictionary<string, Theme> Themes { get; set; } = IconCaptchaService.CaptchaDefaultThemeColors;

        /// <summary>
        /// The error messages.
        /// </summary>
        public MessageOptions Messages { get; set; } = new();

        /// <summary>
        /// The options related to challenge generation.
        /// </summary>
        public ImageOptions Image { get; set; } = new();

        /// <summary>
        /// The options related to attempts and timeouts.
        /// </summary>
        public AttemptsOptions Attempts { get; set; } = new();
        
        public bool Token { get; set; } = true;
    }
    
    public class ImageOptions
    {
        /// <summary>
        /// Number of unique icons available.
        /// </summary>
        public int AvailableIcons { get; set; } = 250;
        
        /// <summary>
        /// The options related to the amount of icons to use in a challenge image.
        /// </summary>
        public ImageAmountOptions Amount { get; set; } = new();
        
        /// <summary>
        /// Whether to randomly rotate the icons in a challenge image.
        /// </summary>
        public bool Rotate { get; set; } = true;
        
        /// <summary>
        /// The options related to random icon flipping.
        /// </summary>
        public ImageFlipOptions Flip { get; set; } = new();
        
        /// <summary>
        /// Whether to render a border between the icons in a challenge image.
        /// </summary>
        public bool Border { get; set; } = true;
    }

    public class ImageAmountOptions
    {
        /// <summary>
        /// The minimum number of icons to use in each challenge image.
        /// The lowest possible number of icons per challenge is 5.
        /// </summary>
        public int Min { get; set; } = 5;
        
        /// <summary>
        /// The minimum number of icons to use in each challenge image.
        /// The highest possible number of icons per challenge is 8.
        /// </summary>
        public int Max { get; set; } = 8;
    }

    public class ImageFlipOptions
    {
        /// <summary>
        /// Whether to randomly flip the icons in a challenge image horizontally.
        /// </summary>
        public bool Horizontally { get; set; } = true;
        
        /// <summary>
        /// Whether to randomly flip the icons in a challenge image vertically.
        /// </summary>
        public bool Vertically { get; set; } = true;
    }

    public class AttemptsOptions
    {
        /// <summary>
        /// The maximum number of incorrect attempts a visitor can make before they will receive a timeout.
        /// </summary>
        public int Amount { get; set; } = 5;
        
        /// <summary>
        /// The time in seconds which the visitor has to wait after making too many incorrect
        /// attempts before being able to request a new challenge.
        /// </summary>
        public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
    
    public class MessageOptions
    {
        public string WrongIcon { get; set; } = "You've selected the wrong image.";
        public string NoSelection { get; set; } = "No image has been selected.";
        public string EmptyForm { get; set; } = "You've not submitted any form.";
        public string InvalidId { get; set; } = "The captcha ID was invalid.";
        public string FormToken { get; set; } = "The form token was invalid";
    }
}
