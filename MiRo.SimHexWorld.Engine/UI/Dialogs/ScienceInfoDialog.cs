using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class ScienceInfoDialog : Window
    {
        ImageBox icon;
        TextBox txtBox;
        Button okButton;

        private ScienceInfoDialog(Manager manager)
            : base(manager)
        {
            Init();

            Height = 300;
            Width = 500;

            txtBox = new TextBox(manager);
            txtBox.Init();
            txtBox.Width = ClientWidth;
            txtBox.Height = ClientHeight - 30;
            txtBox.Left = 0;
            txtBox.Top = 0;
            txtBox.Parent = this;

            okButton = new Button(manager);
            okButton.Init();
            okButton.Text = "OK";
            okButton.Width = txtBox.Width; ;
            okButton.Height = 30;
            okButton.Top = txtBox.Height;
            okButton.Parent = this;
            okButton.Click += new TomShane.Neoforce.Controls.EventHandler(okButton_Click);

            manager.Add(this);
        }

        void okButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Methode zum anzeigen einer MessageBox
        /// </summary>
        /// <param name="text">Der Inhalt der MessageBox</param>
        /// <param name="title">Der Titel der Textbox</param>
        public static void Show(Manager manager, Tech tech, string title)
        {
            ScienceInfoDialog msgBox = new ScienceInfoDialog(manager);
            msgBox.Text = title;
            msgBox.txtBox.Text = tech.Name;
            msgBox.Resizable = false;
            msgBox.ShowModal();
        }
    }
}
