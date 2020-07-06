using System;
using UnityEditor;
using UnityEngine;

namespace HSTween.HSEditor
{
    [CustomEditor(typeof(HSTweener))]
    public class HSTweenerEditor : Editor
    {
        public HSTweener mTargetScript;

        void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying)
                return;

            if (HSTweenerEditorUtility.TargetWindow != null && HSTweenerEditorUtility.TargetScript == mTargetScript)
                return;

            if (mTargetScript == null)
                return;

            if (EditorApplication.isCompiling)
            {
                Stop(eStopMode.Rewind);
                return;
            }

            if (mTargetScript.IsPlaying)
            {
                HSTweenerEditorUtility.OnUpdateTweener(mTargetScript, 0.50f);
            }
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            mTargetScript = (HSTweener) target;
            mTargetScript.Refresh();

            if (HSTweenerEditorUtility.TargetScript == mTargetScript)
            {
                HSTweenerEditorUtility.TweenEditor = this;
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        public override void OnInspectorGUI()
        {
            GUI.skin.label.richText = true;
            EditorStyles.label.richText = true;
            EditorStyles.foldout.richText = true;

            DrawDefaultInspector();

            GUILayout.Space(10f);

            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            mTargetScript.wrapMode = (eWrapMode) EditorGUI.EnumPopup(rect, "Wrap Mode", mTargetScript.wrapMode);

            rect = EditorGUILayout.GetControlRect(true, 20);
            mTargetScript.mAutoPlay = EditorGUI.Toggle(rect, "Auto Play", mTargetScript.AutoPlay);

            rect = EditorGUILayout.GetControlRect(true, 20);
            mTargetScript.mKey = EditorGUI.TextField(rect, "Key", mTargetScript.AnimationKey);

            rect = EditorGUILayout.GetControlRect(true, 20);
            mTargetScript.AnimationTime = EditorGUI.FloatField(rect, "Animation Time", mTargetScript.AnimationTime);

            if (EditorApplication.isCompiling)
                return;

            bool isEditting = (HSTweenerEditorUtility.TargetWindow != null && HSTweenerEditorUtility.TargetScript == mTargetScript);

            rect = EditorGUILayout.GetControlRect(true, 20);
            {
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey;
                GUI.enabled = !isEditting;

                Styles.guiStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

                if (GUI.Button(new Rect(rect.x, rect.y, 20, 20), Styles.editIcon, (GUIStyle) "box"))
                {
                    HSTweenerWindow.ShowWindow(this);
                    HSTweenerEditorUtility.Select();
                }

                Rect subRect = Rect.MinMaxRect(rect.xMin + 24, rect.yMin, rect.xMax, rect.yMax);
                if (isEditting)
                {
                    GUI.Label(subRect, "Editing. Show HSTweener Window please.");
                }
                else
                {
                    HSTweenerEditorUtility.DrawPlayButtons(subRect, mTargetScript, Play, Stop);
                }
                GUILayout.EndHorizontal();
            }
            GUI.enabled = true;

            DrawerBasic.BackGUIAllColors();
            if (HSTweenerEditorUtility.TargetWindow != null && HSTweenerEditorUtility.TargetScript == mTargetScript)
            {
                Clip clip = HSTweenerEditorUtility.TargetWindow.GetSelectClip();
                if (clip != null)
                {
                    HSClipInspector.ClipInspector(mTargetScript, HSTweenerEditorUtility.SelectGroup, HSTweenerEditorUtility.SelectTrack, HSTweenerEditorUtility.SelectTime);
                }

                if (GUI.changed)
                    HSTweenerEditorUtility.TargetWindow.Repaint();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            DrawerBasic.BackGUIAllColors();
        }

        void Play(ePlayMode playMode)
        {
            mTargetScript.Play(playMode);
        }

        void Stop(eStopMode stopMode)
        {
            mTargetScript.Stop(stopMode);
        }
    }
}