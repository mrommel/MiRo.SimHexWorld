using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.UI
{
    public class ScreenNotification
    {
        private string _txt;
        private DateTime _dateTime;

        public ScreenNotification(string txt, DateTime dateTime)
        {
            _txt = txt;
            _dateTime = dateTime;
        }

        public bool Obsolete
        {
            get
            {
                return _dateTime < DateTime.Now;
            }
        }

        public string Text
        { 
            get { return _txt; }
        }
    }
}
