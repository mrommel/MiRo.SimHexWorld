using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance.AI
{
    public class Flavours : List<Flavour>
    {
        public static Flavours FromGrandStrategy(GrandStrategyData _grandStrategy)
        {
            Flavours f = new Flavours();

            foreach (Flavour flavour in _grandStrategy.Flavours)
                f.Add(flavour);

            return f;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Flavour f in this)
                if (f.Amount > 0)
                    sb.AppendFormat("{0}:{1},{2}", f.Name, f.Amount, Environment.NewLine);

            return sb.ToString();
        }

        // Overloading '+' operator:
        public static Flavours operator +(Flavours a, Flavours b)
        {
            Flavours result = new Flavours();

            if (a != null)
                result.AddRange(a);

            if (b != null)
            {
                foreach (Flavour flavour in b)
                {
                    Flavour f = result.FirstOrDefault(c => c.Name == flavour.Name);

                    if (f == null)
                        result.Add(flavour);
                    else
                        f.Amount += f.Amount;
                }
            }

            return result;
        }

        public static Flavours operator +(Flavours a, List<Flavour> b)
        {
            Flavours result = new Flavours();

            if (a != null)
                result.AddRange(a);

            if (b != null)
            {
                foreach (Flavour flavour in b)
                {
                    Flavour f = result.FirstOrDefault(c => c.Name == flavour.Name);

                    if (f == null)
                        result.Add(flavour);
                    else
                        f.Amount += f.Amount;
                }
            }

            return result;
        }

        public static float Distance(List<Flavour> f1, List<Flavour> f2)
        {
            float sum = 0f;

            List<string> flavourNames = new List<string>();

            flavourNames.AddRange(f1.Select(a => a.Name));

            foreach (string name in f2.Select(a => a.Name))
                if (!flavourNames.Contains(name))
                    flavourNames.Add(name);

            foreach (string flavourName in flavourNames)
            {
                Flavour fl1 = f1.FirstOrDefault(a => a.Name == flavourName);
                Flavour fl2 = f2.FirstOrDefault(a => a.Name == flavourName);

                if (fl1 == null && fl2 == null)
                    sum += 0f;
                else if (fl1 == null && fl2 != null)
                    sum += fl2.Amount * fl2.Amount;
                else if (fl1 != null && fl2 == null)
                    sum += fl1.Amount * fl1.Amount;
                else
                    sum += (fl1.Amount - fl2.Amount) * (fl1.Amount - fl2.Amount);
            }

            return (float)Math.Sqrt(sum);
        }

        // Overloading '+' operator:
        public static Flavours operator /(Flavours a, float f)
        {
            Flavours result = new Flavours();

            foreach (Flavour flavour in a)
            {
                Flavour n = new Flavour();
                n.Name = flavour.Name;
                n.Amount /= f;
                result.Add(n);
            }

            return result;
        }
    }
}
