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
    }
}
