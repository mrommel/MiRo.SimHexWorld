using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.UI
{
    public enum NotificationType { None, ProducationReady, Science, ImprovementReady, FoundCity, CityGrowth, CityDecline };

    public class ScreenNotification
    {
        private string _txt;
        private DateTime _dateTime;
        private NotificationType _type;
        private object _obj;

        public ScreenNotification(NotificationType type, string txt, DateTime dateTime, object obj)
        {
            _type = type;
            _txt = txt;
            _dateTime = dateTime;
            _obj = obj;
        }

        public NotificationType Type
        {
            get { return _type; }
        }

        public bool Obsolete {  get; set; }

        public string Text { get { return _txt; } }

        public object Obj { get { return _obj; } }
    }
}
