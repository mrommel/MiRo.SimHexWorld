using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.UI.Dialogs;

namespace MiRo.SimHexWorld.Engine.UI
{
    public partial class MainWindow
    {
        public PolicyChooseDialog PolicyChooseDialog { get; set; }
        public DiplomacyDialog DiplomacyDialog { get; set; }

        public void InitRightTop()
        {
            PolicyChooseDialog = new PolicyChooseDialog(Manager);
            PolicyChooseDialog.Left = 100;
            PolicyChooseDialog.Top = 100;
            PolicyChooseDialog.Visible = false;
            Manager.Add(PolicyChooseDialog);

            DiplomacyDialog = new DiplomacyDialog(Manager);
            DiplomacyDialog.Visible = false;
            Manager.Add(DiplomacyDialog);
        }

        public void ShowPolicyDialog()
        {
            PolicyChooseDialog.ShowModal();
        }
    }
}
