using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.World.Helper;
using NUnit.Framework;
using MiRo.SimHexWorld.Engine.UI.Controls;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    public class UnitItem
    {
        private static Random rnd = new Random();

        public UnitItem(Vector3 position, Vector3 relativPosition, Vector3 rotation)
        {
            Position = position;
            RelativPosition = relativPosition;

            Rotation = rotation;

            StartOffset = (float)rnd.NextDouble() * 0.5f - 0.25f;
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 RelativPosition { get; set; }
        public ObjectAnimation Animation { get; set; }
        public float StartOffset { get; set; }
    }

    public class UnitEntity : Entity
    {
        AbstractPlayerData _player;
        //ObjectAnimation anim;
        public enum ModelStatus { Standing, Moving, Rotating }

        List<UnitItem> _items = new List<UnitItem>();
        Model model;
        Matrix scaleMatrix;

        /// <summary>Transforms being applied to this model instance only</summary>
        private Matrix[] boneTransforms;
        private Matrix[] boneTransformsOriginal;

        /// <summary>Absolute transform matrices for all bones in model space</summary>
        /// <remarks>
        ///   The contents of this field are recreated on-the-fly during each Draw() call,
        ///   but to avoid feeding the gargabe collector by creating a new temporary
        ///   array each time, we keep reusing this one.
        /// </remarks>
        private Matrix[] absoluteBoneTransforms;

        Unit _unit;

        public UnitEntity(AbstractPlayerData player, Unit unit, string name)
        {
            Point = unit.Point;
            _unit = unit;
            _player = player;

            model = MainApplication.ManagerInstance.Content.Load<Model>("Content\\Models\\" + name);
            Status = ModelStatus.Standing;

            for (int i = 0; i < unit.Formation.Positions; ++i)
            {
                Vector3 startOffset = unit.Formation.GetPosition(i);

                _items.Add(new UnitItem(Position + startOffset, startOffset, Rotation));
            }

            scaleMatrix = Matrix.CreateScale(Scale * 0.3f);

            int boneCount = model.Bones.Count;

            this.boneTransforms = new Matrix[boneCount];
            model.CopyBoneTransformsTo(this.boneTransforms);

            this.boneTransformsOriginal = new Matrix[boneCount];
            model.CopyBoneTransformsTo(this.boneTransformsOriginal);

            this.absoluteBoneTransforms = new Matrix[boneCount];
        }

        public override Vector3 Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                base.Scale = value;

                scaleMatrix = Matrix.CreateScale(Scale * 0.3f);
            }
        }

        public ModelStatus Status
        {
            get;
            set;
        }

        public Vector3 TargetPosition
        {
            get
            {
                if (_unit.Path == null || _unit.Path.Finished)
                    return MapData.GetWorldPosition(_unit.Point);

                return MapData.GetWorldPosition(_unit.Path.Peek);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (MainWindow.Game.Map == null)
                return;

            bool allItemsReady = true;

            foreach (UnitItem item in _items)
                allItemsReady &= item.Animation == null || item.Animation.Ready;

            //// health check
            //if (allItemsReady)
            //{
            //    // they should all have the same angle, right?
            //    bool sameAngle = true;
            //    float angle = _items[0].Rotation.Y;

            //    foreach (UnitItem item in _items)
            //        sameAngle &= angle == item.Rotation.Y;

            //    Assert.IsTrue(sameAngle, "The Items should have the same angle after animation");
            //}

            switch (_unit.Action)
            {
                case Types.UnitAction.Idle:

                    if (allItemsReady)
                    {
                        HexDirection dir = HexDirection.All.Shuffle().First();

                        foreach (UnitItem item in _items)
                            StartIdle(item, dir);
                    }

                    foreach (UnitItem item in _items)
                        AnimateIdle(item, gameTime);
                    break;
                case Types.UnitAction.Move:
                    //switch (Status)
                    //{
                    //    case ModelStatus.Standing:
                    //        if (_unit.Path == null || _unit.Path.Finished)
                    //            return;

                    //        HexDirection dir = Point.GetDirection(_unit.Path.Peek);

                    //        anim = new ObjectAnimation(
                    //            Position,
                    //            Position,
                    //            Rotation,
                    //            new Vector3(0, dir.Angle + _unit.Data.ModelRotation, 0),
                    //            TimeSpan.FromSeconds(0.3f), false);

                    //        Status = ModelStatus.Rotating;

                    //        break;
                    //    case ModelStatus.Rotating:
                    //        if (anim != null)
                    //        {
                    //            anim.Update(gameTime);
                    //            if (anim.Ready)
                    //            {
                    //                Status = ModelStatus.Moving;
                    //                Rotation = anim.Rotation;
                    //                anim = new ObjectAnimation(
                    //                    Position,
                    //                    TargetPosition,
                    //                    Rotation,
                    //                    Rotation,
                    //                    TimeSpan.FromSeconds(0.2f), false);
                    //            }
                    //        }
                    //        break;
                    //    case ModelStatus.Moving:
                    //        if (anim != null)
                    //        {
                    //            anim.Update(gameTime);
                    //            if (anim.Ready)
                    //            {
                    //                HexPoint pt = _unit.Path.Peek;                                   
                    //                _unit.Path.Pop();
                    //                _unit.Move(pt);
                    //                Point = pt;

                    //                anim = null;
                    //                Status = ModelStatus.Standing;
                    //            }
                    //        }
                    //        break;
                    //}
                    break;
            }
        }

        private void StartIdle(UnitItem item, HexDirection dir)
        {
            HexPoint next = Point.Clone();
            next.MoveDir(dir);

            //_unit.SetTarget(next);

            float rotationY = (float)Point.AngleRad(next) + _unit.Data.ModelRotation;
            rotationY %= (float)Math.PI * 2;
            Vector3 newRotation = new Vector3(0, rotationY, 0);
            Vector3 delta = Vector3.Transform(item.RelativPosition, Matrix.CreateRotationY(rotationY));
            Vector3 newPosition = Position + delta;

            item.Animation = new ObjectAnimation(
                item.Position,
                newPosition,
                item.Rotation,
                newRotation,
                TimeSpan.FromSeconds(0.5f + item.StartOffset), false);
        }

        /// <summary>
        /// idle animation (turning around)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="gameTime"></param>
        private void AnimateIdle(UnitItem item, GameTime gameTime)
        {
            if (item.Animation != null && item.Animation.Ready)
            {
                item.Position = item.Animation.Position;
                item.Rotation = item.Animation.Rotation;

                item.Animation = null;
            }

            if (item.Animation != null)
                item.Animation.Update(gameTime);
        }

        public void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            // Set the world matrix as the root transform of the model.
            model.Root.Transform = world;

            // Look up combined bone matrices for the entire model.
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        public override void Draw(GameTime time)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                UnitItem item = _items[i];
                ObjectAnimation anim = item.Animation; 

                Matrix tmpMatrix = scaleMatrix
                    * Matrix.CreateRotationY(anim != null ? anim.Rotation.Y : item.Rotation.Y)
                    * Matrix.CreateTranslation(anim != null ? anim.Position : item.Position);

                DrawModel(model, tmpMatrix, GameMapBox.Camera.View, GameMapBox.Camera.Projection);
            }
        }
    }
}
