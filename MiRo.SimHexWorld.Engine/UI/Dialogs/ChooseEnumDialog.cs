using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class EnumChooseEventArgs
    {
        object _selectedEnum;

        public EnumChooseEventArgs(object selectedEnum)
        {
            _selectedEnum = selectedEnum;
        }

        public object SelectedEnum
        {
            get
            { return _selectedEnum;  }
        }
    }

    public delegate void EnumChooseEventHandler(Object sender, EnumChooseEventArgs e);

    /// <summary>
    /// Shows a window which allows to chose one entry from a enum.
    /// </summary>
    public class ChooseEnumDialog
    {
        Type _type;
        EnumChooseEventHandler _handler;

        /// <summary>
        /// Shows the chose enum dialog.
        /// </summary>
        public void ShowChooseEnumDialog(Manager manager, string caption, Type type, EnumChooseEventHandler btnEventHandler)
        {
            _type = type;
            _handler = btnEventHandler;

            window = new Window(manager);
            window.Init();
            window.Width = 175;
            window.Text = caption;
            window.Closing += OnChooseEnumDialogClosing;
            window.Visible = true;

            if (!type.IsEnum)
            {
                CloseDialog();
            }
            else
            {
                int offsetY = 5;

                foreach (object value in Enum.GetValues(type))
                {
                    string name = value.ToString();

                    Button btnEnumName = new Button(manager);
                    btnEnumName.Init();
                    btnEnumName.Text = name;
                    btnEnumName.Width = 150;
                    btnEnumName.Height = 30;
                    btnEnumName.Left = 5;
                    btnEnumName.Top = offsetY;
                    btnEnumName.Tag = value;
                    btnEnumName.Click += OnClickBtnEnumName;
                    btnEnumName.Parent = window;

                    offsetY += 35;
                }

                window.Height = offsetY + 35;
                manager.Add(window);
            }
        }

        #region nonpublic members

        private Window window;

        private void CloseDialog()
        {
            window.Close();
        }

        /// <summary>
        /// Called when the chose enum dialog is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChooseEnumDialogClosing(Object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
        }

        /// <summary>
        /// Called when the user clicks on a button in the chose enum dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickBtnEnumName(Object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (_handler != null)
            {
                Button btn = sender as Button;
                object obj = Enum.Parse(_type, btn.Text);
                _handler(sender, new EnumChooseEventArgs(obj));
            }

            CloseDialog();
        }

        #endregion
    }
}
