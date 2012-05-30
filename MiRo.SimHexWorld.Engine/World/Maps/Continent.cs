using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class Continent : HexArea
    {
        string _name;
        MapData _map;
        readonly short _id;
        readonly List<GaiaTile> _gaias = new List<GaiaTile>();

        public class GaiaTile
        {
            HexPoint _pt;
            int _value;

            public GaiaTile( HexPoint pt, int value)
            {
                _pt = pt;
                _value = value;
            }
        }

        public Continent(MapData map, string name, short id)
            : base(map)
            
        {
            _map = map;
            _id = id;
            _name = name;
        }

        public int Size
        {
            get { return Count; }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public void AddGoodTileLocation(int food, HexPoint hexPoint)
        {
            _gaias.Add(new GaiaTile(hexPoint, food));
        }

        public override string ToString()
        {
            if (_id == ContinentList.NoContinent)
                return "Ocean";
            return Name + " (" + Size + ")";
        }
    }
}
