using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class NewEraWindow : Window
    {
        ImageBox _lblEraImage;
        Label _lblDescription;

        private NewEraWindow(Manager manager, Era era)
            : base(manager)
        {
            Init();

            Width = 950;
            Height = 550;

            _lblEraImage = new ImageBox(manager);
            _lblEraImage.Init();
            _lblEraImage.Left = 4;
            _lblEraImage.Top = 10;          
            _lblEraImage.Width = 924;
            _lblEraImage.Height = 472;
            _lblEraImage.Image = era.Image;
            Add(_lblEraImage);

            _lblDescription = new Label(manager);
            _lblDescription.Init();
            _lblDescription.Left = 4;
            _lblDescription.Top = 482;
            _lblDescription.Width = 924;
            _lblDescription.Height = 16;
            _lblDescription.Text = string.Format(Strings.TXT_KEY_UI_NEWERA_WELCOME, era.Title);
            Add(_lblDescription);

            Manager.Add(this);
        }

        public static void Show(Manager manager, Era era, string title)
        {
            NewEraWindow msgBox = new NewEraWindow(manager, era);
            msgBox.Text = title;
            msgBox.Resizable = false;
            msgBox.ShowModal();
        }
    }
}
