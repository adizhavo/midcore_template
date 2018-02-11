using UnityEngine;
using UnityEngine.UI;
using Services.Core;
using Services.Core.GUI;

namespace Services.Game.Misc
{
    [RequireComponent(typeof(RectTransform))]
    public class FloatingUI : MonoBehaviour
    {
        private RectTransform rectTransform;
        private LTDescr yAnim;
        private LTDescr xAnim;
        private LTDescr sizeAnim;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Move(Vector3 fromPos, GUIService guiService, string panelId, string viewID, float duration)
        {
            var panel = guiService.GetPanelView(panelId);
            var rectTransform = panel.GetView(viewID).GetComponent<RectTransform>();
            Move(fromPos, rectTransform, duration);
        }

        public void Move(Vector3 fromPos, RectTransform to, float duration)
        {
            Animate(fromPos, to.position, to.GetSize(), duration);
        }

        protected virtual void Animate(Vector3 fromPos, Vector3 toPos, Vector2 size, float duration)
        {
            ClearAnimations();

            var currSize = rectTransform.GetSize();
            sizeAnim = LeanTween.value(0f, 1f, duration).setOnUpdate(
                (value) =>
            {
                var deltaSize = size - currSize;
                rectTransform.SetSize(currSize + deltaSize * value);
            }
            )
                .setOnComplete(() => rectTransform.SetSize(currSize))
                .setIgnoreTimeScale(true);

            rectTransform.position = fromPos;
            yAnim = LeanTween.value(rectTransform.position.y, toPos.y, duration).setOnUpdate(
                (value) => rectTransform.SetY(value)
            )
                .setEaseInBack()
                .setIgnoreTimeScale(true);

            xAnim = LeanTween.value(rectTransform.position.x, toPos.x, duration).setOnUpdate(
                (value) => rectTransform.SetX(value)
            )
                .setIgnoreTimeScale(true)
                .setOnComplete(() => Stop());
        }

        public void Stop(bool disable = true)
        {
            ClearAnimations();

            if (disable)
                gameObject.SetActive(false);
        }

        private void ClearAnimations()
        {
            if (yAnim != null)
            {
                LeanTween.cancel(yAnim.uniqueId, false);
                yAnim = null;
            }
            if (xAnim != null)
            {
                LeanTween.cancel(xAnim.uniqueId, false);
                xAnim = null;
            }
            if (sizeAnim != null)
            {
                LeanTween.cancel(sizeAnim.uniqueId, true);
                sizeAnim = null;
            }
        }
    }
}