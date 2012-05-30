using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.Types.AI
{
    public enum YieldType { Production, Science, Gold, Food, Happiness };

    public class Yield
    {
        public YieldType Type { get; set; }
        public int Amount { get; set; }

        [ContentSerializer(Optional = true)]
        public bool CapitalOnly { get; set; }
    }
}
