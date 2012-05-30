using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentImporter(".civ5map", DisplayName = "Civ5Map - MiRo Importer", DefaultProcessor = "Civ5MapImporter")]
    public class Civ5MapImporter : ContentImporter<Civ5Map>
    {
        public override Civ5Map Import(string filename, ContentImporterContext context)
        {
            Civ5Map civ5Map = new Civ5Map();
            civ5Map.Load(filename);
            return civ5Map;
        }
    }
}
