using UnityEngine;
using UnityEditor;
using System;

namespace HSTween.HSEditor
{
    public class DrawerTimeLine
    {
        private const float kHighMode = 1000000f;
        public Action<Group, Track, float> onSelectTrackAndTime;
        public Func<Clip> onGetSelectClip;

        private IDrawerTimeLine mTargetTime;

        private Rect mBarRect;
        private Rect mTimeRect;
        private Rect mScrollRect;

        private float CurrentTime
        {
            get { return mTargetTime.CurrentTime; }
            set { mTargetTime.SetTimeEditor(value); }
        }

        public float AnimationTime
        {
            get { return mTargetTime.AnimationTime; }
            set { mTargetTime.AnimationTime = value; }
        }

        public float maxTime
        {
            get { return Mathf.Max(viewTimeMax, AnimationTime); }
        }

        private float viewTime
        {
            get { return viewTimeMax - viewTimeMin; }
        }

        private float mViewTimeMin = 0;
        public float viewTimeMin
        {
            get { return mViewTimeMin; }
            private set
            {
                float view = Mathf.Min(viewTimeMax - 0.01f, value);
                mViewTimeMin = Mathf.Max(view, 0);
            }
        }

        private float mViewTimeMax = 1;
        public float viewTimeMax
        {
            get { return mViewTimeMax; }
            private set { mViewTimeMax = Mathf.Max(viewTimeMin + 0.01f, value); }
        }

        float lowMod = 0f;
        float highMod = 0f;
        float timeInterval = 0f;
        float[] modulos;
        float start;
        float end;

        float clickTime = 0f;

        public DrawerTimeLine()
        {
        }

        public void Init(IDrawerTimeLine target)
        {
            mTargetTime = target;
        }

        public void SetRect(Rect timeInfoRect, Rect scrollRect)
        {
            mBarRect = new Rect(timeInfoRect.x, timeInfoRect.y,
                timeInfoRect.width, (timeInfoRect.height / 2) - 1);
            mTimeRect = new Rect(timeInfoRect.x, timeInfoRect.y + (timeInfoRect.height / 2) + 1,
                timeInfoRect.width, (timeInfoRect.height / 2) - 1);
            mScrollRect = scrollRect;
            SetBaseData();
        }

        public void DrawTime()
        {
            ShowSlider();
            ShowTimeDot();
            ShowSetMaxTime();
        }

        public void DrawScroll()
        {
            ShowScroll();
        }

        void SetBaseData()
        {
            timeInterval = kHighMode;
            highMod = kHighMode;
            lowMod = 0.01f;
            var modulos = new float[] { 0.1f, 0.5f, 1, 5, 10, 50, 100, 500, 1000, 5000, 10000, 50000, 100000, 250000, 500000 }; //... O.o

            for (var i = 0; i < modulos.Length; i++)
            {
                var count = viewTime / modulos[i];
                if (mScrollRect.width / count > 50)
                { //50 is approx width of label
                    timeInterval = modulos[i];
                    lowMod = i > 0 ? modulos[i - 1] : lowMod;
                    highMod = i < modulos.Length - 1 ? modulos[i + 1] : kHighMode;
                    break;
                }
            }

            start = (float) Mathf.FloorToInt(viewTimeMin / timeInterval) * timeInterval;
            end = (float) Mathf.CeilToInt(viewTimeMax / timeInterval) * timeInterval;
            start = Mathf.Round(start * 10) / 10;
            end = Mathf.Round(end * 10) / 10;
        }

        void ShowSlider()
        {
            // Time Min Max Slider
            float _timeMin = viewTimeMin;
            float _timeMax = viewTimeMax;
            var sliderRect = Rect.MinMaxRect(mBarRect.xMin+5, mBarRect.yMin, mBarRect.xMax-5, mBarRect.yMin+18);
            EditorGUI.MinMaxSlider(sliderRect, ref _timeMin, ref _timeMax, 0, maxTime);
            viewTimeMin = _timeMin;
            viewTimeMax = _timeMax;
            if (sliderRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
            {
                viewTimeMin = 0;
                viewTimeMax = AnimationTime;
            }
        }

        private Color colorDotTime
        { get { return new Color(0.5f, 0.5f, 0.5f, 1.0f); } }
        private Color colorDotLine
        { get { return EditorGUIUtility.isProSkin ? new Color(1.0f, 1.0f, 1.0f, 0.1f) : new Color(0.5f, 0.5f, 0.5f, 1.0f); } }

        private Color colorIntervalLine
        { get { return EditorGUIUtility.isProSkin ? new Color(1.0f, 1.0f, 1.0f, 0.3f) : new Color(0.0f, 0.0f, 0.0f, 0.5f); } }

        void ShowTimeDot()
        {
            //Dot
            if (mTimeRect.width / (viewTime / lowMod) > 6)
            {
                for (var i = start; i <= end; i += lowMod)
                {
                    if (i < viewTimeMin)
                        continue;
                    if (viewTimeMax < i)
                        continue;

                    float posX = TimeToPos(i);

                    // Time Dot
                    Rect frameRect = Rect.MinMaxRect(mTimeRect.xMin + posX - 1, mTimeRect.yMax - 2, mTimeRect.xMin + posX + 1, mTimeRect.yMax - 1);
                    DrawerBasic.DrawTexture(frameRect, colorDotTime);

                    // Time Line
                    Rect lineRect = Rect.MinMaxRect(mScrollRect.xMin + posX, mScrollRect.yMin, mScrollRect.xMin + posX + 1, 100000);
                    DrawerBasic.DrawTexture(lineRect, colorDotLine);
                }
            }

            //the time interval
            for (var i = start; i <= end; i += timeInterval)
            {
                if (i < viewTimeMin)
                    continue;
                if (viewTimeMax < i)
                    continue;

                var posX = TimeToPos(i);

                var rounded = Mathf.Round(i * 10) / 10;

                // Time Dot
                var markRect = Rect.MinMaxRect(mTimeRect.xMin + posX - 2, mTimeRect.yMax - 3, mTimeRect.xMin + posX + 2, mTimeRect.yMax - 1);
                DrawerBasic.DrawTexture(markRect, EditorGUIUtility.isProSkin ? Color.white : Color.black);

                // Time Label
                var text = rounded.ToString("0.00");
                var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(text));
                var stampRect = new Rect(0, 0, size.x, size.y);
                stampRect.center = new Vector2(mTimeRect.xMin + posX, mTimeRect.yMin + mTimeRect.height - size.y + 4);
                DrawerBasic.GUIColor(rounded % highMod == 0 ? Color.white : new Color(1, 1, 1, 0.5f));
                GUI.Box(stampRect, text, (GUIStyle) "label");
                DrawerBasic.BackGUIColor();

                var guideRect = new Rect(posX + mScrollRect.x, mScrollRect.y, 1, 100000);
                DrawerBasic.DrawTexture(guideRect, colorIntervalLine);
            }

            //the number showing current time when scubing
            if (CurrentTime > viewTimeMin && CurrentTime <= viewTimeMax)
            {
                var posX = TimeToPos(CurrentTime);
                var label = CurrentTime.ToString("0.00");
                var text = "<b><size=17>" + label + "</size></b>";
                var size = new Vector2(50, 20);
                var stampRect = new Rect(0, 0, size.x, size.y);
                stampRect.center = new Vector2(mTimeRect.xMin + posX, mTimeRect.yMin + mTimeRect.height - size.y / 2);

                DrawerBasic.GUIBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
                DrawerBasic.DrawLabel(stampRect, text, Color.yellow);
                Rect lineRect = new Rect(mScrollRect.x + posX, mScrollRect.y, 1, 100000);
                DrawerBasic.DrawTexture(lineRect, Color.yellow);
                DrawerBasic.BackGUIAllColors();
            }

            //the length position carret texture and pre-exit length indication
            float lengthPos = TimeToPos(AnimationTime);
            if (lengthPos >= 0 && AnimationTime < viewTimeMax)
            {
                Rect lengthRect = new Rect(0, 0, 16, 16);
                lengthRect.center = new Vector2(mTimeRect.xMin + lengthPos, mTimeRect.yMin + mTimeRect.height - 2);
                DrawerBasic.DrawTexture(lengthRect, EditorGUIUtility.isProSkin ? Color.white : Color.black, Styles.carretIcon);
            }
        }

        void ShowSetMaxTime()
        {
            Rect rect = Rect.MinMaxRect(mTimeRect.xMin - 4, mTimeRect.yMax - 5, mTimeRect.xMax + 60, mTimeRect.yMax + 10);
            float curTime = CurrentTime;
            curTime = EditorGUI.Slider(rect, curTime, viewTimeMin, viewTimeMax);
            if (curTime != CurrentTime)
                CurrentTime = curTime;
        }

        void ShowScroll()
        {
            DrawerBasic.DrawTexture(new Rect(mScrollRect.x, mScrollRect.y, mScrollRect.width, 100000), new Color(0.1f,0.1f,0.1f,0.1f));
        }

        float TimeToPos(float time)
        {
            return (time - viewTimeMin) / viewTime * mScrollRect.width;
        }

        float PosToTime(float pos)
        {
            return (pos - mTimeRect.xMin) / mTimeRect.width * viewTime + viewTimeMin;
        }

        public void DrawTexture(float yMin, float yMax, Color color)
        {
            DrawerBasic.DrawTexture(Rect.MinMaxRect(mScrollRect.xMin, yMin, mScrollRect.xMax, yMax), color);
        }

        private readonly Color kSelectClipColor   = new Color(0.0f, 0.1f, 0.0f, 1.0f);
        private readonly Color kUnSelectClipColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        public void DrawTrack(float yMin, float yMax, Group group, Track track, Event cEvent)
        {
            Rect trackRect = Rect.MinMaxRect(mScrollRect.xMin, yMin, mScrollRect.xMax, yMax);
            bool clipMouse = false;

            for (byte i = 0; i < track.Count; ++i)
            {
                clipMouse = (clipMouse || DrawClip(yMin, yMax, group, track, track[i], cEvent));
            }

            if (clipMouse == false && trackRect.Contains(cEvent.mousePosition))
            {
                if (cEvent.type == EventType.MouseDown)
                {
                    SelectTrackAndTime(group, track, -1);
                }
                else if (cEvent.type == EventType.ContextClick)
                {
                    clickTime = PosToTime(cEvent.mousePosition.x);
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Add Clip"), false, () =>
                    {
                        track.AddClip(clickTime);
                    });

                    menu.AddSeparator("/");

                    menu.AddItem(new GUIContent("Remove Track"), false, () =>
                    {
                        group.RemoveTrack(track.TrackType);
                    });

                    menu.ShowAsContext();
                }
            }
        }

        public bool DrawClip(float yMin, float yMax, Group group, Track track, Clip clip, Event cEvent)
        {
            bool clipMouse = false;

            float startTime = clip.StartTime;
            float endTime = clip.EndTime;

            if (startTime > viewTimeMax)
                return false;
            if (endTime < viewTimeMin)
                return false;

            if (startTime < viewTimeMin)
                startTime = viewTimeMin;

            if (viewTimeMax < endTime)
                endTime = viewTimeMax;

            Clip targetClip = GetSelectClip();
            float startPos = TimeToPos(clip.StartTime);
            float endPos = TimeToPos(clip.EndTime);

            Rect clipRect = Rect.MinMaxRect(mScrollRect.xMin + startPos, yMin, mScrollRect.xMin + endPos, yMax);
            if (clipRect.xMax > mScrollRect.xMax)
            {
                clipRect.xMax = mScrollRect.xMax;
            }
            if (clipRect.xMin < mScrollRect.xMin)
            {
                clipRect.xMin = mScrollRect.xMin;
            }

            DrawerBasic.DrawTexture(clipRect, (targetClip == clip) ? kSelectClipColor : kUnSelectClipColor);
            if (clipRect.width > 2)
            {
                DrawerBasic.DrawTexture(Rect.MinMaxRect(clipRect.xMin + 1, clipRect.yMin + 2, clipRect.xMax - 1, clipRect.yMax - 2), new Color(1.0f,1.0f,1.0f,0.5f));
            }

            if (cEvent.type == EventType.MouseDown)
            {
                if (clipRect.Contains(cEvent.mousePosition))
                {
                    clipMouse = true;
                    SelectTrackAndTime(group, track, PosToTime(cEvent.mousePosition.x));
                }
            }
            else if (cEvent.type == EventType.ContextClick && clipRect.Contains(cEvent.mousePosition))
            {
                clickTime = PosToTime(cEvent.mousePosition.x);
                clipMouse = true;
                GenericMenu menu = new GenericMenu();

                Clip beforeClip = clip.BeforeClip;
                Clip nextClip = clip.NextClip;

                if (beforeClip != null)
                {
                    menu.AddItem(new GUIContent("Link Before Value"), clip.IsLinkBeforeValue, () =>
                    {
                        clip.IsLinkBeforeValue = !clip.IsLinkBeforeValue;
                    });
                }
                if (nextClip != null)
                {
                    menu.AddItem(new GUIContent("Link Next Value"), clip.IsLinkNextValue, () =>
                    {
                        clip.IsLinkNextValue = !clip.IsLinkNextValue;
                    });
                }

                menu.AddItem(new GUIContent("Remove Clip"), false, () =>
                {
                    track.RemoveClip(clickTime);
                });
                menu.ShowAsContext();
            }

            return clipMouse;
        }

        private void SelectTrackAndTime(Group group, Track track, float time)
        {
            if (onSelectTrackAndTime != null)
            {
                onSelectTrackAndTime(group, track, time);
            }
        }

        private Clip GetSelectClip()
        {
            if (onGetSelectClip != null)
            {
                return onGetSelectClip();
            }
            return null;
        }
    }
}