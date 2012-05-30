using System;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Windows.Forms;

namespace MiRo.SimHexWorld.Engine.Instance.Commands
{
    public class ExceptionCommand : SimpleCommand
    {
        /**
         */
        public override void Execute(INotification note)
        {
            var exception = note as Exception;
            MessageBox.Show(exception != null ? exception.ToString() : "Error during Exception handling", "Error");
        }
    }
}
