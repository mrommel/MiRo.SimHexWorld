using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public enum GameNotification
    {
        /// <summary>
        /// triggered if map should be created
        /// body: GameStartupSettings
        /// </summary>
        CreateMap,
        /// <summary>
        /// triggered from CreateMap if creation was successful
        /// </summary>
        CreateMapSuccess,
        /// <summary>
        /// triggered to load an civ5 map
        /// body: filename (string)
        /// </summary>
        LoadMap,
        /// <summary>
        /// triggered from LoadMap when the loading was successful
        /// </summary>
        LoadMapSuccess,
        /// <summary>
        /// an error occured
        /// body: Exception
        /// </summary>
        Exception,
        Message,
        UpdateImprovements,
        UpdateSpotting,
        UpdateMapControlling,
        UpdateMapExploiting,
        ShowScoreHistory
    }
}
