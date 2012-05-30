using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Instance;
using PureMVC.Interfaces;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI
{
    public abstract class GameControl : Control, IMediator
    {
        public abstract List<GameNotification> NotificationInterests { get; }

        public IList<string> ListNotificationInterests()
        {
            List<string> list = new List<string>();

            foreach (GameNotification notification in NotificationInterests)
                list.Add(notification.ToString());

            return list;
        }

        public string MediatorName
        {
            get { return ToString(); }
        }

        public void OnRegister()
        {
            
        }

        public void OnRemove()
        {

        }

        public object ViewComponent
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public GameControl(TomShane.Neoforce.Controls.Manager manager)
            : base(manager)
        {
        }

        public abstract void HandleNotification(INotification notification);
    }
}
