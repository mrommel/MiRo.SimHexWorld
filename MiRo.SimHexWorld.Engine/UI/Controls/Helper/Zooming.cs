using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.UI.Controls.Helper
{
    public delegate void ZoomHandler( float zoom );

    public class Zooming
    {
        private Dictionary<string, float> _zooms = new Dictionary<string, float>();
        private string _currentZoomName;

        public event ZoomHandler ZoomChanged;

        /// <summary>
        /// create a new zooming object
        /// </summary>
        /// <param name="currentZoomName">first zoom level name, must be part of the zoomPairs</param>
        /// <param name="zoomPairs">list of zoom pairs: {'name', 0.1, 'name2', 0.2, etc. }</param>
        public Zooming(string currentZoomName, params object[] zoomPairs)
        {
            if( zoomPairs.Length % 2 != 0 )
                throw new Exception("Parameters must be dividable by two");

            for (int i = 0; i < zoomPairs.Length; i += 2)
                _zooms.Add(zoomPairs[i].ToString(), float.Parse(zoomPairs[i+1].ToString()));

            CurrentZoomName = currentZoomName;
        }

        public void ZoomIn()
        {
            string index = "none";

            // find the biggest zoom that is smaller than the current
            foreach (KeyValuePair<string, float> pair in _zooms)
            { 
                if( index == "none" ) 
                {
                    if( pair.Value < CurrentZoom )
                        index = pair.Key;
                }
                else if(pair.Value < CurrentZoom && pair.Value > _zooms[index] )
                    index = pair.Key;
            }

            if (index != "none")
                CurrentZoomName = index;
        }

        public void ZoomOut()
        {
            string index = "none";

            // find the smallest zoom that is bigger than the current
            foreach (KeyValuePair<string, float> pair in _zooms)
            {
                if (index == "none")
                {
                    if (pair.Value > CurrentZoom)
                        index = pair.Key;
                }
                else if (pair.Value > CurrentZoom && pair.Value < _zooms[index])
                    index = pair.Key;
            }

            if (index != "none")
                CurrentZoomName = index;
        }

        public float CurrentZoom
        {
            get { return _zooms[_currentZoomName]; }
        }

        public string CurrentZoomName 
        {
            get { return _currentZoomName; }
            set 
            {
                if (_zooms.ContainsKey(value))
                {
                    _currentZoomName = value;

                    OnZoomChanged(_zooms[value]);
                }
                else
                    throw new Exception("Could not find " + value);
            }
        }

        private void OnZoomChanged(float newZoom)
        {
            if (ZoomChanged != null)
                ZoomChanged(newZoom);
        }
    }
}
