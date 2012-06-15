using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using PureMVC.Interfaces;
using MiRo.SimHexWorld.Engine.Types;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class ScienceDialog : AssetWindow
    {
        Texture2D _activeTexture;

        public ScienceDialog(Manager manager)
            : base(manager, "Content//Controls//ScienceDialog")
        {
            Init();

            _activeTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//ScienceView//TechActive");

            foreach (Tech tech in MainWindow.Game.Human.Technologies)
            {
                Control c = GetControl(tech.Name + "Background");
                if (c != null)
                {
                    ImageBox box = c as ImageBox;
                    box.Image = _activeTexture;
                }
            }
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { return new List<Instance.GameNotification>(); }
        }

        public override void HandleNotification(INotification notification)
        {
            
        }
    }
}
