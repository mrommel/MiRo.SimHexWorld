using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class Style
    {
        public Style()
        {
        }

        public string Id { get; set; }

        [ContentSerializerAttribute(Optional = true)]
        public string Grid9 { get; set; }

        [ContentSerializerAttribute(Optional = true)]
        public string Grid9Hover { get; set; }

        [ContentSerializerAttribute(Optional = true)]
        public string Grid9Disabled { get; set; }
    }
}
