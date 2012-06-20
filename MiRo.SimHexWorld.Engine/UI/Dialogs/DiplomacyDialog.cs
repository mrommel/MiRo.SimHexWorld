using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class DiplomacyDialog : AssetWindow
    {
        public DiplomacyDialog(Manager manager)
            : base(manager, "Content//Controls//DiplomacyDialog")
        {
            int i = 0;
            foreach (AbstractPlayerData player in MainWindow.Game.Players)
            {
                ImageBox box = GetControl("LeaderLogo" + i) as ImageBox;

                if( box != null )
                    box.Image = player.Leader.Image;

                Label title = GetControl("LeaderName" + i) as Label;

                if (title != null)
                    title.Text = player.IsHuman ? Strings.TXT_KEY_SATURATE : player.Leader.Title;

                Label civilization = GetControl("CivilizationName" + i) as Label;

                if (civilization != null)
                    civilization.Text = player.Civilization.Title;

                Label rank = GetControl("CivRank" + i) as Label;

                if (rank != null)
                    rank.Text = player.Score.ToString();

                i++;
            }
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { return new List<Instance.GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            
        }
    }
}
