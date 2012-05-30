using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class Civ5Map
    {
        byte _type;
        int width;
        int height;
        int _numOfPlayers;
        int _wrap;
        int _lengthOfTerrainTypeList;
        int _lengthOf1StFeatureTypeList;
        int _lengthOf2NdFeatureTypeList;
        int _lengthOfResourceTypeList;
        int _lengthOfMapName;
        int _lengthOfDescription;
        string terrainTypes;
        string featureTypes;
        string feature2ndType;
        string resourceTypes;
        string mapName;
        string description;
        int lengthOfStr;
        string str;

        PlotItem[,] plots;

        int lengthOfImprovementTypeList; //int -- Length of Improvement type list
        int lengthOfUnitTypeList; //int -- Length of Unit type list
        int lengthOfTechTypeList; //int -- Length of Tech type list
        int lengthOfPolicyTypeList; //int -- Length of Policy type list
        int lengthOfBuildingTypeList; //int -- Length of Building type list
        int lengthOfPromotionTypeList; //int -- Length of Promotion type list
        int lengthOfVictoryTypeList;

        string improvementTypes; //string[] -- Improvement type list
        string unitTypes; //string[] -- Unit type list
        string techTypes; //string[] -- Tech type list
        string policyTypes; //string[] -- Policy type list
        string buildingTypes; //string[] -- Building type list
        string promotionTypes; //string[] -- Promotion type list
        string victoryTypes;

        //byte 0 -- Terrain type ID (index into list of Terrain types read from header)
        //byte 1 -- Resource type ID; 0xFF if none
        //byte 2 -- 1st Feature type ID; 0xFF if none
        //byte 3 -- River indicator (non-zero if tile borders a river; actual value probably indicates direction)
        //byte 4 -- Elevation (0 = Flat, 1 = Hill, 2 = Mountain)
        //byte 5 -- Unknown (possibly related to continent art style)
        //byte 6 -- 2nd Feature type ID; 0xFF if none
        //byte 7 -- Unknown
        public class PlotItem
        {
            public byte TerrainType;
            public byte RessourceType;
            public byte Feature1stType;
            public byte River;
            public byte Elevation;
            public byte Feature2ndType;
            public byte Unused2;

            private byte artStyle;

            public PlotItem() { }
            public PlotItem(BinaryReader br)
            {
                TerrainType = br.ReadByte();
                RessourceType = br.ReadByte();
                Feature1stType = br.ReadByte();
                River = br.ReadByte();
                Elevation = br.ReadByte();
                artStyle = br.ReadByte();
                Feature2ndType = br.ReadByte();
                Unused2 = br.ReadByte();
            }

            public ArtStyle ArtStyle
            {
                get
                {
                    return (ArtStyle)Enum.ToObject(typeof(Civ5Map.ArtStyle), (ulong)artStyle);
                }
            }

            public byte ArtStyleValue
            {
                set
                {
                    artStyle = value;
                }
            }
        }

        public enum ArtStyle
        {
            EUROPEAN = 1,
            ASIAN = 2,
            SOUTH_AMERICA = 3,
            MIDDLE_EAST = 4,
            GRECO_ROMAN = 5,
            BARBARIAN = 6
        }

        public enum PlotDirection
        {
            DIRECTION_NORTH = 1,
            DIRECTION_NORTHEAST = 2,
            DIRECTION_EAST = 4,
            DIRECTION_SOUTHEAST = 8,
            DIRECTION_SOUTH = 16,
            DIRECTION_SOUTHWEST = 32,
            DIRECTION_WEST = 64,
            DIRECTION_NORTHWEST = 128
        }

        public Civ5Map()
        {
        }

        public bool Load( byte[] bytes )
        {
            using(var s = new MemoryStream(bytes))
                using (var reader = new BinaryReader(s))
                {
                    Load(reader);
                }

            return true;
        }

        private bool Load(BinaryReader reader)
        {
            // header
            _type = reader.ReadByte(); //byte -- Type/version indicator. See Notes below.
            width = reader.ReadInt32(); //int -- Map Width
            height = reader.ReadInt32(); //int -- Map Height
            _numOfPlayers = reader.ReadByte(); //byte -- Possibly number of players for scenario.
            _wrap = reader.ReadInt32(); //int -- Unknown; seems to be a world wrap indicator (1 if wrap, 0 if no wrap)
            _lengthOfTerrainTypeList = reader.ReadInt32(); //int -- Length of Terrain type list
            _lengthOf1StFeatureTypeList = reader.ReadInt32(); //int -- Length of 1st Feature type list
            _lengthOf2NdFeatureTypeList = reader.ReadInt32(); //int -- Length of 2nd Feature type list
            _lengthOfResourceTypeList = reader.ReadInt32(); //int -- Length of Resource type list
            reader.ReadInt32(); //int -- Unknown
            _lengthOfMapName = reader.ReadInt32(); //int -- Length of Map Name string
            _lengthOfDescription = reader.ReadInt32(); //int -- Length of Description string

            terrainTypes = new string(reader.ReadChars(_lengthOfTerrainTypeList));//string[] -- Terrain type list
            featureTypes = new string(reader.ReadChars(_lengthOf1StFeatureTypeList)); //string[] -- 1st Feature type list
            feature2ndType = new string(reader.ReadChars(_lengthOf2NdFeatureTypeList)); //string[] -- 2nd Feature type list
            resourceTypes = new string(reader.ReadChars(_lengthOfResourceTypeList)); //string[] -- Resource type list

            mapName = reader.ReadStringZ(); //string -- Map Name string
            description = reader.ReadStringZ(); //string -- Description string

            if (_type > 135)
            {
                lengthOfStr = reader.ReadInt32(); //int -- Length of String3 (only present in version xB or later)
                str = new string(reader.ReadChars(lengthOfStr)); //string -- String3 (only present in version xB or later)
            }

            // data (x*y)
            plots = new PlotItem[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    plots[x, y] = new PlotItem(reader);

            // scenario
            byte[] unused = reader.ReadBytes(84); //84 unknown bytes
            lengthOfImprovementTypeList = reader.ReadInt32(); //int -- Length of Improvement type list
            lengthOfUnitTypeList = reader.ReadInt32(); //int -- Length of Unit type list
            lengthOfTechTypeList = reader.ReadInt32(); //int -- Length of Tech type list
            lengthOfPolicyTypeList = reader.ReadInt32(); //int -- Length of Policy type list
            lengthOfBuildingTypeList = reader.ReadInt32(); //int -- Length of Building type list
            lengthOfPromotionTypeList = reader.ReadInt32(); //int -- Length of Promotion type list
            int unusedInt = reader.ReadInt32();
            lengthOfVictoryTypeList = reader.ReadInt32();
            int unusedInt2 = reader.ReadInt32();

            improvementTypes = new string(reader.ReadChars(lengthOfImprovementTypeList)); //string[] -- Improvement type list
            unitTypes = new string(reader.ReadChars(lengthOfUnitTypeList)); //string[] -- Unit type list
            techTypes = new string(reader.ReadChars(lengthOfTechTypeList)); //string[] -- Tech type list
            policyTypes = new string(reader.ReadChars(lengthOfPolicyTypeList)); //string[] -- Policy type list
            buildingTypes = new string(reader.ReadChars(lengthOfBuildingTypeList)); //string[] -- Building type list
            promotionTypes = new string(reader.ReadChars(lengthOfPromotionTypeList)); //string[] -- Promotion type list
            victoryTypes = new string(reader.ReadChars(lengthOfVictoryTypeList));

            //string rest = "";
            //while (reader.PeekChar() != -1)
            //    rest += reader.ReadChar();

            // rest = "<start>" + rest + "<end>";

            return true;
        }

        public bool Load(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Load(reader);
                }
            }

            return true;
        }

        public string Name
        {
            get { return mapName; }
            set { mapName = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public void Init()
        {
            plots = new PlotItem[width, height];

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    plots[x, y] = new PlotItem();
        }

        List<string> _terrains = new List<string>();

        public List<string> TerrainNames
        {
            get
            {
                if (_terrains.Count == 0 && terrainTypes != null)
                    _terrains.AddRange(terrainTypes.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //else
                //    _terrains = new List<string>();

                return _terrains;
            }
            set
            {
                _terrains = value;
            }
        }

        public string GetTerrainName(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return null;

            if (plots[x, y].Elevation == 1)
                return "Hills";
            else if (plots[x, y].Elevation == 2)
                return "Mountains";

            int terrainId = plots[x, y].TerrainType;

            if (terrainId < 0 || terrainId >= TerrainNames.Count)
                return "Ocean"; // was null

            return TerrainNames[terrainId]; //.Replace("TERRAIN_", "");
        }

        List<string> _features1st = new List<string>();

        public List<string> FeatureNames1st
        {
            get
            {
                if (_features1st.Count == 0 && featureTypes != null)
                    _features1st.AddRange(featureTypes.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                return _features1st;
            }
            set
            {
                _features1st = value;
            }
        }

        readonly List<string> _features2nd = new List<string>();

        private List<string> FeatureNames2nd
        {
            get
            {
                if (_features2nd.Count == 0)
                    _features2nd.AddRange(feature2ndType.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                return _features2nd;
            }
        }

        public IEnumerable<string> GetFeatureNames(int x, int y)
        {
            var result = new List<string>();

            if (x < 0 || y < 0 || x >= width || y >= height)
                return result;

            int feature1stId = plots[x, y].Feature1stType;

            if (feature1stId >= 0 && feature1stId < FeatureNames1st.Count)
                result.Add(FeatureNames1st[feature1stId].Replace("FEATURE_", "").Replace("_", "").ToLower());

            int feature2ndId = plots[x, y].Feature2ndType;

            if (feature2ndId >= 0 && feature2ndId < FeatureNames2nd.Count)
                result.Add(FeatureNames2nd[feature2ndId].Replace("FEATURE_", "").Replace("_", "").ToLower());

            return result;
        }

        readonly List<string> _resources = new List<string>();

        private List<string> ResourceNames
        {
            get
            {
                if (_resources.Count == 0)
                    _resources.AddRange(terrainTypes.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                return _resources;
            }
        }

        public string GetResource(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return null;

            return ResourceNames[plots[x, y].RessourceType];
        }

        public byte GetRiver(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return 0;

            return plots[x, y].River;
        }

        public int Width
        {
            get
            {
                return width;
            }
            set { width = value; }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set { height = value; }
        }

        public PlotItem this[int x, int y]
        {
            get { return plots[x, y]; }
            set { plots[x, y] = value; }
        }

        public PlotItem[,] Plots
        {
            get
            {
                return plots;
            }
        }

        public byte GetTerrainId(int x, int y)
        {
            return plots[x, y].TerrainType;
        }

        public byte GetFeatureId(int x, int y)
        {
            return plots[x, y].Feature1stType;
        }

        public byte GetFeature2ndId(int x, int y)
        {
            return plots[x, y].Feature2ndType;
        }

        public enum HeightType { Flat, Hills, Mountain }

        public HeightType GetHeight(int x, int y)
        {
            switch (plots[x, y].Elevation)
            {
                default:
                case 0:
                    return HeightType.Flat;
                case 1:
                    return HeightType.Hills;
                case 2:
                    return HeightType.Mountain;
            }
        }
    }
}