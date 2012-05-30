using System;
using MiRo.SimHexWorld.Engine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.Instance.Commands
{
    public class MapCreateCommand : SimpleCommand
    {
        MapData _map;

        public override void Execute(INotification note)
        {
            try
            {
                _map = new MapData();
                _map.Create(note.Body as GameStartupSettings);

                _map.ProgressNotificationChanged += _map_ProgressNotificationChanged;

                GameFacade.getInstance().SendNotification(GameNotification.CreateMapSuccess, _map);
            }
            catch (Exception ex)
            {
                GameFacade.getInstance().SendNotification(GameNotification.Exception, ex);
            }
        }

        void _map_ProgressNotificationChanged(object sender, ProgressNotificationEventArgs e)
        {
            MainApplication.Instance.MainWindow.Text = string.Format("{0} ({1} of {2})", e.DetailName, e.DetailProgress, e.OverallProgress);
        }
    }
}
