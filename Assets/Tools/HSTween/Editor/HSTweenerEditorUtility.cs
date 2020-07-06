using UnityEngine;
using UnityEditor;
using System;

namespace HSTween.HSEditor
{
    public static class HSTweenerEditorUtility
    {
        public static Color colorGrayHalf = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [System.NonSerialized]
        private static HSTweenerWindow mTargetWindow;
        public static HSTweenerWindow TargetWindow
        {
            get { return mTargetWindow; }
            set { mTargetWindow = value; }
        }

        [System.NonSerialized]
        private static HSTweenerEditor mTweenEditor;
        public static HSTweenerEditor TweenEditor
        {
            get { return mTweenEditor; }
            set
            {
                mTweenEditor = value;
                mTargetScript = mTweenEditor.mTargetScript;
            }
        }

        [System.NonSerialized]
        private static HSTweener mTargetScript;
        public static HSTweener TargetScript { get { return mTargetScript; } }

        public static void Select(Group group = null, Track track = null, float time = -1)
        {
            mSelectGroup = group;
            mSelectTrack = track;
            mSelectTime = time;

            if (TargetScript != null)
            {
                GameObject[] select = new GameObject[] { TargetScript.gameObject };
                Selection.objects = select;
            }

            if (TweenEditor != null)
                TweenEditor.Repaint();
            if (TargetWindow != null)
                TargetWindow.Repaint();
        }

        [System.NonSerialized]
        private static Group mSelectGroup;
        public static Group SelectGroup { get { return mSelectGroup; } }
        [System.NonSerialized]
        private static Track mSelectTrack;
        public static Track SelectTrack { get { return mSelectTrack; } }
        public static Clip SelectClip
        {
            get
            {
                if (SelectTrack != null)
                {
                    return SelectTrack.GetClip(SelectTime);
                }
                return null;
            }
        }
        [System.NonSerialized]
        private static float mSelectTime;
        public static float SelectTime { get { return mSelectTime; } }

        public static void EditorRepaint()
        {
            if (TargetWindow != null)
                TargetWindow.Repaint();
            if (TweenEditor != null)
                TweenEditor.Repaint();
        }

        public static void OnUpdateTweener(HSTweener tweener, float timeScale = 1f)
        {
            int playMode = 0;
            if (tweener.PlayMode == ePlayMode.Forwards)
            {
                playMode = 1;
            }
            else if (tweener.PlayMode == ePlayMode.Backwards)
            {
                playMode = -1;
            }

            float time = Time.fixedDeltaTime * timeScale * playMode;
            time = time + tweener.CurrentTime;

            if (time < 0)
            {
                tweener.Stop(eStopMode.Rewind);
            }
            else if (time > tweener.AnimationTime)
            {
                tweener.Stop(eStopMode.Skip);
            }
            else
            {
                tweener.SetTimeEditor(time);
            }

            EditorRepaint();
        }

        public static void DrawPlayButtons(Rect rect, HSTweener Tween, Action<ePlayMode> playAction, Action<eStopMode> stopAction)
        {
            GUI.backgroundColor = Color.grey;
            GUI.contentColor = Color.white;

            float baseWidth = 20;
            float baseHeight = 20;

            if (rect.width < baseWidth * 5 + 16)
            {
                baseWidth = (rect.width - 16) / 5;
            }
            if (rect.height < baseHeight)
            {
                baseHeight = rect.height;
            }

            Func<byte, Rect> getRect = delegate (byte index)
            {
                return new Rect(rect.x + ((baseWidth + 4) * index), rect.y, baseWidth, baseHeight);
            };

            byte idx = 0;
            if (GUI.Button(getRect(idx), Styles.stepReverseIcon, (GUIStyle) "box"))
            {
                if (stopAction != null)
                    stopAction(eStopMode.Rewind);
            }
            ++idx;
            if (Tween.IsPlaying)
            {
                Rect tRect = getRect(idx);
                tRect.width = tRect.width + baseWidth + 4;
                if (GUI.Button(tRect, Styles.pauseIcon, (GUIStyle) "box"))
                {
                    if (stopAction != null)
                        stopAction(eStopMode.Pause);
                }
                ++idx;
                ++idx;
            }
            else
            {
                if (GUI.Button(getRect(idx), Styles.playReverseIcon, (GUIStyle) "box"))
                {
                    if (playAction != null)
                        playAction(ePlayMode.Backwards);
                }
                ++idx;
                if (GUI.Button(getRect(idx), Styles.playIcon, (GUIStyle) "box"))
                {
                    if (playAction != null)
                        playAction(ePlayMode.Forwards);
                }
                ++idx;
            }
            if (GUI.Button(getRect(idx), Styles.stepIcon, (GUIStyle) "box"))
            {
                if (stopAction != null)
                    stopAction(eStopMode.Skip);
            }

            DrawerBasic.BackGUIAllColors();
        }
    }
}