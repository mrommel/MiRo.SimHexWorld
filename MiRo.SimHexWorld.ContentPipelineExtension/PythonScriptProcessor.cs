using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentProcessor(DisplayName = "PythonScript - MiRo Processor")]
    public class PythonScriptProcessor : ContentProcessor<PythonScript, PythonScript>
    {
        public override PythonScript Process(PythonScript input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
