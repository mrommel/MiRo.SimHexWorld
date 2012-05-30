using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MiRo.SimHexWorld
{
    public class HexagonTexture
    {
        float[] _core = new float[8];
        int _tilesX = 1, _tilesY = 1;

        public HexagonTexture(float b1, float b2, float b3, float a1, float a2, float a3, float a4, float a5, int tilesX, int tilesY)
        {
            // x-axis
            _core[0] = b1;
            _core[1] = b2;
            _core[2] = b3;

            // y-axis
            _core[3] = a1;
            _core[4] = a2;
            _core[5] = a3;
            _core[6] = a4;
            _core[7] = a5;

            _tilesX = tilesX;
            _tilesY = tilesY;
        }

        public float[] GetPoints(int x, int y)
        {
            return new float[] { 
		                _core[0] / _tilesX + ((float)x) / _tilesX, 
		                _core[1] / _tilesX + ((float)x) / _tilesX, 
		                _core[2] / _tilesX + ((float)x) / _tilesX, 
		                _core[3] / _tilesY + ((float)y) / _tilesY, 
		                _core[4] / _tilesY + ((float)y) / _tilesY, 
		                _core[5] / _tilesY + ((float)y) / _tilesY, 
		                _core[6] / _tilesY + ((float)y) / _tilesY, 
		                _core[7] / _tilesY + ((float)y) / _tilesY 
		            };
        }
    }
}
