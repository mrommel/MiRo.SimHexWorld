using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentImporter(".py", DisplayName = "Python Script Importer - MiRo Importer")]
    public class PythonScriptImporter : ContentImporter<string>
    {
        public override string Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllText(filename); 
        }
    }
}
