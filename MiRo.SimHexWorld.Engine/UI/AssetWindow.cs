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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _engine.Invoke("window", "HandleKey", this, e);

            if( !e.Handled )
                base.OnKeyUp(e);
        }

        private void HookAction(IEnumerable<Control> controls)
        {
            foreach (Control c in controls)
            {
                if (c is ImageBox || c is ImageButton || c is TechInfoButton)
                {
                    Control box = c as Control;
                    ControlItem item = c.Tag as ControlItem;

                    if (item != null && !string.IsNullOrEmpty(item.Click))
                    {
                        box.Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                        {
                            _engine.Invoke("window", item.Click, this, sender, e);
                        };
                    }
                }
                else if (c is CheckBox)
                {
                    CheckBox check = c as CheckBox;
                    ControlItem item = c.Tag as ControlItem;

                    if (item != null && !string.IsNullOrEmpty(item.CheckedChanged))
                    {
                        check.CheckedChanged += delegate(object sender, TomShane.Neoforce.Controls.EventArgs args)
                        {
                            _engine.Invoke("window", item.CheckedChanged, this, sender, args);
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
                else if (c is ImageListBox)
                {
                    ImageListBox list = c as ImageListBox;
                    ControlItem item = c.Tag as ControlItem;

                    if (item != null && !string.IsNullOrEmpty(item.ItemIndexChanged))
                    {
                        list.ItemIndexChanged += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                        {
                            _engine.Invoke("window", item.ItemIndexChanged, this, sender, e);
                        };
                    }
                }
                else if (c is ContextMenu)
                {
                    ContextMenu menu = c as ContextMenu;
                    ControlItem item = c.Tag as ControlItem;

                    for (int i = 0; i < item.Items.Count; i++)
                    {
                        MenuItemEntry entry = item.Items[i];

                        if (!string.IsNullOrEmpty(entry.Click))
                        {
                            menu.Items[i].Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                            {
                                _engine.Invoke("window", entry.Click, this, sender, e);
                            };
                        }
                    }
                }

                HookAction(c.Controls);
            }
        }

        public void Execute(string var, string method, Control c, object sender, TomShane.Neoforce.Controls.EventArgs args)
        {
            _engine.Invoke(var, method, c, sender, args);
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

    public class GenericWindow : AssetWindow
    {
        public GenericWindow(Manager manager, string asset) : base(manager, asset) { }

        public void Invoke(string method, params object[] objs)
        {
            _engine.Invoke("window", method, objs);
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { return new List<Instance.GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
