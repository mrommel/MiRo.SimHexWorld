using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentTypeWriter]
    public class Civ5MapWriter : ContentTypeWriter<Civ5Map>
    {
        protected override void Write(ContentWriter output, Civ5Map value)
        {
            output.Write(value.Name);
            output.Write(value.Description);
            output.Write(value.Width);
            output.Write(value.Height);

            for (int x = 0; x < value.Width; x++ )
            {
                for (int y = 0; y < value.Height; y++)
                {
                    output.Write(value[x, y].TerrainType);
                    output.Write(value[x, y].RessourceType);
                    output.Write(value[x, y].Feature1stType);
                    output.Write(value[x, y].River);

                    output.Write(value[x, y].Elevation);
                    output.Write(value[x, y].Feature2ndType);
                    output.Write((byte)value[x, y].ArtStyle);
                    output.Write(value[x, y].Unused2);
                }
            }

            output.Write(value.TerrainNames.Count);
            foreach(string terrainName in value.TerrainNames)
                output.Write(terrainName);

            output.Write(value.FeatureNames1st.Count);
            foreach (string featureName in value.FeatureNames1st)
                output.Write(featureName);
        }

        public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return "MiRo.SimHexWorld.Engine.World.Maps.Civ5MapReader, MiRo.SimHexWorld.Engine";
        }
    }
}
