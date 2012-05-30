using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class HumanPlayerData : AbstractPlayerData
	{
        public HumanPlayerData(int id, Civilization tribe) 
			: base(id, tribe, true)
		{
		
		}
		
		public override void StartAiThreads()
		{
		
		}

        public override void Dispose()
        {
            
        }

        public override string ToString()
        {
            return "Human";
        }
    }
}
