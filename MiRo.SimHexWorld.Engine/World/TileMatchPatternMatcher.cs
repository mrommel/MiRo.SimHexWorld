using System;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.World.Maps;

namespace MiRo.SimHexWorld.Engine.World
{
    public class TileMatchPatternMatcher
    {
        readonly NamedList<TileMatchPattern> _pattern;

        public TileMatchPatternMatcher(NamedList<TileMatchPattern> pattern)
        {
            _pattern = pattern;
        }

        private int Match(TileMatchPattern pattern, MapData map, MapCell cell)
        {
            int result;
            bool reset = false;

            if (map != null && cell != null && map.IsValid(cell.Point))
            {
                result = CalcScore(pattern.TerrainName, cell) * 8;

                if (result > 0)
                {
                    int i = 0;
                    foreach (HexDirection dir in HexDirection.All)
                    {
                        HexPoint pt = cell.Point;
                        pt = pt.Neighbor(dir);

                        if (map.IsValid(pt)) // on map board, calc score 0-3
                        {
                            int score = CalcScore(pattern.TerrainNameAdjacent[i], map[pt]);

                            if (score == 0)
                                reset = true;
                            else
                                result += score;
                        }
                        else if (pattern.TerrainNameAdjacent[i] == "*" || pattern.TerrainNameAdjacent[i] == "IsOcean") // not at map board but *
                            result += 1;
                        else // no match at all
                            reset = true;

                        i++;
                    }
                }
            }
            else
                return -1;

            return (reset) ? 0 : result;
        }

        private int CalcScore(string terrainNameAdjacent, MapCell mapCell)
        {
            switch (terrainNameAdjacent)
            {
                // terrain 
                case "Ocean":
                case "Shore":
                case "Coast":
                case "Grassland":
                case "Plains":
                case "Desert":
                case "Tundra":
                case "Snow":
                    if (terrainNameAdjacent == mapCell.Terrain.Name)
                        return 3;
                    break;
                // features
                case "Oasis":
                case "Jungle":
                case "Forest":
                case "Hills":
                case "Mountains":
                    if (mapCell.FeatureStr.ToLower().Contains(terrainNameAdjacent.ToLower()))
                        return 3;
                    break;                
                case "IsLand":
                    if (mapCell.IsLand)
                        return 2;
                    break;
                case "IsOcean":
                    if (mapCell.IsOcean)
                        return 2;
                    break;
                case "IsRiver":
                    if (mapCell.IsRiver)
                        return 2;
                    break;
                case "IsRiver||IsCoast":
                    if (mapCell.IsRiver || mapCell.IsCoast)
                        return 2;
                    break;
                case "*":
                    return 1;
                default:               
                    if (terrainNameAdjacent.StartsWith("River_"))
                    {
                        if (!mapCell.IsRiver)
                            break;

                        byte riverNum = byte.Parse(terrainNameAdjacent.Replace("River_", ""));

                        if (mapCell.RiverTileValue == riverNum)
                            return 3;

                        break;
                    }
                    else
                        throw new Exception(string.Format("Unkown Property: '{0}'", terrainNameAdjacent));
            }

            return 0;
        }

        public static byte ShiftRight(byte value, int shiftBy)
        {
            byte val = value;

            for (int i = 0; i < shiftBy; ++i)
                val = ShiftRight(val);

            return val;
        }

        public static byte ShiftRight(byte value)
        {
            value += (byte)((value % 2) * (byte)Math.Pow(2, 6));
            return (byte)((value >> 1) & 63);
        }

        public TileMatchPattern Match(MapData map, MapCell cell)
        {
            TileMatchPattern match = null;
            int bestScore = int.MinValue;

            foreach (TileMatchPattern pattern in _pattern)
            {
                int score = Match(pattern, map, cell);

                if (score > bestScore)
                {
                    bestScore = score;
                    match = pattern;
                }
            }

            if (match == null)
                throw new Exception("No default TileMatchPattern found!");

            match.Score = bestScore;

            return match;
        }
    }
}
