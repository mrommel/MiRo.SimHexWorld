using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.UI
{
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
                Add(_msgLabels[i]);
            }
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

        void MainWindow_Draw(object sender, DrawEventArgs e)
        {
            ImageBox cnt = sender as ImageBox;
            int num = int.Parse( cnt.Tag.ToString() );
            e.Renderer.DrawString(_notificationFont, _messages.ElementAt(num).Text, e.Rectangle, Microsoft.Xna.Framework.Color.White, Alignment.MiddleCenter);
        }
    }
}
