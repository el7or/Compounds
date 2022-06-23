using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Puzzle.Compound.Common.Extensions
{
    public static class EnumExtensions
    {
        public static List<EnumValue> GetValues<T>()
        {
            var values = new List<EnumValue>();

            foreach (var itemType in Enum.GetValues(typeof(T)))
            {
                values.Add(new EnumValue
                {
                    NameAr = (itemType as Enum).GetDescription(),
                    Name = Enum.GetName(typeof(T), itemType),
                    Value = (int)itemType
                });
            }

            return values;
        }

        public static string GetDescription(this Enum enumValue)
        {
            if (enumValue == null || enumValue.ToString() == "0")
            {
                return "";
            }

            Type type = enumValue.GetType();
            FieldInfo fi = type.GetField(enumValue.ToString());
            var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attrs != null && attrs.Length > 0)
                return attrs[0].Description;
            else
                return enumValue.ToString();
        }
    }

    public class EnumValue
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int Value { get; set; }
    }
}
