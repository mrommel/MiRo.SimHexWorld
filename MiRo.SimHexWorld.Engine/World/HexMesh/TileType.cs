using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.World
{
    [Flags]
    public enum TileType
    {
        None = 0x00,
        Sand = 0x01,
        Grass = 0x02,
        Rock = 0x04,
        Snow = 0x08,
    };

    //[Flags]
    public enum TileHeight
    {
        None = 0x00,
        Zero = 0x01,
        Shore = 0x02,
        Deep = 0x04,
        Hill = 0x08,
        Peak = 0x10,
        Coast,
    };
}
