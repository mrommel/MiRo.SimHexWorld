using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class InfoPediaDialog : AssetWindow
    {
        public InfoPediaDialog(Manager manager)
            : base(manager, "Content//Controls//InfoPediaDialog")
        { 
        }

        private AbstractNamedEntity _focus = null;
        public AbstractNamedEntity Focus
        {
            get { return _focus; }
            set
            {
                _focus = value;
                _engine.Invoke("window", "Focused", this, _focus);
            }
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
