using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;
using MiRo.SimHexWorld.Engine.Types.AI;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public enum SpecialistType { Scientist, Merchant };

    public interface IProductionTarget
    { 
        int Cost { get;  } 
    }

    public class Building : AbstractNamedEntity, IProductionTarget
    {
        public int Cost { get; set; }

        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                try
                {
                    if (_image == null)
                        _image = Provider.GetAtlas("BuildingAtlas").GetTexture(ImageName);
                }
                catch { }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return ""; }
        }

        public int Maintenance { get; set; }

        public string RequiredTechName { get; set; }

        [ContentSerializerIgnore]
        public Tech RequiredTech
        {
            get
            {
                if (string.IsNullOrEmpty(RequiredTechName))
                    return null;

                return Provider.GetTech(RequiredTechName);
            }
        }

        [ContentSerializer(Optional = true)]
        public SpecialistType SpecialistType { get; set; }

        [ContentSerializer(Optional = true)]
        public int SpecialistSlots { get; set; }

        [ContentSerializer(Optional = true)]
        public bool AllowsRangeStrike { get; set; }

        [ContentSerializer(Optional = true)]
        public int Defense { get; set; }

        [ContentSerializer(Optional = true)]
        public bool CityWall { get; set; }

        [ContentSerializer(Optional = true)]
        public int Culture { get; set; }

        [ContentSerializer(Optional = true)]
        public bool NeverCapture { get; set; }

        [ContentSerializer(Optional = true)]
        public int Happiness { get; set; }

        [ContentSerializer(Optional = true)]
        public bool InCapitalOnly { get; set; }

        [ContentSerializer(Optional = true)]
        public float ConquestProb { get; set; }

        [ContentSerializer(Optional = true)]
        public float HurryCostModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsSpecial { get; set; }

        [ContentSerializer(Optional = true)]
        public string UniqueUnitOfCivilization { get; set; } // if IsSpecial is True

        public Flavours Flavours { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Yield> Yields { get; set; }

        [ContentSerializer(Optional = true)]
        public float SciencePerCitizen { get; set; }

        [ContentSerializer(Optional = true)]
        public float ScienceModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public float ProductionModifier { get; set; }

        [ContentSerializerIgnore]
        public int YieldsFood 
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Food);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsScience
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Science);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsProduction
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Production);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializer(Optional = true)]
        public List<string> RequiredRessourceNames { get; set; }

        [ContentSerializer(Optional = true)]
        public bool NeedRiver { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImageName));

            // check flavours
            if (Flavours != null)
            {
                foreach (Flavour f in Flavours)
                {
                    if (f.Data == null)
                        result.Add(new MissingAsset(this, "Flavour", f.Name));
                }
            }
            else
                result.Add(new MissingAsset(this, "Flavour (no Flavours)", ""));

            return result;
        }
    }
}
