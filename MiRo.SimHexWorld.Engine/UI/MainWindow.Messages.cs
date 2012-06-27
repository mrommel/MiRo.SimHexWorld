using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance;
using PureMVC.Interfaces;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.UI.Dialogs;
using MiRo.SimHexWorld.Engine.Locales;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.UI
{
    [Flags]
    public enum MessageFilter { Self, Friends, Neutral, Enemies };

    partial class MainWindow
    {
        public List<ScreenNotification> Messages = new List<ScreenNotification>();

        SpriteFont _notificationFont;

        Texture2D _notificationiconsproduction, 
            _notificationiconsscience, 
            _notificationgenericglow,
            _notificationcitygrowthglow,
            _notificationcitydeclineglow,
            _notificationiconsculture;

        private void InitMessages()
        {
            _notificationFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");

            _notificationiconsproduction = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationiconsproduction");
            _notificationiconsscience = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationiconsscience");
            _notificationgenericglow = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationgenericglow");
            _notificationcitygrowthglow = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationcitygrowthglow");
            _notificationcitydeclineglow = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationcitydeclineglow");
            _notificationiconsculture = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//NotificationView//notificationiconsculture");
        }

        //void Message_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    ImageBox cnt = sender as ImageBox;
        //    int num = int.Parse( cnt.Tag.ToString() );

        //    if (Messages.Count > num)
        //    {
        //        ScreenNotification not = Messages.ElementAt(num);

        //        MouseEventArgs args = e as MouseEventArgs;

        //        // handle only left clicks (right click will close the notification icon)
        //        if (args.Button == MouseButton.Left)
        //        {
        //            switch (not.Type)
        //            {
        //                case NotificationType.CityGrowth:
        //                case NotificationType.CityDecline:
        //                case NotificationType.FoundCity:
        //                    {
        //                        City city = not.Obj as City;
        //                        MapBox.CenterAt(city.Point);
        //                    }
        //                    break;
        //                case NotificationType.ImprovementReady:
        //                    {
        //                        List<object> objs = not.Obj as List<object>;
        //                        MapBox.CenterAt(objs[1] as HexPoint);
        //                    }
        //                    break;
        //                case NotificationType.Science:
        //                    ShowScienceDialog();
        //                    break;
        //                case NotificationType.PolicyReady:
        //                    ShowPolicyDialog();
        //                    break;
        //                case NotificationType.ProducationReady:
        //                    {
        //                        City city = not.Obj as City;
        //                        MapBox.CenterAt(city.Point);
        //                        CurrentCity = city;
        //                    }
        //                    break;
        //            }
        //        }

        //        not.Obsolete = true;
        //    }      
        //}

        private void UpdateMessages()
        {
            if (View == MapView.Main)
            {
                Messages.RemoveAll(a => a.Obsolete);

                for (int i = 0; i < 10; i++ )
                {
                    Control c = GetControl("Notification" + i);

                    if( c != null )
                        c.Visible = i < Messages.Count;
                }
            }
        }

        private bool IsValidMessage(DiplomaticStatus diplomaticStatus, MessageFilter filter)
        {
            if (diplomaticStatus == null)
                return false;

            switch (diplomaticStatus.Status)
            {
                case BilateralStatus.AtWar:
                    if (IsSet(filter, MessageFilter.Enemies))
                        return true;
                    break;
                case BilateralStatus.Peace:
                    if (IsSet(filter, MessageFilter.Friends))
                        return true;
                    break;
            }

            return false;
        }

        private bool IsSet(MessageFilter value, MessageFilter test)
        {
            return (value & test) == test;
        }

        public void Notification_Draw(object sender, DrawEventArgs e)
        {
            ImageBox cnt = sender as ImageBox;
            int num = int.Parse( "" + cnt.Name.Last() );

            if (Messages.Count > num)
            {
                ScreenNotification not = Messages.ElementAt(num);

                cnt.ToolTip.Text = not.Text;

                cnt.ToolTip.Left = Manager.GraphicsDevice.Viewport.Width - (int)cnt.ToolTip.Skin.Layers[0].Text.Font.Resource.MeasureString(not.Text).X - 10;

                switch (not.Type)
                {
                    default:
                    case NotificationType.FoundCity:
                        e.Renderer.Draw(_notificationgenericglow, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                    case NotificationType.CityGrowth:
                        e.Renderer.Draw(_notificationcitygrowthglow, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                    case NotificationType.CityDecline:
                        e.Renderer.Draw(_notificationcitydeclineglow, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                    case NotificationType.ProducationReady:
                        e.Renderer.Draw(_notificationiconsproduction, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                    case NotificationType.Science:
                        e.Renderer.Draw(_notificationiconsscience, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                    case NotificationType.PolicyReady:
                        e.Renderer.Draw(_notificationiconsculture, e.Rectangle, Microsoft.Xna.Framework.Color.White);
                        break;
                }
                //e.Renderer.DrawString(_notificationFont, _messages.ElementAt(num).Text, e.Rectangle, Microsoft.Xna.Framework.Color.White, Alignment.MiddleCenter);
            }
        }
    }
}
