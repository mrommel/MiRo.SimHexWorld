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

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        List<Unit> _currentUnits = null;
        List<UnitAction> _currentUnitActions = null;

        ImageBox[] _actionButtons = new ImageBox[6];

        private void InitUnitControls()
        {
            // unit /////////////////////////////////
            _lblUnit = new ImageBox(Manager);
            _lblUnit.Init();
            _lblUnit.Width = 256;
            _lblUnit.Height = 256;
            _lblUnit.Left = 0;
            _lblUnit.Top = Manager.GraphicsDevice.Viewport.Height - 36 - 256;
            _lblUnit.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//unitpanel");
            _lblUnit.StayOnTop = true;
            _lblUnit.Draw += new DrawEventHandler(LblUnit_Draw);
            _lblUnit.Click += new TomShane.Neoforce.Controls.EventHandler(LblUnit_Click);
            Add(_lblUnit);

            for( int i = 0; i < _actionButtons.Length; ++i )
            {
                _actionButtons[i] = new ImageBox(Manager);
                _actionButtons[i].Init();
                _actionButtons[i].Width = 48;
                _actionButtons[i].Height = 48;
                _actionButtons[i].Left = 256 + 16 + i * 48;
                _actionButtons[i].Top = Manager.GraphicsDevice.Viewport.Height - 128;
                _actionButtons[i].Tag = i;
                _actionButtons[i].StayOnTop = true;
                _actionButtons[i].Visible = false;
                _actionButtons[i].Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//UnitView//action");
                _actionButtons[i].Draw += new DrawEventHandler(ActionButton_Draw);
                _actionButtons[i].Click += new TomShane.Neoforce.Controls.EventHandler(ActionButton_Click);
                _actionButtons[i].SizeMode = SizeMode.Stretched;
                Add(_actionButtons[i]);
            }
        }

        void ActionButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (_currentUnitActions == null)
                return;

            ImageBox cnt = sender as ImageBox;
            int num = int.Parse(cnt.Tag.ToString());

            UnitAction action = _currentUnitActions[num];

            _currentUnits.First().Execute(action);
        }

        void ActionButton_Draw(object sender, DrawEventArgs e)
        {
            if (_currentUnitActions == null)
                return;

            ImageBox cnt = sender as ImageBox;
            int num = int.Parse(cnt.Tag.ToString());

            try
            {
                if (_currentUnitActions.Count > num)
                {
                    UnitAction action = _currentUnitActions[num];
                    Texture2D texture = Provider.GetAtlas("UnitActionAtlas").GetTexture(action.ToString());

                    if (texture != null)
                        e.Renderer.Draw(texture, e.Rectangle, Color.White);
                }
            }
            catch (Exception ex)
            {
 
            }
        }

        void LblUnit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (_currentUnits == null || _currentUnits.Count == 0)
                return;

            Unit unit = _currentUnits.First();

            UnitInfoDialog.Show(Manager, unit.Data, "Unit");
        }

        void LblUnit_Draw(object sender, DrawEventArgs e)
        {
            if (_currentUnits != null && _currentUnits.Count > 0)
            {
                Unit unit = _currentUnits.First();

                Rectangle r = e.Rectangle;
                r.Width = 256;
                r.Height = 256;
                r.X -= 26;
                r.Y += 24;
                e.Renderer.Draw(unit.Data.Image, r, Color.White);
            }
        }
    
        void MapBox_HumanUnitsSelected(List<Unit> units)
        {
            _currentUnits = units;

            foreach (ImageBox box in _actionButtons)
                box.Visible = false;

            // show units actions
            _currentUnitActions = new List<UnitAction>();

            foreach (Unit u in units)
            {
                foreach (UnitAction action in u.Actions)
                    if (!_currentUnitActions.Contains(action))
                        _currentUnitActions.Add(action);
            }

            for (int i = 0; i < Math.Min(_currentUnitActions.Count, _actionButtons.Length); i++)
                _actionButtons[i].Visible = true;
        }
    }
}
