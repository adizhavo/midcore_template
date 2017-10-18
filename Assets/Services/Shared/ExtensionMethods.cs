using System.Collections.Generic;

namespace Services.Core
{
    public static class ExtensionMethods 
    {
        public static bool Contains(this string[] collection, string collect)
        {
            foreach(var item in collection)
            {
                if (string.Equals(item, collect))
                    return true;
            }

            return false;
        }

        public static List<string> Collect(this string[] collection, List<string> filter)
        {
            List<string> collected = new List<string>();

            foreach(var item in collection)
            {
                if (filter.Contains(item))
                    collected.Add(item);
            }

            return collected;
        }
    }
}