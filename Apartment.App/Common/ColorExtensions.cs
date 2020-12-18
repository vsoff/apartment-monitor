using System;
using System.Windows.Media;

namespace Apartment.App.Common
{
    public static class ColorExtensions
    {
        public static string ToHexString(this Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";

        public static Color ParseColor(this string hex)
        {
            if (hex == null) throw new ArgumentNullException(nameof(hex));
            hex = hex.Replace("#", string.Empty);
            switch (hex.Length)
            {
                case 6:
                {
                    byte r = (byte) Convert.ToUInt32(hex.Substring(0, 2), 16);
                    byte g = (byte) Convert.ToUInt32(hex.Substring(2, 2), 16);
                    byte b = (byte) Convert.ToUInt32(hex.Substring(4, 2), 16);
                    return Color.FromRgb(r, g, b);
                }
                case 8:
                {
                    byte a = (byte) Convert.ToUInt32(hex.Substring(0, 2), 16);
                    byte r = (byte) Convert.ToUInt32(hex.Substring(2, 2), 16);
                    byte g = (byte) Convert.ToUInt32(hex.Substring(4, 2), 16);
                    byte b = (byte) Convert.ToUInt32(hex.Substring(6, 2), 16);
                    return Color.FromArgb(a, r, g, b);
                }
                default: throw new ArgumentOutOfRangeException(nameof(hex));
            }
        }
    }
}