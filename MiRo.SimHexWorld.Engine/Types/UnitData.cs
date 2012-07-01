using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public enum UnitClass
    { 
        Armored, 
        Gunpowder, 
        Archery, 
        Naval, 
        Civilian,
        Melee, 
        Recon 
    };

    public enum MoveRateType
    { 
        WoodenBoat,
        Boat,
        Wheeled,
        BiPed
    };

    public enum DomainType
    {
        Land, 
        Sea
    }

    public enum UnitAI
    {
        Work,
        Settle,
        Explore,
        Attack,
        FastAttack,
        Defense,
        Counter
    }

    public enum UnitAction
    {
        Idle,
        Move,
        Found,
        Explore,
        AttackCity,
        AttackUnit,
        BuildRoad,
        BuildFarm
    }

    public class UnitData : AbstractNamedEntity, IProductionTarget
    {
        public class UnitAction
        {
            [ContentSerializerIgnore]
            public Improvement Improvement
            {
                get
                {
                    if (Provider.Instance.Improvements.ContainsKey(Name))
                        return Provider.Instance.Improvements[Name];

                    return null;
                }
            }

            public string Name { get; set; }
        }

        public UnitData()
        {
            MoveRateType = MoveRateType.BiPed;
        }
        
        public string Notes { get; set; }
        public string EraName { get; set; }

        [ContentSerializerIgnore]
        public Era Era
        {
            get 
            {
                if( Provider.Instance.Eras.ContainsKey(EraName) )
                    return Provider.Instance.Eras[EraName];

                return null;
            }
        }

        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                if (_image == null)
                {
                    try
                    {
                        _image = Provider.GetAtlas("UnitAtlas").GetTexture(ImageName);
                    }
                    catch { }
                }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get 
            {
                return "";
            }
        }

        public UnitClass UnitType { get; set; }

        public bool IsSpecial { get; set; }

        [ContentSerializer(Optional = true)]
        public string UniqueUnitOfCivilization { get; set; } // if IsSpecial is True

        [ContentSerializer(Optional = true)]
        public string Replaces { get; set; } // if IsSpecial is True

        public int Cost { get; set; }
        public int CombatStrength { get; set; }
        public int RangedCombatStrength { get; set; }
        public int Range { get; set; }
        public int Sight { get; set; }
        public int Movement { get; set; }

        [ContentSerializer(Optional = true)]
        public MoveRateType MoveRateType { get; set; }

        public DomainType Domain { get; set; }
        public List<UnitAI> UnitAIs { get; set; }
        public UnitAI DefaultUnitAI { get; set; }

        public bool NoBadGoodies { get; set; }
        public bool MilitarySupport { get; set; }
        public bool MilitaryProduction { get; set; }
        public bool Pillage { get; set; }
        public bool IgnoreBuildingDefense { get; set; }
        public bool Mechanized { get; set; }

        [ContentSerializer(Optional = true)]
        public List<string> RequiredRessourceNames { get; set; }

        [ContentSerializerIgnore]
        public List<Ressource> RequiredRessources 
        {
            get
            {
                List<Ressource> rs = new List<Ressource>();

                if (RequiredRessourceNames != null)
                {
                    foreach (string str in RequiredRessourceNames)
                        rs.Add(Provider.GetRessource(str));
                }

                return rs;
            }
        }

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

        public string ObsoleteTechName { get; set; }

        [ContentSerializerIgnore]
        public Tech ObsoleteTech
        {
            get
            {
                if (string.IsNullOrEmpty(ObsoleteTechName))
                    return null;

                return Provider.GetTech(ObsoleteTechName);
            }
        }

        public string UpgradesTo { get; set; }

        [ContentSerializer(Optional = true)]
        public Flavours Flavours { get; set; }

        [ContentSerializer(Optional = true)]
        public List<UnitAction> Actions { get; set; }

        [ContentSerializer(Optional = true)]
        public bool CanFound { get; set; }

        public List<string> PromotionNames { get; set; }

        public string Formation { get; set; }

        public string ModelName { get; set; }
        public float ModelScale { get; set; }
        public string ModelScript { get; set; }

        [ContentSerializer(Optional = true)]
        public float ModelRotation { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            //if (Image == null)
            //    result.Add(new MissingAsset(this, "Image", ImagePath));
            if (!Provider.GetAtlas("UnitAtlas").HasIdentifier(ImageName))
                result.Add(new MissingAsset(this, "AtlasEntry in 'UnitAtlas'", ImageName));

            if(! Provider.Instance.Eras.ContainsKey(EraName))
                result.Add(new MissingAsset(this, "Era", EraName));

            // check special units
            if (IsSpecial && string.IsNullOrEmpty(UniqueUnitOfCivilization))
                result.Add(new MissingAsset(this, "Special Unit empty", ""));

            if (IsSpecial && !Provider.Instance.Civilizations.ContainsKey(UniqueUnitOfCivilization))
                result.Add(new MissingAsset(this, "Civilization", UniqueUnitOfCivilization));

            if (IsSpecial && string.IsNullOrEmpty(Replaces))
                result.Add(new MissingAsset(this, "Replace Unit empty", ""));

            if (IsSpecial && !Provider.Instance.Units.ContainsKey(Replaces))
                result.Add(new MissingAsset(this, "Unit", Replaces));

            // check ressources
            foreach( string reqRes in RequiredRessourceNames )
                if( !Provider.Instance.Ressources.ContainsKey(reqRes) )
                    result.Add(new MissingAsset(this, "Ressource", reqRes));

            // check technology
            if (!string.IsNullOrEmpty(RequiredTechName) && !Provider.Instance.Techs.ContainsKey(RequiredTechName))
                result.Add(new MissingAsset(this, "Technology (Required)", RequiredTechName));
            
            if (!string.IsNullOrEmpty(ObsoleteTechName) && !Provider.Instance.Techs.ContainsKey(ObsoleteTechName))
                result.Add(new MissingAsset(this, "Technology (Obsolete)", ObsoleteTechName));

            if( !string.IsNullOrEmpty(UpgradesTo) && !Provider.Instance.Units.ContainsKey(UpgradesTo) )
                result.Add(new MissingAsset(this, "Unit", UpgradesTo));

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

            // check promotions
            foreach (string promotionStr in PromotionNames)
            {
                if (!Provider.Instance.Promotions.ContainsKey(promotionStr))
                    result.Add(new MissingAsset(this, "Promotion", promotionStr));
                else
                {
                    PromotionData promotion = Provider.Instance.Promotions[promotionStr];

                    if (promotion.Types != null && promotion.Types.Count > 0)
                    {
                        if( !promotion.Types.Contains( this.UnitType ) )
                            result.Add(new MissingAsset(this, "Promotion cannot be assigned to unit: " + this.Name, promotionStr));
                    }
                }
            }

            // check actions
            if (Actions != null)
            {
                foreach (UnitAction f in Actions)
                {
                    if (f.Improvement == null)
                        result.Add(new MissingAsset(this, "Improvement", f.Name));
                }
            }

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
