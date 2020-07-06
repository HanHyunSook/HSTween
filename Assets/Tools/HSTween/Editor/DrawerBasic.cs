using UnityEngine;
using UnityEditor;

namespace HSTween.HSEditor
{
    public static class DrawerBasic
    {
        private static bool isProSkin
        {
            get { return EditorGUIUtility.isProSkin; }
        }

        private static Texture2D whiteTexture
        {
            get { return Styles.whiteTexture; }
        }

        public static void GUIColor(Color _color)
        {
            GUI.color = _color;
        }

        public static void BackGUIColor()
        {
            GUI.color = Color.white;
        }

        public static void GUIBackgroundColor(Color _color)
        {
            GUI.backgroundColor = _color;
        }

        public static void BackGUIBackgroundColor()
        {
            GUI.backgroundColor = Color.white;
        }

        public static void GUIContentColor(Color _color)
        {
            GUI.contentColor = _color;
        }

        public static void BackGUIContentColor()
        {
            GUI.contentColor = Color.white;
        }

        public static void BackGUIAllColors()
        {
            BackGUIColor();
            BackGUIBackgroundColor();
            BackGUIContentColor();
        }

        public static void DrawLabel(Rect rect, string text, Color color)
        {
            Styles.guiStyle.normal.textColor = color;
            GUI.Label(rect, text, Styles.guiStyle);
        }

        public static bool DrawToggleLabel(Rect rect, ref bool isToggled, Color color, string addString = "")
        {
            GUIColor(color);
            bool isClick = false;

            if (GUI.Button(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + 20, rect.yMax), isToggled ? Styles.dropDownIcon : Styles.dropUpIcon, (GUIStyle) "label"))
            {
                isToggled = !isToggled;
                isClick = true;
            }

            if (string.IsNullOrEmpty(addString) == false)
            {
                if (GUI.Button(Rect.MinMaxRect(rect.xMin + 20, rect.yMin, rect.xMax, rect.yMax), addString, (GUIStyle) "label"))
                {
                    isToggled = !isToggled;
                    isClick = true;
                }
            }
            BackGUIColor();

            return isClick;
        }

        public static void DrawTexture(Rect rect, Color color)
        {
            DrawTexture(rect, color, Styles.whiteTexture);
        }

        public static void DrawTexture(Rect rect, Color color, Texture image)
        {
            GUIColor(color);
            GUI.DrawTexture(rect, image);
            BackGUIColor();
        }
    }
}