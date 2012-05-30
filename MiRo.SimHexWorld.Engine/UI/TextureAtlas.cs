using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace MiRo.SimHexWorld.Engine.UI
{
    public class TextureAtlas
    {
        public class AtlasPosition
        {
            public string Name { get; set; }
            public int Index { get; set; }

            [ContentSerializer(Optional = true)]
            public bool Default { get; set; }
        }

        [ContentSerializerIgnore]
        public Texture2D _texture { get; set; }

        public string Name { get; set; }
        public string TextureName { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        private bool _initialized = false;
        private int _width = 0;
        private int _height = 0;

        public List<AtlasPosition> Tiles { get; set; }
        private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public void Initialize()
        {
            _texture = MainApplication.Instance.Content.Load<Texture2D>("Content\\Textures\\Atlases\\" + TextureName);
            _width = _texture.Width / Columns;
            _height = _texture.Height / Rows;

            _initialized = true;
        }

        public int MaxTiles
        {
            get
            {
                return Rows * Columns;
            }
        }

        public string DefaultTextureName
        {
            get
            {
                return Tiles.FirstOrDefault(a => a.Default).Name;
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (!_initialized)
                Initialize();

            if (_textures.ContainsKey(name))
                return _textures[name];
            else
            {
                Rectangle r = RectangleFromName(name);

                if (r.Width > 1)
                {
                    Color[] colors = new Color[_width * _height];

                    _texture.GetData<Color>(0, r, colors, 0, _width * _height);

                    Texture2D tex = new Texture2D(MainApplication.Instance.GraphicsDevice, _width, _height);

                    tex.SetData<Color>(0, new Rectangle(0, 0, _width, _height), colors, 0, _width * _height);

                    _textures.Add(name, tex);

                    return tex;
                }

                return null;
            }
        }

        public void SaveAsPNG(string filename)
        {
            if (!_initialized)
                Initialize();

            using (Stream stream = File.OpenWrite(filename))
            {
                _texture.SaveAsPng(stream, _texture.Width, _texture.Height);
            }
        }

        public Texture2D GetDefaultTexture()
        {
            return GetTexture(DefaultTextureName);
        }

        public bool HasIdentifier(string name)
        {
            return Tiles.Count( a => a.Name == name ) > 0;
        }

        public int IndexFromDefault()
        {
            if (Tiles.Count(a => a.Default) == 0)
                return -1;

            return Tiles.FirstOrDefault(a => a.Default).Index;
        }

        public Rectangle RectangleFromDefault()
        {
            return RectangleFromIndex(IndexFromDefault());
        }

        public Rectangle RectangleFromIndex(int index)
        {
            if (!_initialized)
                Initialize();

            if (index < 0 || index >= MaxTiles)
                return new Rectangle();

            int row = (int)((float)index / (float)Columns);
            int column = index % Columns;

            return new Rectangle(_width * column, _height * row, _width, _height);
        }

        public Rectangle RectangleFromName(string name)
        {
            return RectangleFromIndex(IndexFromName(name));
        }

        public int IndexFromName(string name)
        {
            if( Tiles.Count( a => a.Name == name ) == 0)
                return -1;

            return Tiles.FirstOrDefault(a => a.Name == name).Index;
        }
    }
}
