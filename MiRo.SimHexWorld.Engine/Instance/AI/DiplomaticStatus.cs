using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public enum BilateralStatus
    {
        AtWar,
        Peace,
        Alliance,
        VoteForUN
    }

    public class DiplomaticStatus
    {
        public string CivilizationName { get; set; }

        public bool IsMinor { get; set; }
        public bool IsDiscovered { get; set; }
        public bool IsConnected { get; set; }
        public bool HasBeenConquered { get; set; }
        public bool HasBeenAttacked { get; set; }

        public BilateralStatus Status { get; set; }
    }
}
