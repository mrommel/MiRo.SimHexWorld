using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiRoSimHexWorld.Engine.World.Scenarios;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentTypeWriter]
    public class ScenarioWriter : ContentTypeWriter<Scenario>
    {
        protected override void Write(ContentWriter output, Scenario value)
        {
            output.Write(value.Name);
            output.Write(value.MapName);

            output.Write(value.StartLocations.Count);
            foreach (Scenario.StartLocation loc in value.StartLocations)
            {
                output.Write(loc.Position);
                output.Write(loc.Civilization);
            }
        }

        public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return "MiRo.SimHexWorld.Engine.World.Scenarios.ScenarioReader, MiRo.SimHexWorld.Engine";
        }
    }
}
