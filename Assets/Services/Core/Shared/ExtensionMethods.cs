using System.Collections.Generic;

namespace Services.Core
{
    public static class ExtensionMethods 
    {
        public static bool Contains(this string[] collection, string item)
        {
            foreach(var c in collection)
                if (string.Equals(c, item))
                    return true;
            return false;
        }

        public static List<string> Collect(this string[] collection, List<string> filter)
        {
            List<string> collected = new List<string>();

            foreach(var c in collection)
            {
                if (filter.Contains(c))
                    collected.Add(c);
            }

            return collected;
        }
    }
}