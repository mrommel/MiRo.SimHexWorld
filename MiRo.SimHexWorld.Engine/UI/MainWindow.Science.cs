﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Dialogs;

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        ImageBox _lblLeftTopCorner;
        Texture2D _scienceBackTexture, _scienceFrameTexture, _scienceTexture;

        public void InitScienceControls()
        {
            _scienceBackTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//techpanelback");
            _scienceFrameTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//techpanelframe");
            _scienceTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//techpanelmeter");

            _lblLeftTopCorner = new ImageBox(Manager);
            _lblLeftTopCorner.Init();
            _lblLeftTopCorner.Top = 24;
            _lblLeftTopCorner.Left = 0;
            _lblLeftTopCorner.Width = 512;
            _lblLeftTopCorner.Height = 256;
            _lblLeftTopCorner.StayOnBack = true;
            _lblLeftTopCorner.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//topleft022");
            _lblLeftTopCorner.SizeMode = SizeMode.Normal;
            _lblLeftTopCorner.Draw += new DrawEventHandler(LblLeftTopCorner_Draw);
            _lblLeftTopCorner.Click += new TomShane.Neoforce.Controls.EventHandler(LblLeftTopCorner_Click);
            Add(_lblLeftTopCorner);
        }

        void LblLeftTopCorner_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if( Game.Human.CurrentResearch != null )
                ScienceInfoDialog.Show(Manager, Game.Human.CurrentResearch, "Science");
        }

        void LblLeftTopCorner_Draw(object sender, DrawEventArgs e)
        {
            Texture2D _scienceMeterModTexture = new Texture2D(Manager.GraphicsDevice, 220, 220);

            float radReady = Game.Human.ScienceReady * (float)Math.PI * 2f - (float)Math.PI;

            // remove not present slice
            Color[] colors = new Color[220 * 220];
            _scienceTexture.GetData<Color>(colors);

            for (int x = 0; x < 220; x++)
            {
                for (int y = 0; y < 220; ++y)
                {
                    int i = y * 220 + x;

                    if (Math.Atan2(x - 110, y - 110) < radReady)
                        colors[i] = Microsoft.Xna.Framework.Color.Transparent;
                }
            }

            _scienceMeterModTexture.SetData<Color>(colors);

            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 220, 220);

            e.Renderer.Draw(_scienceMeterModTexture, r, Color.White);
            e.Renderer.Draw(_scienceFrameTexture, r, Color.White);

            if (Game.Human.CurrentResearch != null)
            {
                if (Game.Human.CurrentResearch.Image != null)
                    e.Renderer.Draw(Game.Human.CurrentResearch.Image, r, Color.White);
                else
                    e.Renderer.DrawString(_notificationFont, Game.Human.CurrentResearch.Title, r, Color.White, Alignment.MiddleCenter);
            }
        }

        //public void UpdateScienceControls()
        //{

        //}
    }
}
