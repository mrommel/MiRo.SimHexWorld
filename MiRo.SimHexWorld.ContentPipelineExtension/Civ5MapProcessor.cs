using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentProcessor(DisplayName = "Civ5Map - MiRo Processor")]
    public class Civ5MapProcessor : ContentProcessor<Civ5Map, Civ5Map>
    {
        public override Civ5Map Process(Civ5Map input, ContentProcessorContext context)
        {
            return input;
            //var civ5Map = new Civ5Map();
            //civ5Map.Load(input);
            //return civ5Map;
            //return MapData.FromCiv5Map(civ5Map,true);

            //return new MapData();
        }
    }
}
