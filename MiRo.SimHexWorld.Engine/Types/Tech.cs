using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using System;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Tech : AbstractNamedEntity
    {
        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                try
                {
                    if (_image == null)
                        _image = Provider.GetAtlas("TechnologyAtlas").GetTexture(ImageName);
                }
                catch { }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return "Content/Textures/Techs/" + ImageName; }
        }

        public string Quote { get; set; }
        public int Cost { get; set; }
        public string Notes { get; set; }

        public List<string> Required { get; set; }

        [ContentSerializerIgnore]
        public List<Tech> RequiredTech
        {
            get 
            {
                List<Tech> list = new List<Tech>();

                foreach (string req in Required)
                {
                    try
                    {
                        list.Add(Provider.Instance.Techs[req]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Name + " => " + req + " not found");
                    }
                }

                return list;
            }
        }

        public List<string> RequiredOr { get; set; }

        [ContentSerializerIgnore]
        public List<Tech> RequiredOrTech
        {
            get
            {
                List<Tech> list = new List<Tech>();

                foreach (string req in RequiredOr)
                    list.Add(Provider.Instance.Techs[req]);

                return list;
            }
        }
 
        public string EraName { get; set; }

        [ContentSerializerIgnore]
        public Era Era
        {
            get
            {
                if( Provider.Instance.Eras.ContainsKey( EraName ) )
                    return Provider.Instance.Eras[EraName];

                return null;
            }
        }

        public List<Flavour> Flavours { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add( new MissingAsset( this, "Image", ImageName ));

            if (Era == null)
                result.Add(new MissingAsset(this, "Era", EraName));

            foreach (string req in Required)
                if (!Provider.Instance.Techs.ContainsKey(req))
                    result.Add(new MissingAsset(this, "Technology", req));

            foreach (string req in RequiredOr)
                if (!Provider.Instance.Techs.ContainsKey(req))
                    result.Add(new MissingAsset(this, "Technology (OR)", req));

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

