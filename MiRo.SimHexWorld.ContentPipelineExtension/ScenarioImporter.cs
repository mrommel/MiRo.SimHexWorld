using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentImporter(".scenario", DisplayName = "Scenario - MiRo Importer", DefaultProcessor = "ScenarioImporter")]
    public class ScenarioImporter : ContentImporter<string>
    {
        public override String Import(string filename, ContentImporterContext context)
        {
            return System.IO.File.ReadAllText(filename);
        }
    }
}
