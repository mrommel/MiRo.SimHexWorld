using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Era : AbstractNamedEntity
    {
        public string Successor { get; set; }

        private Texture2D _image;

        public float GrowthModifier { get; set; }

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                if (_image == null)
                {
                    try
                    {
                        _image = MainApplication.ManagerInstance.Content.Load<Texture2D>(ImagePath);
                    }
                    catch { }
                }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public Era SuccessorEra
        {
            get
            {
                if (string.IsNullOrEmpty(Successor))
                    return null;

                if( Provider.Instance.Eras.ContainsKey(Successor) )
                    return Provider.Instance.Eras[Successor];

                return null;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get
            {
                return "Content/Textures/Eras/" + ImageName;
            }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            var result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImagePath));

            if (!string.IsNullOrEmpty(Successor) && !Provider.Instance.Eras.ContainsKey(Successor))
                result.Add(new MissingAsset(this, "Era", Successor));

            return result;
        }

        private static List<Era> _eras = new List<Era>();

        public static bool operator >(Era e1, Era e2)
        {
            if (_eras.Count == 0)
            {
                foreach (Era e in Provider.Instance.Eras.Values)
                    if (e.SuccessorEra != null && _eras.Contains(e.SuccessorEra))
                        _eras.Insert(_eras.FindIndex(a => a.Name == e.SuccessorEra.Name),e);
                    else
                        _eras.Add(e);
            }

            return _eras.FindIndex(a => a.Name == e1.Name) - _eras.FindIndex(a => a.Name == e2.Name) > 0;
        }

        public static bool operator ==(Era e1, Era e2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(e1, e2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)e1 == null) || ((object)e2 == null))
            {
                return false;
            }

            return e1.Name == e2.Name;
        }

        public static bool operator !=(Era e1, Era e2)
        {
            return !(e1 == e2);
        }

        public static bool operator <(Era e1, Era e2)
        {
            return !(e1 > e2 || e1 == e2);  
        }
    }
}
