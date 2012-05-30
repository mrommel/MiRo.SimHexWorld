using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Translation
    {
        public class TranslationItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public string Locale { get; set; }
        public List<TranslationItem> Items { get; set; }
    }
}
