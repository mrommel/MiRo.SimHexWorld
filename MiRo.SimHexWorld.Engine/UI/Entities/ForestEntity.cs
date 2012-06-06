using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    public class ForestEntity : Entity
    {
        static Model model;

        private bool[] boneVisible;

        /// <summary>Transforms being applied to this model instance only</summary>
        private Matrix[] boneTransforms;

        /// <summary>Absolute transform matrices for all bones in model space</summary>
        /// <remarks>
        ///   The contents of this field are recreated on-the-fly during each Draw() call,
        ///   but to avoid feeding the gargabe collector by creating a new temporary
        ///   array each time, we keep reusing this one.
        /// </remarks>
        private Matrix[] absoluteBoneTransforms;

        public ForestEntity(HexPoint point)
        {
            if( model == null )
                model = MainApplication.ManagerInstance.Content.Load<Model>("Content\\Models\\forest");

            Point = point;
            Scale = new Vector3(0.01f);
            Rotation = new Vector3(0, (float)(rand.NextDouble() * Math.PI * 2), 0);

            int boneCount = model.Bones.Count;
            boneVisible = Enumerable.Repeat<bool>(true, boneCount).ToArray();

            this.boneTransforms = new Matrix[boneCount];
            model.CopyBoneTransformsTo(this.boneTransforms);

            this.absoluteBoneTransforms = new Matrix[boneCount];
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        /// <summary>Calculates the absolute bone transformation matrices in model space</summary>
        private void calculateAbsoluteBoneTransforms()
        {
            // Obtain the local transform for the bind pose of all bones
            model.CopyBoneTransformsTo(this.absoluteBoneTransforms);

            // Convert the relative bone transforms into absolute transforms
            ModelBoneCollection bones = model.Bones;
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

        public override void Draw(GameTime time)
        {
            calculateAbsoluteBoneTransforms();

            Matrix wMatrix =
                Matrix.CreateScale(Scale) *
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z) *
                Matrix.CreateTranslation(Position);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * wMatrix;
                    effect.View = GameMapBox.Camera.View;
                    effect.Projection = GameMapBox.Camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            //int meshCount = model.Meshes.Count;

            //// first draw the shadows
            //for (int meshIndex = 0; meshIndex < meshCount; meshIndex++)
            //{
            //    // hide models which ought not be seen
            //    if (!boneVisible[meshIndex])
            //        continue;

            //    ModelMesh mesh = model.Meshes[meshIndex];

            //    int parentBoneIndex = mesh.ParentBone.Index;

            //    int effectCount = mesh.Effects.Count;
            //    for (int effectIndex = 0; effectIndex < effectCount; effectIndex++)
            //    {
            //        Effect effect = mesh.Effects[effectIndex];
            //        if (effect == null)
            //        {
            //            continue; // Model.Draw() would throw an exception in this case
            //        }

            //        // Hand the mesh's transformation matrices to the effect
            //        IEffectMatrices matrices = effect as IEffectMatrices;
            //        if (matrices != null)
            //        {
            //            matrices.World = this.absoluteBoneTransforms[parentBoneIndex] * wMatrix;
            //            matrices.View = GameMapBox.Camera.View;
            //            matrices.Projection = GameMapBox.Camera.Projection;
            //        }
            //    }

            //    MainApplication.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //    mesh.Draw();
            //}
        }
    }
}
