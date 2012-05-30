using System;
using System.Collections.Generic;
using MiRoSimHexWorld.Engine.World.Scenarios;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentProcessor(DisplayName = "Scenario - MiRo Processor")]
    public class ScenarioProcessor : ContentProcessor<string, Scenario>
    {
        public override Scenario Process(String input, ContentProcessorContext context)
        {
            string[] lines = input.Split(new char[] { '\n' });

            Scenario scenario = new Scenario();

            scenario.Name = lines[0];
            scenario.MapName = lines[1];

            int startPositions = int.Parse(lines[2]);

            scenario.StartLocations = new List<Scenario.StartLocation>();

            for (int i = 0; i < startPositions; ++i)
            {
                string[] lineParts = lines[i + 3].Split(' ');

                if (lineParts.Length == 3)
                {
                    scenario.StartLocations.Add(new Scenario.StartLocation()
                    {
                        Position = new Microsoft.Xna.Framework.Vector2(int.Parse(lineParts[0]), int.Parse(lineParts[1])),
                        Civilization = lineParts[2]
                    });
                }
            }

            return scenario;
        }
    }
}
