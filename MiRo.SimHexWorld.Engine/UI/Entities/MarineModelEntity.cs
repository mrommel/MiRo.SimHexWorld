using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    public class MarineModelEntity : UnitEntity
    {
        public MarineModelEntity(AbstractPlayerData player, Unit unit)
            : base(player, unit, "PlayerMarine_mdla")
        {
            Scale = new Vector3(0.2f);
        }
    }
}
