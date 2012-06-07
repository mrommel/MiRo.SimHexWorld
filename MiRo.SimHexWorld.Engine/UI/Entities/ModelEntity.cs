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
    public class ModelEntity : Entity
    {
        AbstractPlayerData _player;
        ObjectAnimation anim;
        public enum ModelStatus { Standing, Moving, Rotating }

        Model model, box;

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

        Unit _parent;

        public ModelEntity(AbstractPlayerData player, Unit parent, string name)
        {
            Point = parent.Point;
            _parent = parent;
            _player = player;

            model = MainApplication.ManagerInstance.Content.Load<Model>("Content\\Models\\" + name);
            box = MainApplication.ManagerInstance.Content.Load<Model>("Content\\Models\\box");
            Status = ModelStatus.Standing;

            int boneCount = model.Bones.Count;

            this.boneTransforms = new Matrix[boneCount];
            model.CopyBoneTransformsTo(this.boneTransforms);

            this.boneTransformsOriginal = new Matrix[boneCount];
            model.CopyBoneTransformsTo(this.boneTransformsOriginal);

            this.absoluteBoneTransforms = new Matrix[boneCount];
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
                if (_parent.Path == null || _parent.Path.Finished)
                    return MapData.GetWorldPosition(_parent.Point);

                return MapData.GetWorldPosition(_parent.Path.Peek);
           }
        }

        public override void Update(GameTime gameTime)
        {
            if (MainWindow.Game.Map == null)
                return;

            switch (_parent.Action)
            {
                case Types.UnitAction.Idle:
                    if (anim == null || anim.Ready)
                    {
                        foreach (HexDirection dir in HexDirection.All.Shuffle())
                        {
                            HexPoint next = Point.Clone();
                            next.MoveDir(dir);

                            _parent.SetTarget( next );

                            Assert.IsTrue(Point.IsNeighbor(next));

                            if (!MainWindow.Game.Map.IsValid(next))
                                continue;

                            if (MainWindow.Game.Map[next].IsOcean && !MainWindow.Game.Map[Point].IsOcean)
                                continue;

                            anim = new ObjectAnimation(
                                Position,
                                Position,
                                Rotation,
                                new Vector3(0, dir.Angle + _parent.Data.ModelRotation, 0),
                                TimeSpan.FromSeconds(0.5f), false);

                            break;
                        }
                    }
                    else
                        anim.Update(gameTime);
                    break;
                case Types.UnitAction.Move:
                    switch (Status)
                    {
                        case ModelStatus.Standing:
                            if (_parent.Path == null || _parent.Path.Finished)
                                return;

                            HexDirection dir = Point.GetDirection(_parent.Path.Peek);

                            anim = new ObjectAnimation(
                                Position,
                                Position,
                                Rotation,
                                new Vector3(0, dir.Angle + _parent.Data.ModelRotation, 0),
                                TimeSpan.FromSeconds(0.3f), false);

                            Status = ModelStatus.Rotating;

                            break;
                        case ModelStatus.Rotating:
                            if (anim != null)
                            {
                                anim.Update(gameTime);
                                if (anim.Ready)
                                {
                                    Status = ModelStatus.Moving;
                                    Rotation = anim.Rotation;
                                    anim = new ObjectAnimation(
                                        Position,
                                        TargetPosition,
                                        Rotation,
                                        Rotation,
                                        TimeSpan.FromSeconds(0.2f), false);
                                }
                            }
                            break;
                        case ModelStatus.Moving:
                            if (anim != null)
                            {
                                anim.Update(gameTime);
                                if (anim.Ready)
                                {
                                    HexPoint pt = _parent.Path.Peek;                                   
                                    _parent.Path.Pop();
                                    _parent.Move(pt);
                                    Point = pt;

                                    anim = null;
                                    Status = ModelStatus.Standing;
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        public override void Draw(GameTime time)
        {
            calculateAbsoluteBoneTransforms();

            Matrix wMatrix =
                Matrix.CreateScale(Scale) *
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(anim != null ? anim.Rotation.Y : Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z) *
                Matrix.CreateTranslation(anim != null ? anim.Position : Position);

            //model.Draw(wMatrix, GameMapBox.Camera.View, GameMapBox.Camera.Projection);
            int meshCount = this.model.Meshes.Count;
            for (int meshIndex = 0; meshIndex < meshCount; meshIndex++)
            {
                ModelMesh mesh = this.model.Meshes[meshIndex];
                int parentBoneIndex = mesh.ParentBone.Index;

                int effectCount = mesh.Effects.Count;
                for (int effectIndex = 0; effectIndex < effectCount; effectIndex++)
                {
                    Effect effect = mesh.Effects[effectIndex];
                    if (effect == null)
                    {
                        continue; // Model.Draw() would throw an exception in this case
                    }

                    // Hand the mesh's transformation matrices to the effect
                    IEffectMatrices matrices = effect as IEffectMatrices;
                    if (matrices != null)
                    {
                        matrices.World = this.absoluteBoneTransforms[parentBoneIndex] * wMatrix;
                        matrices.View = GameMapBox.Camera.View;
                        matrices.Projection = GameMapBox.Camera.Projection;
                    }
                }

                MainApplication.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                mesh.Draw();
            }

            Matrix wMatrix2 = Matrix.CreateTranslation(TargetPosition);

            //box.Draw(wMatrix2, GameMapBox.Camera.View, GameMapBox.Camera.Projection);
        }

        /// <summary>Calculates the absolute bone transformation matrices in model space</summary>
        private void calculateAbsoluteBoneTransforms()
        {
            // Obtain the local transform for the bind pose of all bones
            this.model.CopyBoneTransformsTo(this.absoluteBoneTransforms);

            // Convert the relative bone transforms into absolute transforms
            ModelBoneCollection bones = this.model.Bones;
            for (int index = 0; index < bones.Count; ++index)
            {
                // Apply the bone's user-specified transform
                this.absoluteBoneTransforms[index] =
                  this.boneTransforms[index] * bones[index].Transform;

                // Calculate the absolute transform of the bone in model space.
                // Content processors sort bones so that parent bones always appear
                // before their children, thus this works like a matrix stack,
                // resolving the full bone hierarchy in minimal steps.
                ModelBone bone = bones[index];
                if (bone.Parent != null)
                {
                    int parentIndex = bone.Parent.Index;
                    this.absoluteBoneTransforms[index] *= this.absoluteBoneTransforms[parentIndex];
                }
            }
        }
    }
}
