using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Patterns;
using MiRo.SimHexWorld.Engine.Instance.Commands;
using PureMVC.Interfaces;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class GameFacade : Facade
    {
        // we aren't allowed to initialize new instances from outside this class
        protected GameFacade() { }

        // we must specify the type of instance
        static GameFacade()
        {
            m_instance = new GameFacade();
        }

        // Override Singleton Factory method 
        public new static GameFacade getInstance()
        {
            return m_instance as GameFacade;
        }

        // optional initialization hook for Controller
        protected override void InitializeController()
        {
            // call base to use the PureMVC Controller Singleton. 
            base.InitializeController();

            // do any special subclass initialization here
            // such as registering Commands
            RegisterCommand(GameNotification.CreateMap, typeof(MapCreateCommand));
            RegisterCommand(GameNotification.LoadMap, typeof(MapLoadCommand));
            RegisterCommand(GameNotification.Exception, typeof(ExceptionCommand));

            //
            //RegisterCommand(GameNotification.Message, typeof(MessageCommand));
        }

        public void RegisterCommand(GameNotification notification, Type commandType)
        {
            base.RegisterCommand(notification.ToString(), commandType);
        }

        public void SendNotification(GameNotification notification)
        {
            base.SendNotification(notification.ToString());
        }

        public void SendNotification(GameNotification notification, object body)
        {
            base.SendNotification(notification.ToString(),body);
        }

        public void SendNotification(GameNotification notification, object body, object body2 )
        {
            base.SendNotification(notification.ToString(), new List<object> { body, body2 });
        }

        public void SendNotification(GameNotification notification, object body, object body2, object body3)
        {
            base.SendNotification(notification.ToString(), new List<object> { body, body2, body3 });
        }
    }
}
