using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace MiRo.SimHexWorld.Engine.World
{
    public class TiledMeshContainer
    {
        public static int _internCounter = 0;

        private class TiledMeshItem
        {
            private const int TextureId = 0;
            private const TextureManager.TileMatrix Matrix = TextureManager.TileMatrix.Items8X16;

            readonly Manager _manager;
            readonly List<Mesh> _groundMeshs = new List<Mesh>();

            int _instanceId;
            private int _vertices = 0;

            public TiledMeshItem(Manager manager, int instanceId)
            {
                _manager = manager;

                _instanceId = instanceId;

                if( TextureManager.Instance[TextureName] == null )
                    TextureManager.Instance.Create(TextureName, Matrix);

                _groundMeshs.Add(new Mesh(_manager.GraphicsDevice, TextureName));
            }

            private string TextureName
            {
                get
                {
                    return string.Format("generated_{0}_{1}", TextureId, _instanceId);
                }
            }

            public void AddTexture(int index, IEnumerable<string> textures)
            {
                int normalizedIndex = index;// -_range.From;

                foreach (string tileStr in textures)
                    TextureManager.Instance.Add(TextureName, Matrix, normalizedIndex, _manager.Content.Load<Texture2D>("Content/Textures/Ground/Tiles/" + tileStr));
            }

            public void Reset()
            {
                foreach( var mesh in _groundMeshs)
                    mesh.Clear();
            }

            public void UpdateBuffers()
            {
                foreach (var mesh in _groundMeshs)
                    mesh.UpdateBuffers();
            }

            public void LoadContent(ContentManager content)
            {
                foreach (var mesh in _groundMeshs)
                    mesh.LoadContent(content);
            }

            public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3 translation)
            {
                foreach (var mesh in _groundMeshs)
                    mesh.Draw(gameTime, view, projection, translation);
            }

            public bool AddObject(IMeshObject obj, bool updateBuffers)
            {
                // check if we have too much vertices
                if (_vertices + obj.Vertices.Count > 65535)
                    return false;
                
                _vertices += obj.Vertices.Count;
                _groundMeshs.First().AddObject(obj, updateBuffers);

                return true;
            }
        }

        readonly List<TiledMeshItem> _items = new List<TiledMeshItem>();
        private readonly Manager _manager;

        public TiledMeshContainer(Manager manager)
        {
            _internCounter++;
            _manager = manager;

            _items.Add(new TiledMeshItem(manager, _internCounter));
        }

        public void Reset()
        {
            foreach (TiledMeshItem item in _items)
                item.Reset();
        }

        public void AddTexture(int index, List<string> tiles)
        {
            foreach (TiledMeshItem item in _items) 
                item.AddTexture(index, tiles);
        }

        public void UpdateBuffers()
        {
            foreach (TiledMeshItem item in _items)
                item.UpdateBuffers();
        }

        public void LoadContent(ContentManager content)
        {
            foreach (TiledMeshItem item in _items)
                item.LoadContent(content);
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3 translation)
        {
            foreach (TiledMeshItem item in _items)
                item.Draw(gameTime, view, projection, translation);
        }

        public void AddObject(int index, IMeshObject obj, bool updateBuffers = true)
        {
            //log.InfoFormat("AddTexture({0}, ...)", index);
            TiledMeshItem item = _items.LastOrDefault();

            if (!item.AddObject(obj, updateBuffers))
            {
                TiledMeshItem newItem = new TiledMeshItem(_manager, _internCounter);

                newItem.AddObject(obj, updateBuffers);

                _items.Add(newItem);
            }
        }
    }
}