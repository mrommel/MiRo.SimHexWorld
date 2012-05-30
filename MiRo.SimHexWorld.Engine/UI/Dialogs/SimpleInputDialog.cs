using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    /// <summary>
    /// Shows a window with a single textbox and Ok/Cancel buttons.
    /// </summary>
    public class SimpleInputDialog
    {
        /// <summary>
        /// Shows the simple input dialog.
        /// </summary>
        public void ShowSimpleInputDialog(Manager manager, string caption, string text, string initialValue,
            TomShane.Neoforce.Controls.EventHandler okBtnEventHandler)
        {
            window = new Window(manager);
            window.Init();
            window.Width = 450;
            window.Height = 150;
            window.Text = caption;
            window.Closing += OnSimpleInputDialogClosing;
            window.Visible = true;

            Label label = new Label(manager);
            label.Init();
            label.Text = text;
            label.Width = 400;
            label.Height = 20;
            label.Left = 5;
            label.Top = 5;
            label.Parent = window;

            TextBox textBox = new TextBox(manager);
            textBox.Init();
            textBox.Width = 400;
            textBox.Height = 20;
            textBox.Left = 5;
            textBox.Top = 40;
            textBox.Text = initialValue;
            textBox.Parent = window;

            Button btnSimpleInputDialogOk = new Button(manager);
            btnSimpleInputDialogOk.Init();
            btnSimpleInputDialogOk.Text = "Ok";
            btnSimpleInputDialogOk.Width = 100;
            btnSimpleInputDialogOk.Height = 30;
            btnSimpleInputDialogOk.Left = 5;
            btnSimpleInputDialogOk.Top = 75;
            btnSimpleInputDialogOk.Click += okBtnEventHandler;
            btnSimpleInputDialogOk.Click += OnClickBtnSimpleInputDialogOk;
            btnSimpleInputDialogOk.Tag = textBox; // Textbox as Tag, to access the value
            btnSimpleInputDialogOk.Parent = window;

            Button btnSimpleInputDialogCancel = new Button(manager);
            btnSimpleInputDialogCancel.Init();
            btnSimpleInputDialogCancel.Text = "Cancel";
            btnSimpleInputDialogCancel.Width = 100;
            btnSimpleInputDialogCancel.Height = 30;
            btnSimpleInputDialogCancel.Left = 110;
            btnSimpleInputDialogCancel.Top = 75;
            btnSimpleInputDialogCancel.Click += OnClickBtnSimpleInputDialogCancel;
            btnSimpleInputDialogCancel.Parent = window;

            manager.Add(window);
        }

        #region nonpublic members

        private Window window;

        /// <summary>
        /// Called when the simple input dialog is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSimpleInputDialogClosing(Object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
        }

        /// <summary>
        /// Called when the user clicks the 'Ok' button in the simple input dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickBtnSimpleInputDialogOk(Object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            window.Close();
        }

        /// <summary>
        /// Called when the user clicks the 'Cancel' button in the simple input dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickBtnSimpleInputDialogCancel(Object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            window.Close();
        }

        #endregion
    }
}
