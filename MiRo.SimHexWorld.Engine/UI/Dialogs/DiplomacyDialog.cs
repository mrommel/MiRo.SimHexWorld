using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public abstract class AssetWindow : GameWindow 
    {
        public AssetWindow(Manager manager, string assetName)
            : base(manager)
        {
            LoadWindow(assetName);
        }
    }

    public class DiplomacyDialog : AssetWindow
    {
        public DiplomacyDialog(Manager manager)
            : base(manager, "Content//Controls//DiplomacyDialog")
        {
            manager.Add(this);
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { return new List<Instance.GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            
        }
    }
}
