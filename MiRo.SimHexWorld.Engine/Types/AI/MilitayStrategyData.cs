using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class MilitayStrategyData : AbstractNamedEntity
    {
        public override string ImagePath
        {
            get { return ""; }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

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

        public int CheckTriggerTurnCount = 0;

        [ContentSerializer(Optional = true)]
        public int FirstTurnExecuted = 0;
        public int MinimumNumTurnsExecuted = 0;

        [ContentSerializer(Optional = true)]
        public int WeightThreshold = 0;

        [ContentSerializer(Optional = true)]
        public bool UpdateCitySpecializations = false;

        [ContentSerializer(Optional = true)]
        public bool DontUpdateCityFlavors = false;

        [ContentSerializer(Optional = true)]
        public bool NoMinorCivs = false;

        [ContentSerializer(Optional = true)]
        public bool OnlyMinorCivs = false;

        [ContentSerializer(Optional = true)]
        public List<Flavour> Flavours;
    }
}
