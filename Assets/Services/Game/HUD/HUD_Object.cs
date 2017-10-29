using UnityEngine;

namespace Services.Game.HUD
{
    public class HUD_Object : MonoBehaviour
    {
        public string id
        {
            set;
            get;
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}