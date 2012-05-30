using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Misc
{
    public class MissingAsset
    {
        public AbstractNamedEntity Entity { get; set; }
        public string MissingType { get; set; }
        public string MissingItem { get; set; }

        public MissingAsset(AbstractNamedEntity entity, string missingType, string missingAsset)
        {
            Entity = entity;
            MissingType = missingType;
            MissingItem = missingAsset;
        }

        public override string ToString()
        {
            return Entity.Name + " (" + Entity.GetType().Name + ") => " + MissingType + ": " + MissingItem;
        }
    }
}
