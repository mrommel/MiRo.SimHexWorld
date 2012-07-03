using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    public class AnimationAsset
    {
        public string Bone { get; set; }
        public ObjectAnimation Animation { get; set; }
    }

    public class ModelAnimation
    {
        Dictionary<string, ObjectAnimation> _boneAnimation = new Dictionary<string, ObjectAnimation>();

        public ModelAnimation(string assetName)
        {
            List<AnimationAsset> animations = MainApplication.ManagerInstance.Content.Load<List<AnimationAsset>>(assetName);

            foreach (AnimationAsset asset in animations)
                _boneAnimation.Add(asset.Bone, asset.Animation);
        }

        public Matrix GetRotation(string boneName)
        {
            if (_boneAnimation.ContainsKey(boneName))
            {
                Vector3 vec =_boneAnimation[boneName].Rotation;
                return Matrix.CreateRotationX(vec.X) * Matrix.CreateRotationY(vec.Y) * Matrix.CreateRotationZ(vec.Z);
            }

            return Matrix.Identity;
        }

        public void Update(GameTime gameTime)
        {
            foreach (ObjectAnimation ani in _boneAnimation.Values)
                ani.Update(gameTime);
        }
    }
}
