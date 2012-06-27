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
    public class ScienceReadyDialog : AssetWindow
    {
        public ScienceReadyDialog(Manager manager)
            : base(manager, "Content//Controls//ScienceReadyDialog")
        {
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
