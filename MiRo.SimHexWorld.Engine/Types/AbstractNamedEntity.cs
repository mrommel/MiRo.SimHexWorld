using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Screens;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.UI.Data;

namespace MiRo.SimHexWorld.Engine.Types
{
    public abstract class AbstractNamedEntity
    {
        public AbstractNamedEntity() { }

        public AbstractNamedEntity(string name, string desc)
        {
            Name = name;
            _description = desc;
        }

        public string Name { get; set; }

        private string _title = null;

        [ContentSerializer(Optional = true)]
        public string Title 
        {
            get
            {
                if (!string.IsNullOrEmpty(_title))
                {
                    if (_title.Trim().StartsWith("TXT_KEY_") && Provider.CanTranslate)
                        return Provider.Instance.Translate(_title.Trim());
                    else
                        return _title;
                }
                else
                    return Name;
            }
            set
            {
                _title = value.Trim();
            }
        }

        private string _description;
        public string Description 
        {
            get
            {
                if (!string.IsNullOrEmpty(_description))
                {
                    if (_description.StartsWith("TXT_KEY_") && Provider.CanTranslate)
                        return Provider.Instance.Translate(_description.Trim());
                    else
                    {
                        string[] lines = _description.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < lines.Length; ++i)
                            lines[i] = lines[i].Trim();

                        return string.Join(Environment.NewLine, lines);
                    }
                }
                else
                    return "";
            }
            set
            {
                _description = value.Trim();
            }
        }

        public string ImageName { get; set; }

        [ContentSerializerIgnore]
        public abstract string ImagePath { get; }

        [ContentSerializerIgnore]
        public abstract Texture2D Image { get; }

        public abstract List<MissingAsset> CheckIntegrity();

        public static string LimitTextLength(string text, int lengthInChars)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();

            foreach (string word in words)
                if (sb.ToString().Length + 1 + word.Length > lengthInChars)
                    return sb.ToString() + "...";
                else
                    sb.Append(word + ' ');

             return sb.ToString();
        }

        public TeaserList.TeaserItem ToTeaser()
        {
            Name = Name.Trim();
            Description = Description.Trim();

            string desc = LimitTextLength(Description,100);

            return new TeaserList.TeaserItem(Title, desc, ImagePath);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
