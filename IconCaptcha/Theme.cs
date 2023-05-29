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
