using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.Misc
{
    public class ColorConversion
    {
        private static Dictionary<string, Color> _colorMap = new Dictionary<string, Color>();

        public ColorConversion()
        {
            if (_colorMap.Count == 0)
                FillColors();
        }

        public static Color FromName(string name)
        {
            if (_colorMap.Count == 0)
                FillColors();

            KeyValuePair<string, Color> pair = _colorMap.FirstOrDefault(a => a.Key == name);

            if (pair.Value == Color.Transparent && name != "Transparent")
                throw new Exception("Cannot find BackColor with Name: " + name);

            return pair.Value;
        }

        public static string FromColor(Color color)
        {
            KeyValuePair<string, Color> pair = _colorMap.FirstOrDefault(a => a.Value == color);

            if (pair.Value == null)
                throw new Exception("Cannot find BackColorname for: " + color.ToString());

            return pair.Key;
        }

        private static void FillColors()
        {
            _colorMap.Add("AliceBlue", Color.AliceBlue);
            _colorMap.Add("AntiqueWhite", Color.AntiqueWhite);
            _colorMap.Add("Aqua", Color.Aqua);
            _colorMap.Add("Aquamarine", Color.Aquamarine);
            _colorMap.Add("Azure", Color.Azure);
            _colorMap.Add("Beige", Color.Beige);
            _colorMap.Add("Bisque", Color.Bisque);
            _colorMap.Add("Black", Color.Black);
            _colorMap.Add("BlanchedAlmond", Color.BlanchedAlmond);
            _colorMap.Add("Blue", Color.Blue);
            _colorMap.Add("BlueViolet", Color.BlueViolet);
            _colorMap.Add("Brown", Color.Brown);
            _colorMap.Add("BurlyWood", Color.BurlyWood);
            _colorMap.Add("CadetBlue", Color.CadetBlue);
            _colorMap.Add("Chartreuse", Color.Chartreuse);
            _colorMap.Add("Chocolate", Color.Chocolate);
            _colorMap.Add("Coral", Color.Coral);
            _colorMap.Add("CornflowerBlue", Color.CornflowerBlue);
            _colorMap.Add("Cornsilk", Color.Cornsilk);
            _colorMap.Add("Crimson", Color.Crimson);
            _colorMap.Add("Cyan", Color.Cyan);
            _colorMap.Add("DarkBlue", Color.DarkBlue);
            _colorMap.Add("DarkCyan", Color.DarkCyan);
            _colorMap.Add("DarkGoldenrod", Color.DarkGoldenrod);
            _colorMap.Add("DarkGray", Color.DarkGray);
            _colorMap.Add("DarkGreen", Color.DarkGreen);
            _colorMap.Add("DarkKhaki", Color.DarkKhaki);
            _colorMap.Add("DarkMagenta", Color.DarkMagenta);
            _colorMap.Add("DarkOliveGreen", Color.DarkOliveGreen);
            _colorMap.Add("DarkOrange", Color.DarkOrange);
            _colorMap.Add("DarkOrchid", Color.DarkOrchid);
            _colorMap.Add("DarkRed", Color.DarkRed);
            _colorMap.Add("DarkSalmon", Color.DarkSalmon);
            _colorMap.Add("DarkSlateBlue", Color.DarkSlateBlue);
            _colorMap.Add("DarkSlateGray", Color.DarkSlateGray);
            _colorMap.Add("DarkTurquoise", Color.DarkTurquoise);
            _colorMap.Add("DarkViolet", Color.DarkViolet);
            _colorMap.Add("DeepPink", Color.DeepPink);
            _colorMap.Add("DeepSkyBlue", Color.DeepSkyBlue);
            _colorMap.Add("DimGray", Color.DimGray);
            _colorMap.Add("DodgerBlue", Color.DodgerBlue);
            _colorMap.Add("Firebrick", Color.Firebrick);
            _colorMap.Add("FloralWhite", Color.FloralWhite);
            _colorMap.Add("ForestGreen", Color.ForestGreen);
            _colorMap.Add("Fuchsia", Color.Fuchsia);
            _colorMap.Add("Gainsboro", Color.Gainsboro);
            _colorMap.Add("GhostWhite", Color.GhostWhite);
            _colorMap.Add("Gold", Color.Gold);
            _colorMap.Add("Goldenrod", Color.Goldenrod);
            _colorMap.Add("Gray", Color.Gray);
            _colorMap.Add("Green", Color.Green);
            _colorMap.Add("GreenYellow", Color.GreenYellow);
            _colorMap.Add("Honeydew", Color.Honeydew);
            _colorMap.Add("HotPink", Color.HotPink);
            _colorMap.Add("IndianRed", Color.IndianRed);
            _colorMap.Add("Indigo", Color.Indigo);
            _colorMap.Add("Ivory", Color.Ivory);
            _colorMap.Add("Khaki", Color.Khaki);
            _colorMap.Add("Lavender", Color.Lavender);
            _colorMap.Add("LavenderBlush", Color.LavenderBlush);
            _colorMap.Add("LawnGreen", Color.LawnGreen);
            _colorMap.Add("LemonChiffon", Color.LemonChiffon);
            _colorMap.Add("LightBlue", Color.LightBlue);
            _colorMap.Add("LightCoral", Color.LightCoral);
            _colorMap.Add("LightCyan", Color.LightCyan);
            _colorMap.Add("LightGoldenrodYellow", Color.LightGoldenrodYellow);
            _colorMap.Add("LightGray", Color.LightGray);
            _colorMap.Add("LightGreen", Color.LightGreen);
            _colorMap.Add("LightPink", Color.LightPink);
            _colorMap.Add("LightSalmon", Color.LightSalmon);
            _colorMap.Add("LightSeaGreen", Color.LightSeaGreen);
            _colorMap.Add("LightSkyBlue", Color.LightSkyBlue);
            _colorMap.Add("LightSlateGray", Color.LightSlateGray);
            _colorMap.Add("LightSteelBlue", Color.LightSteelBlue);
            _colorMap.Add("LightYellow", Color.LightYellow);
            _colorMap.Add("Lime", Color.Lime);
            _colorMap.Add("LimeGreen", Color.LimeGreen);
            _colorMap.Add("Linen", Color.Linen);
            _colorMap.Add("Magenta", Color.Magenta);
            _colorMap.Add("Maroon", Color.Maroon);
            _colorMap.Add("MediumAquamarine", Color.MediumAquamarine);
            _colorMap.Add("MediumBlue", Color.MediumBlue);
            _colorMap.Add("MediumOrchid", Color.MediumOrchid);
            _colorMap.Add("MediumPurple", Color.MediumPurple);
            _colorMap.Add("MediumSeaGreen", Color.MediumSeaGreen);
            _colorMap.Add("MediumSlateBlue", Color.MediumSlateBlue);
            _colorMap.Add("MediumSpringGreen", Color.MediumSpringGreen);
            _colorMap.Add("MediumTurquoise", Color.MediumTurquoise);
            _colorMap.Add("MediumVioletRed", Color.MediumVioletRed);
            _colorMap.Add("MidnightBlue", Color.MidnightBlue);
            _colorMap.Add("MintCream", Color.MintCream);
            _colorMap.Add("MistyRose", Color.MistyRose);
            _colorMap.Add("Moccasin", Color.Moccasin);
            _colorMap.Add("NavajoWhite", Color.NavajoWhite);
            _colorMap.Add("Navy", Color.Navy);
            _colorMap.Add("OldLace", Color.OldLace);
            _colorMap.Add("Olive", Color.Olive);
            _colorMap.Add("OliveDrab", Color.OliveDrab);
            _colorMap.Add("Orange", Color.Orange);
            _colorMap.Add("OrangeRed", Color.OrangeRed);
            _colorMap.Add("Orchid", Color.Orchid);
            _colorMap.Add("PaleGoldenrod", Color.PaleGoldenrod);
            _colorMap.Add("PaleGreen", Color.PaleGreen);
            _colorMap.Add("PaleTurquoise", Color.PaleTurquoise);
            _colorMap.Add("PaleVioletRed", Color.PaleVioletRed);
            _colorMap.Add("PapayaWhip", Color.PapayaWhip);
            _colorMap.Add("PeachPuff", Color.PeachPuff);
            _colorMap.Add("Peru", Color.Peru);
            _colorMap.Add("Pink", Color.Pink);
            _colorMap.Add("Plum", Color.Plum);
            _colorMap.Add("PowderBlue", Color.PowderBlue);
            _colorMap.Add("Purple", Color.Purple);
            _colorMap.Add("Red", Color.Red);
            _colorMap.Add("RosyBrown", Color.RosyBrown);
            _colorMap.Add("RoyalBlue", Color.RoyalBlue);
            _colorMap.Add("SaddleBrown", Color.SaddleBrown);
            _colorMap.Add("Salmon", Color.Salmon);
            _colorMap.Add("SandyBrown", Color.SandyBrown);
            _colorMap.Add("SeaGreen", Color.SeaGreen);
            _colorMap.Add("SeaShell", Color.SeaShell);
            _colorMap.Add("Sienna", Color.Sienna);
            _colorMap.Add("Silver", Color.Silver);
            _colorMap.Add("SkyBlue", Color.SkyBlue);
            _colorMap.Add("SlateBlue", Color.SlateBlue);
            _colorMap.Add("SlateGray", Color.SlateGray);
            _colorMap.Add("Snow", Color.Snow);
            _colorMap.Add("SpringGreen", Color.SpringGreen);
            _colorMap.Add("SteelBlue", Color.SteelBlue);
            _colorMap.Add("Tan", Color.Tan);
            _colorMap.Add("Teal", Color.Teal);
            _colorMap.Add("Thistle", Color.Thistle);
            _colorMap.Add("Tomato", Color.Tomato);
            _colorMap.Add("Transparent", Color.Transparent);
            _colorMap.Add("Turquoise", Color.Turquoise);
            _colorMap.Add("Violet", Color.Violet);
            _colorMap.Add("Wheat", Color.Wheat);
            _colorMap.Add("White", Color.White);
            _colorMap.Add("WhiteSmoke", Color.WhiteSmoke);
            _colorMap.Add("Yellow", Color.Yellow);
            _colorMap.Add("YellowGreen", Color.YellowGreen);
        }
    }
}
