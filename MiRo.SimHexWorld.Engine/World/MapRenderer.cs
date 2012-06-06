using System;
using System.Linq;
using MiRo.SimHexWorld.Engine.World.Helper;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.World.Meshed;
using MiRoSimHexWorld.Engine.World;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.World.Maps;
using log4net;
using MiRoSimHexWorld.Engine.World.Helper;
using MiRo.SimHexWorld.Engine.UI.Entities;
using MiRo.SimHexWorld.Engine.UI;
using MiRo.SimHexWorld.Helper;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Misc;
using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.World
{
    public class MapRenderer
    {
        readonly ILog _log = LogManager.GetLogger(typeof(MapRenderer));

        readonly Mesh _baseMesh;

        // cursor stuff
        readonly Mesh _cursorsMesh;
        HexPoint _lastPos = new HexPoint();

        Mesh _borderMesh, _roadMesh, _farmMesh;

        readonly Manager _manager;

        enum EBillBoard { Wood1, Wood2, Wood3, Jungle1, Jungle2, Jungle3, Pine1, Pine2 }
        BillboardSystem<EBillBoard> _terrainBillboards;
        readonly Vector3 _woodPos1 = new Vector3(1.7f, 0f, 1.7f);
        readonly Vector3 _woodPos2 = new Vector3(1.7f, 0f, -1.7f);
        readonly Vector3 _woodPos3 = new Vector3(-1.8f, 0f, 0f);

        //readonly TiledMeshContainer _tiledMeshContainer;

        NamedList<TileMatchPattern> _hiddenPatterns;
        TileMatchPatternMatcher _hiddenMatcher;
        int _hiddenTilesetIndex = 0;
        readonly TiledMeshContainer _hiddenMeshContainer;

        MapData _map;
        bool _needToCreate = false;
        bool _needToUpdateHidden = false;
        bool _needToUpdateBorders = false;
        bool _needToUpdateRoads = false;

        List<ForestEntity> _forests = new List<ForestEntity>();

        public MapRenderer(Manager manager)
        {
            _manager = manager;

            _baseMesh = new Mesh(manager.GraphicsDevice, "ground");
            _cursorsMesh = new Mesh(manager.GraphicsDevice, "cursors");
            _borderMesh = new Mesh(manager.GraphicsDevice, "cursors");
            _roadMesh = new Mesh(manager.GraphicsDevice, "roads");
            _farmMesh = new Mesh(manager.GraphicsDevice, "farms");

            TextureManager.Instance.Device = manager.GraphicsDevice;

            //_tiledMeshContainer = new TiledMeshContainer(manager);
            _hiddenMeshContainer = new TiledMeshContainer(manager);

            FogOfWarEnabled = false;

            hMesh = new HexMesh(manager.GraphicsDevice);

            Center = new HexPoint();
        }

        public HexPoint Center { get; set; }

        public void Initialize()
        {
            _hiddenPatterns = _manager.Content.Load<NamedList<TileMatchPattern>>("Content/Data/hiddenmatchpatterns");
            _hiddenMatcher = new TileMatchPatternMatcher(_hiddenPatterns);
        }

        public void LoadContent()
        {
            // big water surface to make the world a gigantic ocean
            TextureManager.Instance.Add("ground", _manager.Content.Load<Texture2D>("Content/Textures/Ground/ground"));
            _baseMesh.LoadContent(_manager.Content);
            _baseMesh.AddObject(new MeshedRectangle(new Vector3(0, -0.1f, 0), new Vector3(1000)));

            // cursor
            TextureManager.Instance.Add("cursors", _manager.Content.Load<Texture2D>("Content/Textures/Ground/cursors"));
            _cursorsMesh.LoadContent(_manager.Content);
            _cursorsMesh.AddObject(new HexagonMeshItem8X8(MapData.GetWorldPosition(20, 20), 63));

            _borderMesh.LoadContent(_manager.Content);

            // roads
            TextureManager.Instance.Add("roads", _manager.Content.Load<Texture2D>("Content/Textures/Ground/roads"));
            _roadMesh.LoadContent(_manager.Content);

            // farms
            TextureManager.Instance.Add("farms", _manager.Content.Load<Texture2D>("Content/Textures/Ground/farms"));
            _farmMesh.LoadContent(_manager.Content);

            _terrainBillboards = new BillboardSystem<EBillBoard>(_manager.GraphicsDevice, _manager.Content);
            _terrainBillboards.AddEntity(EBillBoard.Wood1, "Content/Textures/Billboards/wood1", new Vector2(3, 3));
            _terrainBillboards.AddEntity(EBillBoard.Wood2, "Content/Textures/Billboards/wood2", new Vector2(3, 3));
            _terrainBillboards.AddEntity(EBillBoard.Wood3, "Content/Textures/Billboards/wood3", new Vector2(3, 3));

            _terrainBillboards.AddEntity(EBillBoard.Pine1, "Content/Textures/Billboards/pine1", new Vector2(3, 3));
            _terrainBillboards.AddEntity(EBillBoard.Pine2, "Content/Textures/Billboards/pine2", new Vector2(3, 3));

            hMesh.LoadContent(_manager.Content);
            //_billboards.AddEntity(EBillBoard.Jungle1, "Content/Textures/forest/wood1", new Vector2(3, 3));
            //_billboards.AddEntity(EBillBoard.Jungle2, "Content/Textures/forest/wood3", new Vector2(3, 3));
            //_billboards.AddEntity(EBillBoard.Jungle3, "Content/Textures/forest/wood5", new Vector2(3, 3));
        }

        public HexPoint Cursor
        {
            get
            {
                return _lastPos;
            }
            set
            {
                if (_lastPos != value)
                {
                    _cursorsMesh.Clear();
                    _cursorsMesh.AddObject(new HexagonMeshItem8X8(MapData.GetWorldPosition(value.X, value.Y), 63), true);

                    _lastPos = value;
                }
            }
        }

        public bool FogOfWarEnabled
        {
            get; set;
        }

        public AbstractPlayerData Human
        {
            get
            {
                return MainWindow.Game.Human;
            }
        }

        public MapData Map
        {
            set
            {
                if (value != null)
                {
                    _map = value;
                    _needToCreate = true;
                }                    
            }
        }

        Improvement farm = null;
        public void UpdateImprovements()
        {
            if (farm == null)
                farm = Provider.GetImprovement("Farm");

            _roadMesh.Clear();
            _farmMesh.Clear();

            // now the tiles
            for (int i = 0; i < _map.Width; i++)
            {
                for (int j = 0; j < _map.Height; j++)
                {
                    if (_map[i, j] == null)
                        continue;

                    int roadTileIndex = _map.GetRoadTileIndex(i, j);

                    if (roadTileIndex != -1)
                        _roadMesh.AddObject(new HexagonMeshItem8X8(MapData.GetWorldPosition(i, j), roadTileIndex), false);

                    if( _map[i,j].Improvements.Contains(farm) )
                        _farmMesh.AddObject(new HexagonMeshItem8X8(MapData.GetWorldPosition(i, j), 1), false);
                }
            }

            _roadMesh.UpdateBuffers();
            _farmMesh.UpdateBuffers();
        }

        public void OnMapSpotting(MapSpottingArgs args)
        {
            _needToUpdateHidden = true;           
        }

        private void UpdateSpotting()
        {
            if (_map == null)
                return;

            _hiddenMeshContainer.Reset();

             // now the tiles
            for (int i = 0; i < _map.Width; i++)
            {
                for (int j = 0; j < _map.Height; j++)
                {
                    if (_map[i, j] == null)
                        continue;

                    if (!_map[i, j].IsSpotted(Human))
                    {
                        // hide terrain
                        TileMatchPattern hiddenPattern = _hiddenMatcher.Match(_map, _map[i, j]);

                        // add to tileset
                        foreach (TileSet set in hiddenPattern.Tiles)
                        //TileSet set = pattern.Tiles.First();
                        {
                            if (set.TilesetIndex == TileMatchPattern.Noindex /* && TilesetIndex < prop.TileCount */)
                            {
                                // set index
                                set.TilesetIndex = _hiddenTilesetIndex++;

                                _hiddenMeshContainer.AddTexture(set.TilesetIndex, set.Tiles);
                            }
                        }

                        int hiddenTileIndex = hiddenPattern.TilesetIndices.Shuffle().First();
                        if (hiddenTileIndex != TileMatchPattern.Noindex)
                        {
                            _hiddenMeshContainer.AddObject(hiddenTileIndex, new HexagonMeshItem8X16(MapData.GetWorldPosition(i, j), hiddenTileIndex), false);
                        }
                        else
                            throw new Exception("No Match");
                    }
                }
            }

            _hiddenMeshContainer.UpdateBuffers();
            _hiddenMeshContainer.LoadContent(_manager.Content);
        }

        HexMesh hMesh;
        static Random rnd = new Random();

        public float CalcHeight(TileHeight h1, TileHeight h2, TileHeight h3)
        {
            int num = 0;
            float sum = 0f;

            if (h1 == TileHeight.Coast || h2 == TileHeight.Coast || h3 == TileHeight.Coast)
                return GetHeight(TileHeight.Coast);

            if (h1 == h2 && h1 != TileHeight.None && h3 != TileHeight.None)
                return ( 3 * GetHeight(h1) + GetHeight(h3) ) / 4f;

            if (h2 == h3 && h2 != TileHeight.None && h1 != TileHeight.None)
                return (3 * GetHeight(h2) + GetHeight(h1)) / 4f;

            if (h1 == h3 && h1 != TileHeight.None && h2 != TileHeight.None)
                return (3 * GetHeight(h1) + GetHeight(h2)) / 4f;

            if (h1 != TileHeight.None)
            {
                sum += GetHeight(h1);
                num++;
            }

            if (h2 != TileHeight.None)
            {
                sum += GetHeight(h2);
                num++;
            }

            if (h3 != TileHeight.None)
            {
                sum += GetHeight(h3);
                num++;
            }

            if (num == 0)
                return 0f;

            return sum / num;
        }

        public float GetHeight(TileHeight h)
        {
            switch (h)
            {
                case TileHeight.Deep:
                    return -4f;
                case TileHeight.Shore:
                    return -1f;
                case TileHeight.Coast:
                    return -0.5f;
                case TileHeight.Zero:
                    return 0.0f;
                case TileHeight.Hill:
                    return 2f;
                case TileHeight.Peak:
                    return 4;
                case TileHeight.None:
                    return -1;
            }

            return -1;
        }

        private void Create()
        {
            if (_map == null)
                return;

            hMesh.Clear();
            _forests.Clear();

            // now the tiles
            for (int i = 0; i < _map.Width; i++)
            {
                for (int j = 0; j < _map.Height; j++)
                {
                    if (_map[i, j] == null)
                        continue;

                    HexPoint neNeighbor = _map[i, j].Point.Neighbor(HexDirection.NorthEast);
                    HexPoint nwNeighbor = _map[i, j].Point.Neighbor(HexDirection.NorthWest);
                    HexPoint eNeighbor = _map[i, j].Point.Neighbor(HexDirection.East);
                    HexPoint wNeighbor = _map[i, j].Point.Neighbor(HexDirection.West);
                    HexPoint seNeighbor = _map[i, j].Point.Neighbor(HexDirection.SouthEast);
                    HexPoint swtNeighbor = _map[i, j].Point.Neighbor(HexDirection.SouthWest);

                    Hexagon hex = _manager.Content.Load<Hexagon>("Content\\Data\\Hexagon");

                    hex.Move(MapData.GetWorldPosition(i, j));

                    #region top
                    TileType top = TileType.None;
                    top |= _map[i, j].TileType;
                    top |= _map.IsValid(neNeighbor) ? _map[neNeighbor].TileType : TileType.None;
                    top |= _map.IsValid(nwNeighbor) ? _map[nwNeighbor].TileType : TileType.None;

                    float topH = CalcHeight(
                        _map[i, j].TileHeight, 
                        _map.IsValid(neNeighbor) ? _map[neNeighbor].TileHeight : TileHeight.None, 
                        _map.IsValid(nwNeighbor) ? _map[nwNeighbor].TileHeight : TileHeight.None);
                    #endregion top

                    #region topRight
                    TileType topRight = TileType.None;
                    topRight |= _map[i, j].TileType;
                    topRight |= _map.IsValid(neNeighbor) ? _map[neNeighbor].TileType : TileType.None;
                    topRight |= _map.IsValid(eNeighbor) ? _map[eNeighbor].TileType : TileType.None;

                    float topRightH = CalcHeight(
                        _map[i, j].TileHeight,
                        _map.IsValid(neNeighbor) ? _map[neNeighbor].TileHeight : TileHeight.None,
                        _map.IsValid(eNeighbor) ? _map[eNeighbor].TileHeight : TileHeight.None);
                    #endregion topRight

                    #region bottomRight
                    TileType bottomRight = TileType.None;
                    bottomRight |= _map[i, j].TileType;
                    bottomRight |= _map.IsValid(eNeighbor) ? _map[eNeighbor].TileType : TileType.None;
                    bottomRight |= _map.IsValid(seNeighbor) ? _map[seNeighbor].TileType : TileType.None;

                    float bottomRightH = CalcHeight(
                        _map[i, j].TileHeight,
                        _map.IsValid(eNeighbor) ? _map[eNeighbor].TileHeight : TileHeight.None,
                        _map.IsValid(seNeighbor) ? _map[seNeighbor].TileHeight : TileHeight.None);
                    #endregion bottomRight

                    #region bottom
                    TileType bottom = TileType.None;
                    bottom |= _map[i, j].TileType;
                    bottom |= _map.IsValid(seNeighbor) ? _map[seNeighbor].TileType : TileType.None;
                    bottom |= _map.IsValid(swtNeighbor) ? _map[swtNeighbor].TileType : TileType.None;

                    float bottomH = CalcHeight(
                        _map[i, j].TileHeight,
                        _map.IsValid(seNeighbor) ? _map[seNeighbor].TileHeight : TileHeight.None,
                        _map.IsValid(swtNeighbor) ? _map[swtNeighbor].TileHeight : TileHeight.None);
                    #endregion bottom

                    #region bottomLeft
                    TileType bottomLeft = TileType.None;
                    bottomLeft |= _map[i, j].TileType;
                    bottomLeft |= _map.IsValid(swtNeighbor) ? _map[swtNeighbor].TileType : TileType.None;
                    bottomLeft |= _map.IsValid(wNeighbor) ? _map[wNeighbor].TileType : TileType.None;

                    float bottomLeftH = CalcHeight(
                        _map[i, j].TileHeight,
                        _map.IsValid(swtNeighbor) ? _map[swtNeighbor].TileHeight : TileHeight.None,
                        _map.IsValid(wNeighbor) ? _map[wNeighbor].TileHeight : TileHeight.None);
                    #endregion bottomLeft

                    #region topLeft
                    TileType topLeft = TileType.None;
                    topLeft |= _map[i, j].TileType;
                    topLeft |= _map.IsValid(wNeighbor) ? _map[wNeighbor].TileType : TileType.None;
                    topLeft |= _map.IsValid(nwNeighbor) ? _map[nwNeighbor].TileType : TileType.None;

                    float topLeftH = CalcHeight(
                        _map[i, j].TileHeight,
                        _map.IsValid(wNeighbor) ? _map[wNeighbor].TileHeight : TileHeight.None,
                        _map.IsValid(nwNeighbor) ? _map[nwNeighbor].TileHeight : TileHeight.None);
                    #endregion topLeft

                    hex.Apply(top, topRight, bottomRight, bottom, bottomLeft, topLeft, _map[i, j].TileType);
                    hex.Apply(topH, topRightH, bottomRightH, bottomH, bottomLeftH, topLeftH, GetHeight( _map[i, j].TileHeight));

                    hMesh.AddHexagon(hex, false);

                    if( _map[i, j].IsForest )
                        _forests.Add(new ForestEntity(_map[i, j].Point));
                }
            }

            hMesh.UpdateBuffers();

            UpdateSpotting();

            _terrainBillboards.Build();
        }

        public void OnUpdateBorders(MapControllingArgs args)
        {
            _needToUpdateBorders = true;
        }

        public void UpdateBorders()
        {
            if (_map == null)
                return;

            _borderMesh.Clear();

            // now the tiles
            for (int i = 0; i < _map.Width; i++)
            {
                for (int j = 0; j < _map.Height; j++)
                {
                    if (_map[i, j] == null)
                        continue;

                    int borderTileIndex = _map.GetBorderTileIndex(i,j);

                    if( borderTileIndex != 0 )
                        _borderMesh.AddObject( new HexagonMeshItem8X8( MapData.GetWorldPosition(i,j), borderTileIndex ), false );
                }
            }
            
            _borderMesh.UpdateBuffers();
        }

        public void Update(GameTime gameTime)
        {
            if (_needToCreate)
            {
                Create();
                _needToCreate = false;
            }

            if (_needToUpdateHidden)
            {
                UpdateSpotting();
                _needToUpdateHidden = false;
            }

            if (_needToUpdateBorders)
            {
                UpdateBorders();
                _needToUpdateBorders = false;
            }

            _baseMesh.Update(gameTime);
            _cursorsMesh.Update(gameTime);
            _farmMesh.Update(gameTime);
            hMesh.Update(gameTime);

            foreach (AbstractPlayerData pl in MainWindow.Game.Players)
                pl.Update(gameTime);
        }

        public void Draw(GameTime gameTime, ArcBallCamera camera)
        {
            if (_map != null)
            {
                hMesh.Draw(gameTime, camera.View, camera.Projection, camera.Position);
                _cursorsMesh.Draw(gameTime, camera.View, camera.Projection, Vector3.Zero);
                _borderMesh.Draw(gameTime, camera.View, camera.Projection, Vector3.Zero);
                _farmMesh.Draw(gameTime, camera.View, camera.Projection, Vector3.Zero);
                _roadMesh.Draw(gameTime, camera.View, camera.Projection, Vector3.Zero);
                
                //foreach (ForestEntity forest in _forests)
                //    if (forest.Point.DistanceTo(Center) < 15)
                //        forest.Draw(gameTime);

                if (FogOfWarEnabled)
                    _hiddenMeshContainer.Draw(gameTime, camera.View, camera.Projection, Vector3.Zero);

                foreach (AbstractPlayerData pl in MainWindow.Game.Players)
                    pl.Draw(gameTime);

                _manager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
        }
    }
}
