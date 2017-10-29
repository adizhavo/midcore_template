using UnityEngine;

namespace Services.Game.Components
{
    public class ViewMonoComponent : MonoBehaviour
    {
        [Header ("Will position HUDs based on this pivot")]
        [Header ("If null will consider the transform as the pivot")]
        public Transform HUDpivot;

        private void Awake()
        {
            if (HUDpivot == null)
            {
                HUDpivot = transform;
            }
        }
    }
}