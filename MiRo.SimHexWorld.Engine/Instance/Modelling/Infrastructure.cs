using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public enum Infrastructure : int
    {
        [DescriptionAttribute("No Roads at all.")]
        None = 0,
        CountryLane = 2000,
        Path = 10000,
        Road = 25000,
        Street = 40000,
        Highway = 80000
    }

    public static class InfrastructureExtensions
    {
        public static int GetValue(this Infrastructure c)
        {
            return (int)c;
        }
        public static string GetDescription(this Infrastructure c)
        {
            return EnumUtils.StringValueOf(c);
        }
    }

    public class EnumUtils
    {
        public static string StringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static object EnumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }
    }
}
