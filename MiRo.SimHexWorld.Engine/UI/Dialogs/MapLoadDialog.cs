using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.UI.Controls;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class MapLoadDialog : Grid9Window
    {
        ////////////////////////////////////////////////////////////////////////////    
        public MapLoadDialog(Manager manager)
            : base(manager, "Content//Controls//MapLoadDialog")
        {
        }
        ////////////////////////////////////////////////////////////////////////////    

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
