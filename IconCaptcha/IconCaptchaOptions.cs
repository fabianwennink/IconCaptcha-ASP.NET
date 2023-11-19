/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
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
        public string IconPath { get; set; }

        public IDictionary<string, Theme> Themes { get; set; } = IconCaptchaService.CaptchaDefaultThemeColors;

        public MessageOptions Messages { get; set; } = new();

        public ImageOptions Image { get; set; } = new();

        public AttemptsOptions Attempts { get; set; } = new();
        
        public bool Token { get; set; } = true;
    }

    public class MessageOptions
    {
        public string WrongIcon { get; set; } = "You've selected the wrong image.";
        public string NoSelection { get; set; } = "No image has been selected.";
        public string EmptyForm { get; set; } = "You've not submitted any form.";
        public string InvalidId { get; set; } = "The captcha ID was invalid.";
        public string FormToken { get; set; } = "The form token was invalid";
    }

    public class ImageOptions
    {
        public int AvailableIcons { get; set; } = 250;
        public ImageAmountOptions Amount { get; set; } = new();
        public bool Rotate { get; set; } = true;
        public ImageFlipOptions Flip { get; set; } = new();
        public bool Border { get; set; } = true;
    }

    public class ImageAmountOptions
    {
        public int Min { get; set; } = 5;
        public int Max { get; set; } = 8;
    }

    public class ImageFlipOptions
    {
        public bool Horizontally { get; set; } = true;
        public bool Vertically { get; set; } = true;
    }

    public class AttemptsOptions
    {
        public int Amount { get; set; } = 5;
        public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
