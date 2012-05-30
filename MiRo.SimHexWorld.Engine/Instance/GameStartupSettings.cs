using System;
using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;
using log4net;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class GameStartupSettings
    {
        readonly ILog _log = LogManager.GetLogger(typeof(GameStartupSettings));

        public string PlayerNumber { get; set; }

        #region map type
        public string MapType { get; set; }

        public int PlayersPreset
        {
            get 
            {
                try
                {
                    KeyValuePair<string, MapSize> mapSizePair = Provider.Instance.MapSizes.FirstOrDefault(a => a.Key == MapSize);

                    return mapSizePair.Value.Players;
                }
                catch (Exception ex)
                {
                    _log.Error("No Players from: " + MapSize, ex);
                    return 20;
                }
            }
        }

        public static List<string> MapTypeNames
        {
            get
            {
                return Provider.Instance.MapTypes.Keys.ToList();
            }
        }

        #endregion

        public string CivilizationName { get; set; }

        public static List<string> CivilizationNames
        {
            get
            {
                return Provider.Instance.Civilizations.Keys.ToList();
            }
        }

        public string Difficulty { get; set; }

        #region map sizes
        public string MapSize { get; set; }

        public static List<string> MapSizeNames
        {
            get
            {
                return Provider.Instance.MapSizes.Keys.ToList();
            }
        }

        public int Width
        {
            get
            {
                try
                {
                    KeyValuePair<string, MapSize> mapSizePair = Provider.Instance.MapSizes.FirstOrDefault(a => a.Key == MapSize);

                    return mapSizePair.Value.Size.Width;
                }
                catch (Exception ex)
                {
                    _log.Error("No Width from: " + MapSize, ex);
                    return 20;
                }
            }
        }

        public int Height
        {
            get
            {
                try
                {
                    KeyValuePair<string, MapSize> mapSizePair = Provider.Instance.MapSizes.FirstOrDefault(a => a.Key == MapSize);

                    return mapSizePair.Value.Size.Height;
                }
                catch (Exception ex)
                {
                    _log.Error("No Height from: " + MapSize, ex);
                    return 20;
                }
            }
        }

        public int NumOfRivers
        {
            get
            {
                try
                {
                    KeyValuePair<string, MapSize> mapSizePair = Provider.Instance.MapSizes.FirstOrDefault(a => a.Key == MapSize);

                    return mapSizePair.Value.NumOfRivers;
                }
                catch (Exception ex)
                {
                    _log.Error("No NumOfRivers from: " + MapSize, ex);
                    return 20;
                }
            }
        }

        public float OceanRatio
        {
            get
            {
                try
                {
                    KeyValuePair<string, MapType> mapTypePair = Provider.Instance.MapTypes.FirstOrDefault(a => a.Key == MapType);

                    return mapTypePair.Value.OceanRatio;
                }
                catch (Exception ex)
                {
                    _log.Error("No OceanRatio from: " + MapType, ex);
                    return 0.3f;
                }
            }
        }
        #endregion map sizes

        public float PeakRatio
        {
            get
            {
                return 0.095f;
            }
        }
    }
}