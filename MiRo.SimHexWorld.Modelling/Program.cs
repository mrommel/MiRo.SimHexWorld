using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Instance.Modelling;
using MiRo.SimHexWorld.Engine.World.Maps;
using System.IO;

namespace MiRo.SimHexWorld.Modelling
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter writer = new StreamWriter("out.csv", false))
            {
                ModellingTile tile = new ModellingTile(new HexPoint());

                tile.InheritanceTaxRate = ModellingTile.TaxRate.LOW;
                tile.SalesTaxRate = ModellingTile.TaxRate.LOW;
                tile.IncomeTaxRate = ModellingTile.TaxRate.LOW;
                tile.CorporateTaxRate = ModellingTile.TaxRate.LOW;
                tile.PropertyTaxRate = ModellingTile.TaxRate.LOW;

                writer.WriteLine("Round; Citizen; Growth; Pollution; Revenue; Low; Mid; High; Poverty");
                for (int i = 0; i < 200; i++)
                {
                    Console.WriteLine(string.Format("{0} citizen: {1}\t growth: {2}\t pollution: {3}", i, tile.Citizen, tile.Growth, tile.Pollution));
                    writer.WriteLine("{0}; {1}; {2:0.000}; {3:0.000}; {4:0.000}; {5:0.000}; {6:0.000}; {7:0.000}; {8:0.000}", i, tile.Citizen, tile.Growth, tile.Pollution, tile.Revenue, tile.LowIncomePeople, tile.MidIncomePeople, tile.HighIncomePeople, tile.Poverty);
                    tile.Update();

                    if (i > 100)
                        tile.IncomeTaxRate = ModellingTile.TaxRate.FAIR;
                }
            }

            Console.ReadKey();
        }
    }
}
