using System.Collections.Generic;
using MiRoSimHexWorld.Engine.World.Scenarios;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    // MiRoSimHexWorld.Engine.World.Scenarios.ScenarioReader, MiRoSimHexWorld.Engine.World.Scenarios
    public class MapDataReader : ContentTypeReader<MapData>
    {
        protected override MapData Read(ContentReader input, MapData existingInstance)
        {
            var map = new MapData();

            //scenario.Name = input.ReadString();
            //scenario.MapName = input.ReadString();

            //int pos = input.ReadInt32();

            //scenario.StartLocations = new List<Scenario.StartLocation>();

            //for (int i = 0; i < pos; i++)
            //{
            //    var loc = new Scenario.StartLocation {Position = input.ReadVector2(), Civilization = input.ReadString()};

            //    scenario.StartLocations.Add(loc);
            //}

            return map;
        }
    }
}
