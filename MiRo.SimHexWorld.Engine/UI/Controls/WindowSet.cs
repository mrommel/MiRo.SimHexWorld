using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class WindowSet
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ControlSet Controls { get; set; }

        public void Init(GameWindow gameWindow, Manager manager)
        {
            gameWindow.Text = Title;
            gameWindow.Name = Name;
            gameWindow.Icon = manager.Content.Load<Texture2D>(Icon);

            gameWindow.Left = Left;
            gameWindow.Top = Top;
            gameWindow.Width = Width > 0 ? Width : GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            gameWindow.Height = Height > 0 ? Height : GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Controls.Init(gameWindow, manager);
        }
    }
}
