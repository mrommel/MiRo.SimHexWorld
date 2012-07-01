using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public interface IFormation
    {
        int Positions
        {
            get;
        }

        Vector3 GetPosition(int index);
    }
}
