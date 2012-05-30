using System.Collections.Generic;
using MiRoSimHexWorld.Engine.World.Scenarios;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class Civ5MapReader : ContentTypeReader<Civ5Map>
    {
        protected override Civ5Map Read(ContentReader input, Civ5Map existingInstance)
        {
            var civ5Map = new Civ5Map();

            civ5Map.Name = input.ReadString();
            civ5Map.Description = input.ReadString();
            civ5Map.Width = input.ReadInt32();
            civ5Map.Height = input.ReadInt32();

            civ5Map.Init();

            for (int x = 0; x < civ5Map.Width; x++ )
            {
                for (int y = 0; y < civ5Map.Height; y++)
                {
                    civ5Map[x, y].TerrainType = input.ReadByte();
                    civ5Map[x, y].RessourceType = input.ReadByte();
                    civ5Map[x, y].Feature1stType = input.ReadByte();
                    civ5Map[x, y].River = input.ReadByte();

                    civ5Map[x, y].Elevation = input.ReadByte();
                    civ5Map[x, y].Feature2ndType = input.ReadByte();
                    civ5Map[x, y].ArtStyleValue = input.ReadByte();
                    civ5Map[x, y].Unused2 = input.ReadByte();
                }
            }

            int terrainNameCount = input.ReadInt32();

            civ5Map.TerrainNames = new List<string>();

            for (int i = 0; i < terrainNameCount; ++i)
                civ5Map.TerrainNames.Add(input.ReadString());

            int featureNameCount = input.ReadInt32();

            civ5Map.FeatureNames1st = new List<string>();

            for (int i = 0; i < featureNameCount; ++i)
                civ5Map.FeatureNames1st.Add(input.ReadString());

            return civ5Map;
        }
    }
}
