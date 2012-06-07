using System;
using System.Collections.Generic;
using MiRo.SimHexWorld.Helper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Windows.Forms;

namespace MiRo.SimHexWorld.Engine.World
{
    public class TextureManager
    {
        GraphicsDevice _device;
        public enum TileMatrix { Items8X8, Items8X16, Items1X1 };

        readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        readonly Dictionary<string, RenderTarget2D> _targets = new Dictionary<string, RenderTarget2D>();

        public class TileMatrixProperties
        {
            public int Width, Height;
            public int TileWidth, TileHeight;
            public int TilesPerRow, TilesPerColumn;

            public int TileCount
            {
                get { return TilesPerRow * TilesPerColumn; }
            }
        }

        private static TextureManager _instance = null;

        public static TextureManager Instance
        {
            get { return _instance ?? (_instance = new TextureManager()); }
        }

        public GraphicsDevice Device
        {
            get { return _device; }
            set
            {
                _device = value;
            }
        }

        public void Add(string name, Texture2D texture)
        {
            if (!_textures.ContainsKey(name))
                _textures.Add(name, texture);
        }

        public void SaveAsPng(string name, string filename)
        {
            if (_targets.ContainsKey(name))
            {
                RenderTarget2D target = _targets[name];
                using (Stream stream = File.OpenWrite(filename))
                {
                    target.SaveAsPng(stream, target.Width, target.Height);
                }
            }
        }

        public void Create(string name, TileMatrix matrix)
        {
            if (!_targets.ContainsKey(name))
            {
                TileMatrixProperties prop = GetProperties(matrix);

                _targets.Add(name, new RenderTarget2D(_device, prop.Width, prop.Height, false, SurfaceFormat.Color, DepthFormat.Depth24));

                RenderTarget2D target = _targets[name];

                _device.SetRenderTarget(target);
                _device.Clear(Color.Transparent);
                _device.SetRenderTarget(null);

                _targets[name] = target;
            }
            else
                throw new Exception("Target '" + name + "' already exist");
        }

        public static TileMatrixProperties GetProperties(TileMatrix matrix)
        {
            switch (matrix)
            {
                case TileMatrix.Items1X1:
                    {
                        var prop = new TileMatrixProperties();
                        prop.Width = 128;
                        prop.Height = 128;

                        prop.TilesPerRow = 1;
                        prop.TilesPerColumn = 1;

                        prop.TileWidth = prop.Width / prop.TilesPerRow;
                        prop.TileHeight = prop.Height / prop.TilesPerColumn;
                        return prop;
                    }
                case TileMatrix.Items8X8:
                    {
                        var prop = new TileMatrixProperties();
                        prop.Width = 1024;
                        prop.Height = 1024;

                        prop.TilesPerRow = 8;
                        prop.TilesPerColumn = 8;

                        prop.TileWidth = prop.Width / prop.TilesPerRow;
                        prop.TileHeight = prop.Height / prop.TilesPerColumn;
                        return prop;
                    }
                case TileMatrix.Items8X16:
                    {
                        var prop = new TileMatrixProperties();
                        prop.Width = 1024;
                        prop.Height = 2048;

                        prop.TilesPerRow = 8;
                        prop.TilesPerColumn = 16;

                        prop.TileWidth = prop.Width / prop.TilesPerRow;
                        prop.TileHeight = prop.Height / prop.TilesPerColumn;
                        return prop;
                    }
            }

            return null;
        }

        private Vector2 GetIndex(TileMatrix matrix, int index)
        {
            TileMatrixProperties prop = GetProperties(matrix);

            return new Vector2(index % prop.TilesPerRow, index / prop.TilesPerRow);
        }

        private static bool memoryErrorOccured = false;
        public void Add(string name, TileMatrix matrix, int index, Texture2D texture)
        {
            if (!_targets.ContainsKey(name))
                throw new Exception("Target '" + name + "' does not exist");

            if( memoryErrorOccured )
                return;

            try
            {
                RenderTarget2D target = _targets[name];
                TileMatrixProperties prop = GetProperties(matrix);

                var tmpTarget = new RenderTarget2D(_device, prop.Width, prop.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                Vector2 pos = GetIndex(matrix, index);

                _device.SetRenderTarget(tmpTarget);

                _device.Clear(Color.Transparent);

                try
                {
                    using (var spriteBatch = new SpriteBatch(_device))
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(target, Vector2.Zero, Color.White);
                        spriteBatch.Draw(texture, new Rectangle((int)pos.X * prop.TileWidth, (int)pos.Y * prop.TileHeight, prop.TileWidth, prop.TileHeight), null, Color.White);
                        spriteBatch.End();
                    }
                }
                catch { }

                target.Dispose();
                target = null;

                _device.SetRenderTarget(null);

                _targets[name] = null;
                _targets[name] = tmpTarget;
                //tmpTarget.Dispose();
                //tmpTarget = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during Texture generation: " + GC.GetTotalMemory(true) + ", " + GC.CollectionCount(0), "Error");
                memoryErrorOccured = true;
            }
        }

        public TextureManager()
        {
        }

        public Texture2D this[string name]
        {
            get
            {
                if (_textures.ContainsKey(name))
                    return _textures[name];

                if (_targets.ContainsKey(name))
                    return _targets[name];

                return null;
            }
        }

        public static Vector2Rectangle GetRectangle(int tilex, int tiley, int width, int height)
        {
            return new Vector2Rectangle((float)tilex / (float)width, (float)tiley / (float)height, (float)(tilex + 1) / (float)width, (float)(tiley + 1) / (float)height);
        }
    }
}
