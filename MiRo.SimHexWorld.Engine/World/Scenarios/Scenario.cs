using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiRoSimHexWorld.Engine.World.Scenarios
{
    public class Scenario
    {
        public class StartLocation
        {
            public string Civilization { get; set; }
            public Vector2 Position { get; set; }
        }

        public string Name { get; set; }
        public string MapName { get; set; }
        public List<StartLocation> StartLocations { get; set; }
    }
}
