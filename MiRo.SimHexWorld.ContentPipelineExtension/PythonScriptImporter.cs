using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentImporter(".py", DisplayName = "Python Script Importer - MiRo Importer", DefaultProcessor = "PythonScriptProcessor")]
    public class PythonScriptImporter : ContentImporter<PythonScript>
    {
        public override PythonScript Import(string filename, ContentImporterContext context)
        {
            string actions = File.ReadAllText(filename);

            PythonScript script = new PythonScript();

            script.Actions = actions;

            return script;
        }
    }
}
