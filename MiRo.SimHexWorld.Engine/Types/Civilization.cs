using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Civilization : AbstractNamedEntity
    {
        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                return Provider.GetAtlas("CivilizationAtlas").GetTexture(ImageName);
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return ""; }
        }

        public string PlayerColorName { get; set; }

        private PlayerColor _color;

        [ContentSerializerIgnore]
        public PlayerColor PlayerColor
        { 
            get 
            { 
                if( _color == null )
                    _color = MainWindow.Game.PlayerColors.FirstOrDefault(a => a.Name == PlayerColorName);

                if (_color == null)
                    throw new Exception("Color: " + PlayerColorName + " not found in Colors definition");

                return _color;
            } 
        }

        public List<string> UniqueUnits { get; set; }
        public List<string> UniqueBuildings { get; set; }
        public List<string> StartingTechNames { get; set; }

        [ContentSerializerIgnore]
        public List<Tech> StartingTechs
        {
            get
            {
                List<Tech> techs = new List<Tech>();

                foreach (string techname in StartingTechNames)
                {
                    Tech tech = Provider.GetTech(techname);

                    if( tech != null )
                        techs.Add(tech);
                }
 
                return techs;
            }
        }

        public string Duration { get; set; }
        public string Location { get; set; }
        public string Size { get; set; }

        public string Capital { get; set; }
        public List<string> Cities { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImageName));

            foreach(string unitname in UniqueUnits)
                if (!Provider.Instance.Units.ContainsKey(unitname))
                    result.Add(new MissingAsset(this, "Unit", unitname));

            foreach (string buildingname in UniqueBuildings)
                if (!Provider.Instance.Buildings.ContainsKey(buildingname))
                    result.Add(new MissingAsset(this, "Building", buildingname));

            foreach (string techname in StartingTechNames)
                if (!Provider.Instance.Techs.ContainsKey(techname))
                    result.Add(new MissingAsset(this, "Technology", techname));

            return result;
        }
    }
}
