using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public class Formation3 : IFormation
    {
        List<Vector3> _basePositions = new List<Vector3>();

        public Formation3()
        {
            _basePositions.Add(new Vector3(1f, 0.0f, 0f));
            _basePositions.Add(new Vector3(-1.5f, 0.0f, -1.5f));
            _basePositions.Add(new Vector3(-1.5f, 0.0f, 1.5f));
        }

        public int Positions
        {
            get { return _basePositions.Count; }
        }

        public Vector3 GetPosition(int index)
        {
            if (index >= _basePositions.Count || index < 0)
                return Vector3.Zero;

            return _basePositions[index];
        }
    }
}
