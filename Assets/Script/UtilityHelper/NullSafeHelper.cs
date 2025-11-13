
using UnityEngine;
using UnityEngine.UI;

namespace GUtility
{
    /// <summary>
    /// Null 예외 처리 헬퍼 클래스
    /// </summary>
    public static class NullSafeHelper
    {
        public static void SafeSetActive(this GameObject go, bool isActive)
        {
            if (go != null)
            {
               go.SetActive(isActive); 
            }
        }

        public static void SafeSetActive(this Transform transform, bool isActive)
        {
            if (transform != null)
            {
                transform.gameObject.SafeSetActive(isActive);
            }
        }

        public static void SafeSetActive<T>(this T component, bool isActive) where T : Component
        {
            if (component != null)
            {
                component.gameObject.SetActive(isActive);
            }
        }
        
        public static void SafeSetActive<T>(this T[] components, bool isActive) where T : Component
        {
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].SafeSetActive(isActive);   
                }
            }
        }

        public static void SafeSetPosition(this Transform transform, Vector3 pos)
        {
            if (transform != null)
            {
                transform.position = pos;
            }
        }

        public static void SafeSetEnable<T>(this T behaviour, bool isEnabled) where T : Behaviour
        {
            if (behaviour != null)
            {
                behaviour.enabled = isEnabled;
            }
        }

        public static void SafeSetText(this Text text, string val)
        {
            if (text != null)
            {
                text.text = val;
            }
        }
        
        public static void SafeSetSprite(this Image image, Sprite sprite)
        {
            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public static class ArrayHelper
    {
        public static bool IsValidIndex<T>(this T[][] array, int row, int col)
        {
            return array != null && 
                   row >= 0 && row < array.Length && array[row] != null && 
                   col >= 0 && col < array[row].Length;
        }
    }
}
