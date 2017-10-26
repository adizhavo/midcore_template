using UnityEngine;
using System.Collections.Generic;

namespace Services.Core
{
    public static class ExtensionMethods
    {
        public static bool Contains(this string[] collection, string collect)
        {
            foreach (var item in collection)
            {
                if (string.Equals(item, collect))
                    return true;
            }

            return false;
        }

        public static List<string> Collect(this string[] collection, List<string> filter)
        {
            List<string> collected = new List<string>();

            foreach (var item in collection)
            {
                if (filter.Contains(item))
                    collected.Add(item);
            }

            return collected;
        }

        public static FloatVector2 ToFloatVector2(this Vector2 vector2)
        {
            return new FloatVector2(vector2.x, vector2.y);
        }

        public static Vector2 ToVector2(this FloatVector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }

        public static FloatVector3 ToFloatVector3(this Vector3 vector3)
        {
            return new FloatVector3(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 ToVector3(this FloatVector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }

        public static IntVector2 ToIntVector2(this Vector2 vector2)
        {
            return new IntVector2((int)vector2.x, (int)vector2.y);
        }

        public static Vector2 ToVector2(this IntVector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }

        public static IntVector3 ToIntVector3(this Vector3 vector3)
        {
            return new IntVector3((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }

        public static Vector3 ToVector3(this IntVector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }
    }
}