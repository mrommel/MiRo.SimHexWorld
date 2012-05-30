﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using PureMVC.Interfaces;
using MiRo.SimHexWorld.Engine.Instance;

namespace MiRo.SimHexWorld.Engine.UI
{
    public abstract class GameWindow : Window, IMediator
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

        public GameWindow(TomShane.Neoforce.Controls.Manager manager)
            : base(manager)
        {
        }

        public abstract void HandleNotification(INotification notification);
    }
}
