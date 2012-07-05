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
        Model _model;
        Matrix _scaleMatrix;

        private Dictionary<string, ModelAnimation> _animations = new Dictionary<string, ModelAnimation>();
        private ModelAnimation _currentAnimation;

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

            _model = MainApplication.ManagerInstance.Content.Load<Model>("Content\\Models\\" + name);
            _animations.Add("Idle", new ModelAnimation("Content\\Animations\\" + name + "\\Idle"));

            Status = ModelStatus.Standing;

            for (int i = 0; i < unit.Formation.Positions; ++i)
            {
                Vector3 startOffset = unit.Formation.GetPosition(i);

                _items.Add(new UnitItem(Position + startOffset, startOffset, Rotation));
            }

            _scaleMatrix = Matrix.CreateScale(Scale * 0.3f);

            int boneCount = _model.Bones.Count;

            this.boneTransforms = new Matrix[boneCount];
            _model.CopyBoneTransformsTo(this.boneTransforms);

            this.boneTransformsOriginal = new Matrix[boneCount];
            _model.CopyBoneTransformsTo(this.boneTransformsOriginal);

            this.absoluteBoneTransforms = new Matrix[boneCount];

            SetAnimation("Idle");
        }

        public void SetAnimation(string name)
        {
            _currentAnimation = _animations[name];
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

                _scaleMatrix = Matrix.CreateScale(Scale * 0.6f);
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

            if (_currentAnimation != null)
                _currentAnimation.Update(gameTime);

            bool allItemsReady = true;

            foreach (UnitItem item in _items)
                allItemsReady &= item.Animation == null || item.Animation.Ready;

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
                        AnimateUpdate(item, gameTime);
                    break;
                case Types.UnitAction.Move:
                    switch (Status)
                    {
                        case ModelStatus.Standing:
                            if (_unit.Path == null || _unit.Path.Finished)
                                return;

                            if (allItemsReady)
                            {
                                HexDirection dir = Point.GetDirection(_unit.Path.Peek);

                                foreach (UnitItem item in _items)
                                    StartMoveRotate(item, dir);

                                Status = ModelStatus.Rotating;
                            }
                            else
                                foreach (UnitItem item in _items)
                                    AnimateUpdate(item, gameTime);

                            break;
                        case ModelStatus.Rotating:

                            if (allItemsReady)
                            {
                                foreach (UnitItem item in _items)
                                    StartMoveMove(item, TargetPosition);

                                Status = ModelStatus.Moving;
                            }

                            foreach (UnitItem item in _items)
                                AnimateUpdate(item, gameTime);
                            break;
                        case ModelStatus.Moving:
                            if (allItemsReady)
                            {
                                HexPoint pt = _unit.Path.Peek;
                                _unit.Path.Pop();
                                _unit.Move(pt);
                                Point = pt;

                                Status = ModelStatus.Standing;
                            }

                            foreach (UnitItem item in _items)
                                AnimateUpdate(item, gameTime);

                            break;
                    }
                    break;
            }
        }

        private void StartMoveMove(UnitItem item, Vector3 TargetPosition)
        {
            item.Animation = new ObjectAnimation(
                item.Position,
                TargetPosition,
                item.Rotation,
                item.Rotation,
                TimeSpan.FromSeconds(0.5f + item.StartOffset), false);
        }

        private void AnimateUpdate(UnitItem item, GameTime gameTime)
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

        private void StartMoveRotate(UnitItem item, HexDirection dir)
        {
            HexPoint next = Point.Clone();
            next.MoveDir(dir);

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

        public void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            // Set the world matrix as the root transform of the model.
            model.Root.Transform = world;

            // Look up combined bone matrices for the entire model.
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshAnimation = _currentAnimation != null ? _currentAnimation.GetRotation(mesh.ParentBone.Name) : Matrix.Identity;

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * meshAnimation;
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

                Matrix tmpMatrix = _scaleMatrix
                    * Matrix.CreateRotationY(anim != null ? anim.Rotation.Y : item.Rotation.Y)
                    * Matrix.CreateTranslation(anim != null ? anim.Position : item.Position);

                DrawModel(_model, tmpMatrix, GameMapBox.Camera.View, GameMapBox.Camera.Projection);
            }
        }
    }
}
