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

        //ImageBox[] _actionButtons = new ImageBox[6];
        //ImageBox[] _unitButtons = new ImageBox[4];

        private void InitUnitControls()
        {
            //// unit /////////////////////////////////
            //_lblUnit = new ImageBox(Manager);
            //_lblUnit.Init();
            //_lblUnit.Width = 256;
            //_lblUnit.Height = 256;
            //_lblUnit.Left = 0;
            //_lblUnit.Top = Manager.GraphicsDevice.Viewport.Height - 36 - 256;
            //_lblUnit.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//unitpanel");
            //_lblUnit.StayOnTop = true;
            //_lblUnit.Draw += new DrawEventHandler(LblUnit_Draw);
            //_lblUnit.Click += new TomShane.Neoforce.Controls.EventHandler(LblUnit_Click);
            //Add(_lblUnit);

            //for( int i = 0; i < _actionButtons.Length; ++i )
            //{
            //    _actionButtons[i] = new ImageBox(Manager);
            //    _actionButtons[i].Init();
            //    _actionButtons[i].Width = 48;
            //    _actionButtons[i].Height = 48;
            //    _actionButtons[i].Left = 256 + 16 + i * 48;
            //    _actionButtons[i].Top = Manager.GraphicsDevice.Viewport.Height - 128;
            //    _actionButtons[i].Tag = i;
            //    _actionButtons[i].StayOnTop = true;
            //    _actionButtons[i].Visible = false;
            //    _actionButtons[i].Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//UnitView//action");
            //    _actionButtons[i].Draw += new DrawEventHandler(ActionButton_Draw);
            //    _actionButtons[i].Click += new TomShane.Neoforce.Controls.EventHandler(ActionButton_Click);
            //    _actionButtons[i].SizeMode = SizeMode.Stretched;
            //    Add(_actionButtons[i]);
            //}

            //for (int i = 0; i < _unitButtons.Length; ++i)
            //{
            //    _unitButtons[i] = new ImageBox(Manager);
            //    _unitButtons[i].Init();
            //    _unitButtons[i].Width = 48;
            //    _unitButtons[i].Height = 48;
            //    _unitButtons[i].Left = 256 + i * 48;
            //    _unitButtons[i].Top = Manager.GraphicsDevice.Viewport.Height - 170;
            //    _unitButtons[i].Tag = i;
            //    _unitButtons[i].StayOnTop = true;
            //    _unitButtons[i].Visible = false;
            //    _unitButtons[i].Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//UnitView//action");
            //    _unitButtons[i].SizeMode = SizeMode.Stretched;
            //    _unitButtons[i].Draw += new DrawEventHandler(UnitButton_Draw);
            //    _unitButtons[i].Click += new TomShane.Neoforce.Controls.EventHandler(UnitButton_Click);
            //    Add(_unitButtons[i]);
            //}
        }

        //void UnitButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    if (CurrentUnit == null)
        //        return;

        //    ImageBox cnt = sender as ImageBox;
        //    int num = int.Parse(cnt.Tag.ToString());

        //    Unit unit = _currentUnit[num];

        //    // move current unit to the front
        //    _currentUnits.Remove(unit);
        //    _currentUnits.Insert(0, unit);

        //    e.Handled = true;
        //}

        //void UnitButton_Draw(object sender, DrawEventArgs e)
        //{
        //    if (_currentUnits == null)
        //        return;

        //    ImageBox cnt = sender as ImageBox;
        //    int num = int.Parse(cnt.Tag.ToString());

        //    try
        //    {
        //        if (_currentUnits.Count + 1 > num)
        //        {
        //            Unit unit = _currentUnits[num];
        //            Texture2D texture = unit.Data.Image;

        //            if (texture != null)
        //                e.Renderer.Draw(texture, e.Rectangle, Color.White);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //void ActionButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    if (CurrentUnitActions == null)
        //        return;

        //    ImageBox cnt = sender as ImageBox;
        //    int num = int.Parse("" + cnt.Name.Last());

        //    UnitAction action = CurrentUnitActions[num];

        //    if (CurrentUnit != null)
        //    {
        //        CurrentUnit.Execute(action);
        //        e.Handled = true;
        //    }
        //}

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

        //void LblUnit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    if (_currentUnits == null || _currentUnits.Count == 0)
        //        return;

        //    Unit unit = _currentUnits.First();

        //    UnitInfoDialog.Show(Manager, unit.Data, "Unit");
        //}

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


        //void MapBox_UnitsUnselected()
        //{
        //    CurrentUnit = null;

        //    foreach (ImageBox box in _actionButtons)
        //        box.Visible = false;

        //    //_ctxUnitMenu.Items.Clear();
        //}
    
        //void MapBox_HumanUnitSelected(Unit unit)
        //{
        //    CurrentUnit = unit;

        //    //foreach (ImageBox box in _actionButtons)
        //    //    box.Visible = false;

        //    //_ctxUnitMenu.Items.Clear();

        //    //// show units actions
        //    //_currentUnitActions = new List<UnitAction>();

        //    //int j = 0;
        //    //foreach (Unit u in units)
        //    //{
        //    //    foreach (UnitAction action in u.Actions)
        //    //        if (!_currentUnitActions.Contains(action) && CanExecute(action, focus))
        //    //            _currentUnitActions.Add(action);

        //    //    if( j < 4 )
        //    //        _unitButtons[j].Visible = true;

        //    //    j++;
        //    //}

        //    //for (int i = 0; i < Math.Min(_currentUnitActions.Count, _actionButtons.Length); i++)
        //    //{
        //    //    _actionButtons[i].ToolTip.Text = _currentUnitActions[i].ToString();
        //    //    _actionButtons[i].Visible = true;
        //    //}

        //    //foreach (UnitAction ua in _currentUnitActions)
        //    //{
        //    //    MenuItem mi = new MenuItem(ua.ToString());
        //    //    mi.Image = Provider.GetAtlas("UnitActionAtlas").GetTexture(ua.ToString()).GetThumbnail(16,16);
        //    //    mi.Click += new TomShane.Neoforce.Controls.EventHandler(mi_Click);
        //    //    _ctxUnitMenu.Items.Add(mi);
        //    //}
        //}

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

        //void mi_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    if (_currentUnitActions == null)
        //        return;

        //    MenuItem mi = sender as MenuItem;

        //    UnitAction action = _currentUnitActions.FirstOrDefault( a => a.ToString() == mi.Text);

        //    if (CurrentUnit != null)
        //    {
        //        CurrentUnit.Action = action;
        //        CurrentUnit.Execute(action);
        //        e.Handled = true;
        //    }
        //}
    }
}
