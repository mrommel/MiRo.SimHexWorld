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
        public void InitRightTop()
        {
            LoadControls("Content//Controls//MainWindow.Right");
        }

        public void ShowPolicyDialog()
        {
            PolicyChooseDialog pcd = new PolicyChooseDialog(Manager);
            pcd.Left = 100;
            pcd.Top = 100;

            Manager.Add(pcd);

            pcd.ShowModal();
        }

        public void BtnPolicies_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ShowPolicyDialog();
        }

        public void BtnDiplomacy_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            DiplomacyDialog dd = new DiplomacyDialog(Manager);

            Manager.Add(dd);
            dd.ShowModal();
        }

        public void BtnAdvisors_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
        }
    }
}
