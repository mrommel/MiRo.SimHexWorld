using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.Screens
{
    [Serializable]
    public class Size
    {
        public Size() { }

        public Size(Vector2 vector2)
        {
            Width = (int)vector2.X;
            Height = (int)vector2.Y;
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
