using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class ControlSet : List<ControlItem>
    {
        public void Init(Window parent, Manager manager)
        {
            foreach (ControlItem item in this)
            {
                item.Init(parent, manager);
            }
        } 
    }    
}
