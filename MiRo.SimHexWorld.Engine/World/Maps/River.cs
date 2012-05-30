using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class River
    {
        //public int Id { get; set; }
        public string Name { get; private set; }
        public List<HexPoint> Points { get; private set; }

        public River(string name)
        {
            Name = name;
            Points = new List<HexPoint>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}