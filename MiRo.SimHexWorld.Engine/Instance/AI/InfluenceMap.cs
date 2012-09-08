using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.UI;

namespace MiRo.SimHexWorld.Engine.Instance.AI
{
    public class InfluenceMap
    {
        private float[,] _data;
        private int _width, _height;

        public InfluenceMap(int width, int height, float initialValue = 0f)
        {
            _width = width;
            _height = height;

            _data = new float[_width, _height];

            Fill(initialValue);
        }

        public void Fill(float value)
        {
            for (int x = 0; x < _width; ++x)
                for (int y = 0; y < _height; ++y)
                    _data[x, y] = value;
        }

        public bool IsLocalMaximum(HexPoint pt)
        {
            bool localMaximum = this[pt] > 0;

            // local maximum
            foreach (HexPoint pts in pt.Neighbors)
                if (MainWindow.Game.Map.IsValid(pts))
                    localMaximum &= (this[pt] >= this[pts]);

            return localMaximum;
        }

        public float this[int x, int y]
        {
            get
            {
                return _data[x, y];
            }
            set
            {
                _data[x, y] = value;
            }
        }

        public float this[HexPoint pt]
        {
            get
            {
                return _data[pt.X, pt.Y];
            }
            set
            {
                _data[pt.X, pt.Y] = value;
            }
        }

        public void Apply(HexPoint hexPoint, int value)
        {
            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    int dist = HexPoint.GetDistance(hexPoint.X, hexPoint.Y, x, y);
                    this[x, y] += value / (float)Math.Pow(2, dist);
                }
            }
        }
    }
}
