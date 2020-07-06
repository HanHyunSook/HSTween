using UnityEngine;
using UnityEditor;

namespace HSTween.HSEditor
{
    [InitializeOnLoad]
    public static class Styles
    {
        public static Texture2D editIcon;
        public static Texture2D dropDownIcon;
        public static Texture2D dropUpIcon;

        public static Texture2D lockIcon;

        public static Texture2D playIcon;
        public static Texture2D playReverseIcon;
        public static Texture2D stepIcon;
        public static Texture2D stepReverseIcon;
        public static Texture2D pauseIcon;
        public static Texture2D stopIcon;

        public static Texture2D carretIcon;
        public static Texture2D plusIcon;
        public static Texture2D trashIcon;

        //private static GUISkin styleSheet;
        public static GUIStyle guiStyle;

        static Styles()
        {
            Load();
        }

        [InitializeOnLoadMethod]
        public static void Load()
        {
            editIcon = (Texture2D) Resources.Load("EditIcon");
            dropDownIcon = (Texture2D) Resources.Load("DropDownIcon");
            dropUpIcon = (Texture2D) Resources.Load("DropUpIcon");

            lockIcon = (Texture2D) Resources.Load("LockIcon");

            playIcon = (Texture2D) Resources.Load("PlayIcon");
            playReverseIcon = (Texture2D) Resources.Load("PlayReverseIcon");
            stepIcon = (Texture2D) Resources.Load("StepIcon");
            stepReverseIcon = (Texture2D) Resources.Load("StepReverseIcon");
            pauseIcon = (Texture2D) Resources.Load("PauseIcon");
            stopIcon = (Texture2D) Resources.Load("StopIcon");

            carretIcon = (Texture2D) Resources.Load("CarretIcon");
            plusIcon = (Texture2D) Resources.Load("PlusIcon");
            trashIcon = (Texture2D) Resources.Load("TrashIcon");


            //styleSheet = (GUISkin) Resources.Load("StyleSheet");

            guiStyle = new GUIStyle();
        }

        ///Get a white 1x1 texture
        public static Texture2D whiteTexture
        {
            get { return EditorGUIUtility.whiteTexture; }
        }
    }
}