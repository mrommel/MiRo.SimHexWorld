using System;
using System.Linq;
using System.Collections.Generic;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Misc;
using System.Threading;
using MiRo.SimHexWorld.Engine.UI;

namespace MiRo.SimHexWorld.Engine.Instance.Commands
{
    public class MapLoadCommand : SimpleCommand
    {
        /**
         * Constructor.
         */
        public MapLoadCommand()
            : base()
        { }

        /**
         */
        public override void Execute(INotification note)
        {
            Thread t = new Thread(() => Run(note.Body as Civ5Map));
            t.Start();
        }

        public static void Run(Civ5Map civ5map)
        {
            MapData map = new MapData();

            try
            {
                map.Extension = MainApplication.Instance.Content.Load<MapExtension>("Content/MapsExtension/" + civ5map.FileName);
            }
            catch { }

            map.InitFromCiv5Map(civ5map);

            GameFacade.getInstance().SendNotification(GameNotification.LoadMapSuccess, map);
        }

        //class RiverInfo
        //{
        //    public byte Value { get; set; }
        //    public string Binary { get; set; }
        //    public string Flows { get; set; }

        //    public override string ToString()
        //    {
        //        return string.Format("{0}, {1}, {2}, {3}", Value, Binary, Binary.Replace("0", "").Length, Flows);
        //    }
        //}

        //public static byte ShiftRight(byte value, int shiftBy)
        //{
        //    byte val = value;

        //    for (int i = 0; i < shiftBy; ++i)
        //        val = ShiftRight(val);

        //    return val;
        //}

        //public static byte ShiftRight(byte value)
        //{
        //    value += (byte)((value % 2) * (byte)Math.Pow(2, 6));
        //    return (byte)((value >> 1) & 63);
        //}
    }
}
