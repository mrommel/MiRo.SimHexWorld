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

namespace MiRo.SimHexWorld.Engine.UI
{
    [Flags]
    public enum MessageFilter { Self, Friends, Neutral, Enemies };

    partial class MainWindow
    {
        List<ScreenNotification> _messages = new List<ScreenNotification>();
        ImageBox[] _msgLabels = new ImageBox[10];

        Texture2D _notificationTexture;
        SpriteFont _notificationFont;

        private void InitMessages()
        {
            _notificationTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//notification");
            _notificationFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");

            for (int i = 0; i < _msgLabels.Length; i++)
            {
                _msgLabels[i] = new ImageBox(Manager);
                _msgLabels[i].Init();
                _msgLabels[i].Left = (Manager.GraphicsDevice.Viewport.Width - 320) / 2;
                _msgLabels[i].Width = 320;
                _msgLabels[i].Height = 26;
                _msgLabels[i].Top = 70 + i * 28;
                _msgLabels[i].Tag = i;
                _msgLabels[i].Image = _notificationTexture;
                _msgLabels[i].Draw += new DrawEventHandler(MainWindow_Draw);
                _msgLabels[i].Click += new TomShane.Neoforce.Controls.EventHandler(Message_Click);
                Add(_msgLabels[i]);
            }
        }

        void Message_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            
        }

        private void UpdateMessages()
        {
            #region messages
            _messages.RemoveAll(a => a.Obsolete);

            for (int i = 0; i < _msgLabels.Length; i++)
                _msgLabels[i].Visible = false;

            for (int i = 0; i < Math.Min(_msgLabels.Length, _messages.Count); i++)
            {
                _msgLabels[i].Visible = true;
                _msgLabels[i].Text = _messages[i].Text;
            }
            #endregion messages
        }

        public override void HandleNotification(INotification notification)
        {
            switch ((GameNotification)System.Enum.Parse(typeof(GameNotification), notification.Name))
            {
                case GameNotification.CreateMapSuccess:
                    _game.Map = notification.Body as MapData;
                    _mapBox.Map = _game.Map;

                    _game.Initialize();

                    break;
                case GameNotification.LoadMapSuccess:
                    _game.Map = notification.Body as MapData;
                    _mapBox.Map = _game.Map;

                    _game.Initialize();
                    break;
                case GameNotification.Message:
                    List<object> objs = notification.Body as List<object>;

                    string txt = objs[0] as string;
                    Civilization civSender = objs[1] as Civilization;
                    MessageFilter filter = (MessageFilter)objs[2];

                    if ((civSender.Name == Game.Human.Civilization.Name && (IsSet(filter, MessageFilter.Self))) ||
                        IsValidMessage(Game.Human.DiplomaticStatusTo(civSender), filter))
                        _messages.Add(new ScreenNotification(txt, DateTime.Now.AddSeconds(10)));

                    break;
                case GameNotification.UpdateSpotting:
                    _needToUpdateOverview = true;
                    break;
                default:
                    throw new System.Exception(notification.Name + " notification not handled");
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

        void MainWindow_Draw(object sender, DrawEventArgs e)
        {
            ImageBox cnt = sender as ImageBox;
            int num = int.Parse( cnt.Tag.ToString() );
            e.Renderer.DrawString(_notificationFont, _messages.ElementAt(num).Text, e.Rectangle, Microsoft.Xna.Framework.Color.White, Alignment.MiddleCenter);
        }
    }
}
