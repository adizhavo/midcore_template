using UnityEngine;

namespace RectSizeFollower.cs.Game.Misc
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailClearer : MonoBehaviour
    {
        private TrailRenderer trail;

        private void Awake()
        {
            trail = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            trail.Clear();
        }
    }
}
