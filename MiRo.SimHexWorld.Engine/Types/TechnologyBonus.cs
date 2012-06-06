using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class TechnologyBonus
    {
        public string TechnologyName { get; set; }
        public Yield Yield { get; set; }

        public bool IsValid(List<Tech> techs)
        {
            return techs.Select(a => a.Name).Contains(TechnologyName);
        }
    }
}
