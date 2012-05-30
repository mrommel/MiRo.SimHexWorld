using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld
{
    public interface IMeshObject
    {
        List<VertexPositionNormalTexture> Vertices { get; }
        List<short> Indices { get; }
    }
}
