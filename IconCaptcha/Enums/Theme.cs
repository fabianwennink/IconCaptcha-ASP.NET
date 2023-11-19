/*
    IconCaptcha - Copyright 2023, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

namespace IconCaptcha.Enums
{
    public class Theme
    {
        public Mode Icons { get; set; }
        public byte[] Color { get; set; }

        public Theme(Mode icons, byte[] color)
        {
            Icons = icons;
            Color = color;
        }
    }
}
