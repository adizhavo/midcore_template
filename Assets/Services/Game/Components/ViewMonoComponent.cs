using UnityEngine;
using System.Collections.Generic;

namespace Services.Game.Components
{
    public class ViewMonoComponent : MonoBehaviour
    {
        [Header ("Will position HUDs based on this pivot")]
        [Header ("If null will consider the transform as the pivot")]
        public Transform HUDpivot;

        public List<TransformMap> trMap;

        private void Awake()
        {
            if (HUDpivot == null)
            {
                HUDpivot = transform;
            }
        }

        public Transform GetPivot(string id)
        {
            return trMap.Find(t => t.id.Equals(id)).pivot;
        }

        [System.Serializable]
        public class TransformMap
        {
            public string id;
            public Transform pivot;
        }
    }
}