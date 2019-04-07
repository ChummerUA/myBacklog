using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Drawing;

namespace myBacklog.Models
{
    public class NamedColor
    {
        private NamedColor()
        {
        }

        public string Name { private set; get; }

        public Color Color { private set; get; }

        // Static members.
        static NamedColor()
        {
            List<NamedColor> all = new List<NamedColor>();
            StringBuilder stringBuilder = new StringBuilder();

            // Loop through the public static fields of the Color structure.
            foreach (PropertyInfo propInfo in typeof(Color).GetRuntimeProperties())
            {
                if (propInfo.PropertyType == typeof(Color))
                {
                    // Convert the name to a friendly name.
                    string name = propInfo.Name;
                    stringBuilder.Clear();
                    int index = 0;

                    foreach (char ch in name)
                    {
                        if (index != 0 && Char.IsUpper(ch))
                        {
                            stringBuilder.Append(' ');
                        }
                        stringBuilder.Append(ch);
                        index++;
                    }

                    // Instantiate a NamedColor object.
                    Color color = (Color)propInfo.GetValue(null);

                    NamedColor namedColor = new NamedColor
                    {
                        Name = stringBuilder.ToString(),
                        Color = color,
                    };

                    // Add it to the collection.
                    all.Add(namedColor);
                }
            }

            var transparent = all.FirstOrDefault(x => x.Name == "Transparent");

            all.Remove(transparent);
            all.TrimExcess();
            All = all;
        }

        public static List<NamedColor> All { private set; get; }
    }
}
