using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Helper
{
    public class Vector2Rectangle
    {
        public Vector2Rectangle(float x1, float y1, float x2, float y2)
        {
            TopLeft = new Vector2(x1, y1);
            TopRight = new Vector2(x2, y1);
            BottomLeft = new Vector2(x1, y2);
            BottomRight = new Vector2(x2, y2);
        }

        public Vector2 TopLeft
        {
            get;
            set;
        }

        public Vector2 TopRight
        {
            get;
            set;
        }

        public Vector2 BottomLeft
        {
            get;
            set;
        }

        public Vector2 BottomRight
        {
            get;
            set;
        }
    }
}
