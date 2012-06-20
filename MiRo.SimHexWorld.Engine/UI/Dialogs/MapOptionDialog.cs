using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class MapOptionDialog : AssetWindow
    {
        public MapOptionDialog(Manager manager)
            : base(manager, "Content//Controls//MapOptionDialog")
        { 
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { return new List<Instance.GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
