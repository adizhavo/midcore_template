using System;
using UnityEngine;

namespace Services.Core
{
    /// <summary>
    /// Common data structures
    /// Mostly some wrappers on the Unity structure so its possible to use threads
    /// </summary>

    [System.Serializable]
    public class IntVector2
    {
        public int x;
        public int y;

        public IntVector2()
        {
        }

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 Convert(Vector2 vector2)
        {
            return new IntVector2((int)vector2.x, (int)vector2.y);
        }

        public static Vector2 Convert(IntVector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }
    }

    [System.Serializable]
    public class IntVector3
    {
        public int x;
        public int y;
        public int z;

        public IntVector3()
        {
        }

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IntVector3 Convert(Vector3 vector3)
        {
            return new IntVector3((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }

        public static Vector3 Convert(IntVector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }
    }

    [System.Serializable]
    public class FloatVector2
    {
        public float x;
        public float y;

        public FloatVector2()
        {
        }

        public FloatVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static FloatVector2 Convert(Vector2 vector2)
        {
            return new FloatVector2(vector2.x, vector2.y);
        }

        public static Vector2 Convert(FloatVector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }
    }

    [System.Serializable]
    public class FloatVector3
    {
        public float x;
        public float y;
        public float z;


        public FloatVector3()
        {
        }

        public FloatVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static FloatVector3 Convert(Vector3 vector3)
        {
            return new FloatVector3(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 Convert(FloatVector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }
    }
}
