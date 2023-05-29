/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace IconCaptcha
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
