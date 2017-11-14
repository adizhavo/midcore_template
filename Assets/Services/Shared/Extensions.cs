using UnityEngine;
using UnityEngine.UI;
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

        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = Vector3.one;
        }

        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }

        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }

        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetAlpha(this RectTransform trans, float value, bool recursive = false) 
        {
            var image = trans.GetComponent<Image>();
            if (image != null)
            {
                Color c = image.color;
                c.a = value;
                image.color = c;
            }

            if (recursive)
            {
                var images = trans.GetComponentsInChildren<Image>(true);
                foreach(var i in images)
                {
                    Color c = i.color;
                    c.a = value;
                    i.color = c;
                }
            }
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetX(this RectTransform trans, float xPos)
        {
            trans.position = new Vector3(xPos, trans.position.y, trans.position.z);
        }

        public static void SetY(this RectTransform trans, float yPos)
        {
            trans.position = new Vector3(trans.position.x, yPos, trans.position.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x*trans.rect.width),
                newPos.y + (trans.pivot.y*trans.rect.height), trans.localPosition.z);
        }

        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x*trans.rect.width),
                newPos.y - ((1f - trans.pivot.y)*trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x)*trans.rect.width),
                newPos.y + (trans.pivot.y*trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x)*trans.rect.width),
                newPos.y - ((1f - trans.pivot.y)*trans.rect.height), trans.localPosition.z);
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x*trans.pivot.x, deltaSize.y*trans.pivot.y);
            trans.offsetMax = trans.offsetMax +
                new Vector2(deltaSize.x*(1f - trans.pivot.x), deltaSize.y*(1f - trans.pivot.y));
        }

        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }

        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }

        public static Services.Core.Rect ToServiceRect(this UnityEngine.Rect rect)
        {
            var r = new Services.Core.Rect();
            r.x = rect.x;
            r.y = rect.y;
            r.width = rect.width;
            r.height = rect.height;
            return r;
        }
    }
}