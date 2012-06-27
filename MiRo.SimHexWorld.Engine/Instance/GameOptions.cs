using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Helper;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class GameOptions
    {
        public bool HideRecommondations { get; set; }
        public bool ResourceIcons { get; set; }
        public bool YieldIcons { get; set; }
        public bool HexGrid { get; set; }
        public bool UnitIcons { get; set; }
        public bool UnitPromotions { get; set; }

        private XmlSerializerHelper<GameOptions> serializer = new XmlSerializerHelper<GameOptions>();

        public void Load()
        {
            try
            {
                GameOptions opt = serializer.Read("config.xml");

                HideRecommondations = opt.HideRecommondations;
                ResourceIcons = opt.ResourceIcons;
                YieldIcons = opt.YieldIcons;
                HexGrid = opt.HexGrid;
                UnitIcons = opt.UnitIcons;
                UnitPromotions = opt.UnitPromotions;
            }
            catch { }
        }

        public void Save()
        {
            serializer.Save("config.xml", this);
        }
    }
}
