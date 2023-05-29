using System;
using System.Collections.Generic;

namespace IconCaptcha
{
    /// <summary>
    /// Default values for all the server-side options.
    /// </summary>
    public class Options
    {
        public string IconPath { get; set; } // required

        public IDictionary<string, Theme> Themes { get; set; } = IconCaptcha.CAPTCHA_DEFAULT_THEME_COLORS;

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
        public int AvailableIcons { get; set; } = 180;
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
