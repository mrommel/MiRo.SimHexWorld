using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.UI.Dialogs;

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        Texture2D _overviewSideTextureLeft, _overviewSideTextureRight;
        Texture2D _overviewTexture;
        bool _needToUpdateOverview = false;

        Rectangle overviewSideRect = new Rectangle(0, 0, 11, 224);
        Rectangle overviewInnerRect = new Rectangle(0, 0, 300, 224);

        void InitOverviewControls()
        {
            _overviewSideTextureLeft = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\MainView\\sideleft");
            _overviewSideTextureRight = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\MainView\\sideright");
        }

        public void LblOverview_Draw(object sender, DrawEventArgs e)
        {
            if (_overviewTexture != null)
            {
                overviewInnerRect.X = e.Rectangle.X + 11;
                overviewInnerRect.Y = e.Rectangle.Y;
                e.Renderer.Draw(_overviewTexture, overviewInnerRect, Color.White);
            }

            overviewSideRect.X = e.Rectangle.X;
            overviewSideRect.Y = e.Rectangle.Y;
            e.Renderer.Draw(_overviewSideTextureLeft, overviewSideRect, Color.White);

            overviewSideRect.X = e.Rectangle.X + e.Rectangle.Width - 11;
            overviewSideRect.Y = e.Rectangle.Y;
            e.Renderer.Draw(_overviewSideTextureRight, overviewSideRect, Color.White);
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

                    if (MapBox.FogOfWarEnabled && !Game.Map[x, y].IsSpotted(Game.Human))
                        overviewColors[i] = Color.Transparent;
                    else if (Game.Map[x, y].IsOcean)
                        overviewColors[i] = Color.Aquamarine;
                    else
                    {
                        int controlledBy = Game.Map[x,y].ControlledBy;

                        if (controlledBy == -1 || Game.Players.Count <= controlledBy)
                            overviewColors[i] = Color.LawnGreen;
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
