using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.World.Entities;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Dialogs;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        public Unit CurrentUnit { get; set; }
        public List<UnitAction> CurrentUnitActions { get; set; }

        public void ActionButton_Draw(object sender, DrawEventArgs e)
        {
            if (CurrentUnitActions == null)
                return;

            ImageBox cnt = sender as ImageBox;
            int num = int.Parse("" + cnt.Name.Last());

            try
            {
                if (CurrentUnitActions.Count > num)
                {
                    UnitAction action = CurrentUnitActions[num];
                    Texture2D texture = Provider.GetAtlas("UnitActionAtlas").GetTexture(action.ToString());

                    if (texture != null)
                        e.Renderer.Draw(texture, e.Rectangle, Color.White);
                }
            }
            catch (Exception ex)
            {
 
            }
        }

        public void LblUnit_Draw(object sender, DrawEventArgs e)
        {
            if (CurrentUnit != null)
            {
                Rectangle r = e.Rectangle;
                r.Width = 256;
                r.Height = 256;
                r.X -= 26;
                r.Y += 24;

                if (CurrentUnit.Data.Image != null)
                    e.Renderer.Draw(CurrentUnit.Data.Image, r, Color.White);
                else
                    e.Renderer.DrawString(_cityTitleFont, CurrentUnit.Data.Name, r, Color.Pink, Alignment.MiddleCenter);
            }
        }

        private Improvement road, farm;
        private bool CanExecute(UnitAction action, World.Maps.HexPoint focus)
        {
            switch (action)
            {
                case UnitAction.Found:
                    if (Game.GetCityAt(focus) != null)
                        return false;

                    foreach( HexPoint pt in focus.Neighbors )
                        if (Game.GetCityAt(pt) != null)
                            return false;

                    return true;
                case UnitAction.BuildRoad:

                    if (road == null)
                        road = Provider.GetImprovement("Road");

                    return !Game.Map[focus].Improvements.Contains(road);
                case UnitAction.BuildFarm:

                    if (road == null)
                        road = Provider.GetImprovement("Farm");

                    return !Game.Map[focus].Improvements.Contains(farm);
            }

            return false;
        }
    }
}
