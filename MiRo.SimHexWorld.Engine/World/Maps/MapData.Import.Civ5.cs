using System;
using System.Linq;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;
using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.Instance.AI;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    partial class MapData
    {
        public void InitFromCiv5Map(Civ5Map civ5Map)
        {
            Init(civ5Map.Width + 1, civ5Map.Height + 1);

            Dictionary<byte, Terrain> terrainDict = new Dictionary<byte, Terrain>();

            byte b = 0;
            foreach (string terrainName in civ5Map.TerrainNames)
            {
                Terrain t = Provider.Instance.Terrains.FirstOrDefault(a => a.Value.Civ5Name == terrainName).Value;

                if (t == null)
                    throw new Exception("No Terrain found for '" + terrainName + "'");

                terrainDict.Add(b++, t);
            }

            Dictionary<byte, Ressource> ressourceDict = new Dictionary<byte, Ressource>();

            b = 0;
            foreach (string ressourceName in civ5Map.RessourceNames)
            {
                Ressource r = Provider.Instance.Ressources.FirstOrDefault(a => a.Value.Civ5Name == ressourceName).Value;

                if (r == null)
                    throw new Exception("No Ressource found for '" + ressourceName + "'");

                ressourceDict.Add(b++, r);
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

                    byte civ5RessourceId = civ5Map.GetResourceId(x, y);

                    if (civ5RessourceId != 255)
                        this[x + dx, y + dy].SetRessources(ressourceDict[civ5RessourceId]);
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

            //PatchRivers();
        
            // if there are no resources applied, do this
            if (_tiles.Count(a => a.Ressource != null ) == 0)
                MakeResources();
         }   

        private void PatchRivers()
        {
            // fill base data
            List<FlowPoint> riverParts = new List<FlowPoint>();
         
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    MapCell.FlowDirectionType fd = this[x, y].WOfRiver;

                    if (fd != MapCell.FlowDirectionType.NoFlowdirection)
                        riverParts.Add(new FlowPoint(new HexPoint(x, y), fd));

                    fd = this[x, y].NEOfRiver;

                    if (fd != MapCell.FlowDirectionType.NoFlowdirection)
                        riverParts.Add(new FlowPoint(new HexPoint(x, y), fd));

                    fd = this[x, y].NWOfRiver;

                    if (fd != MapCell.FlowDirectionType.NoFlowdirection)
                        riverParts.Add(new FlowPoint(new HexPoint(x, y), fd));
                }
            }

            // create river graphs
            List<River> rivers = new List<River>();
            int riverNum = 0;

            foreach (FlowPoint fp in riverParts)
            {
                River r = rivers.FirstOrDefault(a => a.IsConnected(fp));

                if (r == null)
                {
                    River newRiver = new River("River " + (riverNum++));
                    newRiver.Points.Add(fp);
                    rivers.Add(newRiver);
                }
                else 
                {
                    r.Points.Add(fp);
                }
            }

            // join rivers
            bool riversNeedJoin = true;
            while (riversNeedJoin)
            {
                River r1, r2;
                riversNeedJoin = FindRiversToJoin(rivers, out r1, out r2 );

                if (riversNeedJoin)
                {
                    r1.Join(r2);
                    rivers.Remove(r2);
                }
            }

            // remove strange flows
            foreach (River r in rivers)
                r.Clean();

            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Indent = true;

            //using (XmlWriter writer = XmlWriter.Create("river.xml", settings))
            //{
            //    IntermediateSerializer.Serialize(writer, rivers, null);
            //}

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    this[x, y].River = 0;

            foreach (River r in rivers)
            {
                if (r.Length > 1)
                {
                    foreach (FlowPoint fp in r.Points)
                        this[fp.Point].SetRiver(fp.Flow);
                }
            }
        }

        private bool FindRiversToJoin(List<River> rivers, out River r1, out River r2)
        {
            r1 = null;
            r2 = null;

            foreach (River r in rivers)
            {
                foreach (River rInner in rivers)
                {
                    if (ReferenceEquals(r, rInner))
                        continue;

                    if (r.IsConnected(rInner))
                    {
                        r1 = r;
                        r2 = rInner;
                        return true;
                    }
                }
            }

            return false;
        }

        private static Random rnd = new Random();
        private void MakeResources()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    List<Ressource> possibleResources = new List<Ressource>();

                    MapCell cell = this[x, y];

                    foreach (string resName in cell.Terrain.PossibleRessources)
                        possibleResources.Add(Provider.GetRessource(resName));

                    foreach (Feature feature in cell.Features)
                        foreach (string resName in feature.PossibleRessources)
                            possibleResources.Add(Provider.GetRessource(resName));

                    List<MapRegion> regions = this.GetRegions(x, y);

                    foreach (MapRegion mr in regions)
                    {
                        foreach (string resName in mr.RessourceExcludes)
                            if (possibleResources.Count( a => a.Name == resName ) > 0 )
                                possibleResources.RemoveAll(a => a.Name == resName);
                    }

                    // if we have any possible there is a 20% chance to place a resource
                    if (possibleResources.Count > 0 && rnd.NextDouble() < 0.2)
                    {
                        PropabilityMap<Ressource> map = new PropabilityMap<Ressource>();

                        foreach (Ressource r in possibleResources)
                            map.AddItem(r, 1);

                        cell.SetRessources(map.Random);
                    }
                }
            }
        }
    }
}
