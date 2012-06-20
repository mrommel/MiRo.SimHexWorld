using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.AI;
using MiRo.SimHexWorld.Engine.Instance.AI;
using Microsoft.Scripting.Hosting;
using MiRo.SimHexWorld.Engine.UI.Controls;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.UI
{
    public abstract class AssetWindow : GameWindow
    {
        protected PythonEngine _engine;
        protected Callback _callback;

        public AssetWindow(Manager manager, string assetName)
            : base(manager)
        {
            LoadWindow(assetName);
            LoadAction(assetName + "Actions");

            Init(); // for the close button
        }

        private void LoadAction(string path)
        {
            _callback = new Callback(this);
            _engine = new PythonEngine(_callback);
            _engine.Initialize();

            PythonScript script = Manager.Content.Load<PythonScript>(path);
            _engine.CompileSourceAndExecute(script.Actions);
            _engine.Invoke("window", "Initialize", this);

            HookAction(Controls);
        }

        private void HookAction(IEnumerable<Control> controls)
        {
            foreach (Control c in controls)
            {
                if (c is ImageBox)
                {
                    ImageBox box = c as ImageBox;
                    ControlItem item = c.Tag as ControlItem;

                    if (item != null && !string.IsNullOrEmpty(item.Click))
                    {
                        box.Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                        {
                            _engine.Invoke("window", item.Click, this, sender, e);
                        };
                    }
                }
                else if (c is GameMapBox)
                {
                    GameMapBox box = c as GameMapBox;
                    ControlItem item = c.Tag as ControlItem;

                    if (item != null && !string.IsNullOrEmpty(item.CityOpened))
                    {
                        box.CityOpened += delegate(City city)
                        {
                            _engine.Invoke("window", item.CityOpened, this, city);
                        };
                    }

                    if (item != null && !string.IsNullOrEmpty(item.CitySelected))
                    {
                        box.CitySelected += delegate(City city)
                        {
                            _engine.Invoke("window", item.CitySelected, this, city);
                        };
                    }

                    if (item != null && !string.IsNullOrEmpty(item.FocusChanged))
                    {
                        box.FocusChanged += delegate(MapChangeArgs args)
                        {
                            _engine.Invoke("window", item.FocusChanged, this, args);
                        };
                    }

                    if (item != null && !string.IsNullOrEmpty(item.HumanUnitSelected))
                    {
                        box.HumanUnitsSelected += delegate(Unit unit)
                        {
                            _engine.Invoke("window", item.HumanUnitSelected, this, unit);
                        };
                    }

                    if (item != null && !string.IsNullOrEmpty(item.UnitUnselected))
                    {
                        box.UnitsUnselected += delegate()
                        {
                            _engine.Invoke("window", item.UnitUnselected, this);
                        };
                    }
                    
                } 

                HookAction(c.Controls);
            }
        }

        protected class Callback : IScriptable
        {
            AssetWindow _parent;

            public Callback(AssetWindow parent)
            {
                _parent = parent;
            }

            void IScriptable.ScriptCallback(string message, object body)
            {
                System.Windows.Forms.MessageBox.Show(message);
            }
        }
    }
}
