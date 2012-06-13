using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        ImageBox _lblOverview;
        ImageBox _lblBottomRight;

        Texture2D _overviewSideTexture;
        Texture2D _overviewTexture;
        bool _needToUpdateOverview = false;

        void InitOverviewControls()
        {
            _overviewSideTexture = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\MainView\\sideleft");

            _lblOverview = new ImageBox(Manager);
            _lblOverview.Init();
            _lblOverview.Width = 300;
            _lblOverview.Height = 224;
            _lblOverview.Left = Manager.GraphicsDevice.Viewport.Width - _lblOverview.Width - 32;
            _lblOverview.Top = Manager.GraphicsDevice.Viewport.Height - _lblOverview.Height - 36;
            _lblOverview.SizeMode = SizeMode.Stretched;
            _lblOverview.Draw += new DrawEventHandler(LblOverview_Draw);
            Add(_lblOverview);

            _lblBottomRight = new ImageBox(Manager);
            _lblBottomRight.Init();
            _lblBottomRight.Width = 128;
            _lblBottomRight.Height = 224;
            _lblBottomRight.Left = Manager.GraphicsDevice.Viewport.Width - _lblOverview.Width - _lblBottomRight.Width - 32;
            _lblBottomRight.Top = Manager.GraphicsDevice.Viewport.Height - _lblBottomRight.Height - 36;
            _lblBottomRight.Image = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\MainView\\bottomright128x224");
            _lblBottomRight.SizeMode = SizeMode.Stretched;
            Add(_lblBottomRight);
        }

        void LblOverview_Draw(object sender, DrawEventArgs e)
        {
            if (_overviewTexture != null)
            {
                Rectangle r = new Rectangle(e.Rectangle.X + 11, e.Rectangle.Y, 300, 224);
                e.Renderer.Draw(_overviewTexture, r, Color.White);
            }

            Rectangle r2 = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 11, 224);
            e.Renderer.Draw(_overviewSideTexture, r2, Color.White);
        }

        void UpdateOverviewControls()
        {
            if (Game.Map == null)
                return;

            if (_overviewTexture == null)
                _overviewTexture = new Texture2D(Manager.GraphicsDevice, Game.Map.Width, Game.Map.Height);

            Color[] overviewColors = new Color[Game.Map.Width * Game.Map.Height];

            for (int x = 0; x < Game.Map.Width; x++)
            {
                for (int y = 0; y < Game.Map.Height; y++)
                {
                    int i = x + y * Game.Map.Width;

                    if (_mapBox.FogOfWarEnabled && !Game.Map[x, y].IsSpotted(Game.Human))
                        overviewColors[i] = Color.Transparent;
                    else if (Game.Map[x, y].IsOcean)
                        overviewColors[i] = Color.LightBlue;
                    else
                    {
                        int controlledBy = Game.Map[x,y].ControlledBy;

                        if (controlledBy == -1 || Game.Players.Count <= controlledBy)
                            overviewColors[i] = Color.LightGreen;
                        else
                        {
                            AbstractPlayerData controller = Game.Players[controlledBy];

                            if (Game.GetCityAt(new HexPoint(x,y)) != null)
                                overviewColors[i] = controller.PlayerColor.Primary;
                            else
                                overviewColors[i] = controller.PlayerColor.Secondary;
                        }
                    }
                }
            }

            _overviewTexture.SetData<Color>(overviewColors);
        }
    }
}
