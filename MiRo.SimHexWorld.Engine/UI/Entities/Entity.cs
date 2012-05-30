using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.UI.Controls;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Helper;
using NUnit.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    interface IEntity
    {
        void Update(GameTime time);
        void Draw(GameTime time);
    }

    public abstract class Entity : IEntity
    {
        protected static Random rand = new Random();
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        HexPoint _point;
        public HexPoint Point
        {
            get
            {
                return _point;
            }
            set 
            {
                _point = value;
                Position = MapData.GetWorldPosition(value); 
            }
        }

        public abstract void Update(GameTime time);

        public abstract void Draw(GameTime time);
    }
}
