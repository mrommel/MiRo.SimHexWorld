using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Flavour
    {
        [ContentSerializerIgnore]
        public FlavourData Data
        {
            get
            {
                if (Provider.Instance.Flavours.ContainsKey(Name))
                    return Provider.Instance.Flavours[Name];

                return null;
            }
        }

        public string Name { get; set; }
        public float Amount { get; set; }
    }
}
