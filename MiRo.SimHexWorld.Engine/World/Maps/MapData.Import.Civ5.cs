using System;
using System.Linq;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;
using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    partial class MapData
    {
        //public static MapData FromCiv5Map(Civ5Map civ5Map)
        //{
        //    var map = new MapData();

        //    map.InitFromCiv5Map(civ5Map);

        //    return map;
        //}

        //public void LoadCiv5Map2(string filename)
        //{
        //    var map = new Civ5Map();
        //    map.Load(filename);

        //    InitFromCiv5Map(map);

        //    if( OnMapUpdate != null)
        //        OnMapUpdate(new MapChangeArgs(this));
        //}

        public void InitFromCiv5Map(Civ5Map civ5Map)
        {
            Init(civ5Map.Width + 1, civ5Map.Height + 1);
            //Init(civ5Map.Height + 1, civ5Map.Width + 1);

            Dictionary<byte, Terrain> terrainDict = new Dictionary<byte, Terrain>();

            byte b = 0;
            foreach (string terrainName in civ5Map.TerrainNames)
            {
                Terrain t = Provider.Instance.Terrains.FirstOrDefault(a => a.Value.Civ5Name == terrainName).Value;

                if (t == null)
                    throw new Exception("No Terrain found for '" + terrainName + "'");

                terrainDict.Add(b++, t);
            }

            Dictionary<byte, Feature> featureDict = new Dictionary<byte, Feature>();

            b = 0;
            foreach (string featureName in civ5Map.FeatureNames1st)
            {
                Feature f = Provider.Instance.Features.FirstOrDefault(a => a.Value.Civ5Name == featureName).Value;

                if (f == null)
                    throw new Exception("No Feature found for '" + featureName + "'");

                featureDict.Add(b++, f);
            }

            // add hills / mountains
            featureDict.Add(b++, Provider.Instance.Features.FirstOrDefault(a => a.Value.Name == "Hills").Value);
            featureDict.Add(b++, Provider.Instance.Features.FirstOrDefault(a => a.Value.Name == "Mountains").Value);

            for (int x = 0; x < civ5Map.Width; x++)
            {
                for (int y = 0; y < civ5Map.Height; y++)
                {
                    int dx = 0; // y % 2 == 1 ? 1 : 0;
                    int dy = -y + civ5Map.Height - y - 1;

                    byte civ5TerrainId = civ5Map.GetTerrainId(x, y);
                    this[x + dx, y + dy].Terrain = terrainDict[civ5TerrainId];

                    byte civ5FeatureId = civ5Map.GetFeatureId(x, y);

                    if( civ5FeatureId != 255 )
                        this[x + dx, y + dy].Features.Add(featureDict[civ5FeatureId]);

                    civ5FeatureId = civ5Map.GetFeature2ndId(x, y);

                    if (civ5FeatureId != 255)
                        this[x + dx, y + dy].Features.Add(featureDict[civ5FeatureId]);

                    switch (civ5Map.GetHeight(x, y))
                    {
                        case Civ5Map.HeightType.Hills:
                            this[x + dx, y + dy].Features.Add(featureDict.Values.FirstOrDefault(a => a.Name == "Hills"));
                            break;
                        case Civ5Map.HeightType.Mountain:
                            this[x + dx, y + dy].Features.Add(featureDict.Values.FirstOrDefault(a => a.Name == "Mountains"));
                            break;
                    }

                    this[x + dx, y + dy].River = civ5Map.GetRiver(x, y);

                    //this[x + dx, y + dy].Info = this[x + dx, y + dy].River + "=>" + this[x + dx, y + dy].RiverFlowString;
                }
            }

            // check coast
            Terrain ocean = Provider.Instance.Terrains.FirstOrDefault(a => a.Value.Name == "Ocean").Value;
            Terrain coast = Provider.Instance.Terrains.FirstOrDefault(a => a.Value.Name == "Coast").Value;
            foreach (MapCell cell in Tiles)
            {
                if (cell.Terrain.Name == "Coast" && !ShouldBeCoast(cell.X, cell.Y))
                    cell.Terrain = ocean;

                if (cell.Terrain.Name == "Ocean" && ShouldBeCoast(cell.X, cell.Y))
                    cell.Terrain = coast;
            }
        }
    }
}
