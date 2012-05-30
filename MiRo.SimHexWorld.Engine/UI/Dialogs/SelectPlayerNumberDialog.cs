using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI.Controls;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Types;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI
{
    public class SelectPlayerNumberDialog : Dialog
    {
        ////////////////////////////////////////////////////////////////////////////    
        ImageListBox lstMain;

        private Button btnClose = null;
        private Button btnOk = null;

        private AbstractNamedEntity[] entities = new AbstractNamedEntity[] 
        {
            new SimpleEntity("2 Spieler", "Nur du und eine AI","Content/Textures/PlayerNumbers/two"),
            new SimpleEntity("3 Spieler", "Du und zwei AIs","Content/Textures/PlayerNumbers/three"),
            new SimpleEntity("4 Spieler", "Du und drei AIs","Content/Textures/PlayerNumbers/four"),
            new SimpleEntity("5 Spieler", "Du und vier AIs","Content/Textures/PlayerNumbers/five"),
            new SimpleEntity("6 Spieler", "Du und fünf AIs","Content/Textures/PlayerNumbers/six"),
            new SimpleEntity("7 Spieler", "Du und sechs AIs","Content/Textures/PlayerNumbers/seven"),
        };
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////    
        public SelectPlayerNumberDialog(Manager manager)
            : base(manager)
        {
            Center();
            Text = "Select Player Number";
            Icon = IconProvider.ApplicationIcon;

            TopPanel.Visible = true;
            Caption.Text = "Information";
            Description.Text = "Demonstration of various controls available in Neoforce Controls library.";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);      

            lstMain = new ImageListBox(manager);
            lstMain.Init();
            lstMain.Parent = this;
            lstMain.Top = TopPanel.Height + 8;
            lstMain.Left = 8;
            lstMain.Width = ClientWidth - lstMain.Left - 8;
            lstMain.Height = ClientHeight - 16 - BottomPanel.Height - TopPanel.Height;
            lstMain.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            lstMain.HideSelection = false;
            lstMain.Items.AddRange(entities);

            btnOk = new Button(manager);
            btnOk.Init();
            btnOk.Parent = BottomPanel;
            btnOk.Anchor = Anchors.Top | Anchors.Right;
            btnOk.Top = btnOk.Parent.ClientHeight - btnOk.Height - 8;
            btnOk.Left = btnOk.Parent.ClientWidth - 8 - btnOk.Width * 3 - 8;
            btnOk.Text = "OK";
            btnOk.ModalResult = ModalResult.Ok;

            btnClose = new Button(manager);
            btnClose.Init();
            btnClose.Parent = BottomPanel;
            btnClose.Anchor = Anchors.Top | Anchors.Right;
            btnClose.Top = btnOk.Parent.ClientHeight - btnOk.Height - 8;
            btnClose.Left = btnOk.Parent.ClientWidth - 4 - btnOk.Width * 2 - 8;
            btnClose.Text = "Cancel";
            btnClose.ModalResult = TomShane.Neoforce.Controls.ModalResult.Cancel;

            DefaultControl = lstMain;

            OutlineMoving = true;
            OutlineResizing = true;

            BottomPanel.BringToFront();
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////    
        public string SelectetItem
        {
            get { return lstMain.SelectedItemName; }
        }
        //////////////////////////////////////////////////////////////////////////// 
    }
}
