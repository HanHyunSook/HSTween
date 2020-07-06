using UnityEngine;
using UnityEditor;
using System;

namespace HSTween.HSEditor
{
    public class HSClipInspector
    {
        private static readonly Color fTitleColor = new Color(0.5f,  0.5f,  0.5f,  0.5f);
        private const float kContentGab = 30f;
        private const float kLinkPos = 60f;
        private const float kLinkSize = 20f;

        public static void ClipInspector(HSTweener tweener, Group group, Track track, float time)
        {
            Clip _clip = track.GetClip(time);
            if (_clip == null)
                return;

            Rect titleRect = EditorGUILayout.GetControlRect(true, 20);
            DrawContentTexture(titleRect, fTitleColor, track.TrackType.ToString());

            Action fromAction = delegate ()
            {
                _clip.SetFromValue(group.mTargetObject);
            };
            Action toAction = delegate ()
            {
                _clip.SetToValue(group.mTargetObject);
            };

            switch (track.TrackType)
            {
                // GameObject
                case eTrackType.Activate:
                    DrawToBool(_clip, toAction, "Activate");
                    DrawTimePoint(tweener, track, _clip);
                    break;

                // Transform
                case eTrackType.Position:
                    DrawFromToVector3(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;
                case eTrackType.Rotation:
                    DrawFromToVector3(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;
                case eTrackType.Scale:
                    DrawFromToVector3(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;

                // RectTransform
                case eTrackType.RectSize:
                    DrawFromToVector2(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;

                // Graphic
                case eTrackType.Color:
                    DrawFromToColor(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;
                case eTrackType.Alpha:
                    DrawFromToFloat(_clip, fromAction, toAction);
                    DrawAnimationCurve(_clip);
                    DrawTime(tweener, track, _clip);
                    break;
            }
        }

        #region Draw From To
        private static void SetTitleButton(Rect rect, string title, Action action)
        {
            //Rect rect = EditorGUILayout.GetControlRect(true, 20);
            GUI.Label(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax - kContentGab, rect.yMax), title);
            if (GUI.Button(Rect.MinMaxRect(rect.xMax - kContentGab, rect.yMin, rect.xMax, rect.yMax), Styles.editIcon, "box"))
            {
                action();
            }
        }
        private static void SetTitleFrom(Rect rect, Clip _clip, Action action)
        {
            SetTitleButton(rect, "From", action);

            Rect rect1 = Rect.MinMaxRect(rect.xMin + kLinkPos, rect.yMin, rect.xMin + kLinkPos + kLinkSize, rect.yMax);
            _clip.IsLinkBeforeValue = EditorGUI.Toggle(rect1, _clip.IsLinkBeforeValue);
        }
        private static void SetTitleTo(Rect rect, Clip _clip, Action action)
        {
            SetTitleButton(rect, "To", action);

            Rect rect1 = Rect.MinMaxRect(rect.xMin + kLinkPos, rect.yMin, rect.xMin + kLinkPos + kLinkSize, rect.yMax);
            _clip.IsLinkNextValue = EditorGUI.Toggle(rect1, _clip.IsLinkNextValue);
        }

        private static void DrawFromBool(Clip _clip, Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            bool from = HSTweenUtility.ConvertToBool(_clip.From);
            from = EditorGUI.Toggle(Rect.MinMaxRect(rect.xMin + kContentGab, rect.yMin, rect.xMax, rect.yMax), title, from);
            _clip.From = HSTweenUtility.ConvertToVector4(from);
        }
        private static void DrawToBool(Clip _clip, Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            bool to = HSTweenUtility.ConvertToBool(_clip.To);
            to = EditorGUI.Toggle(Rect.MinMaxRect(rect.xMin + kContentGab, rect.yMin, rect.xMax, rect.yMax), title, to);
            _clip.To = HSTweenUtility.ConvertToVector4(to);
        }
        private static void DrawFromToBool(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromBool(_clip, fromAction, title);
            DrawToBool(_clip, toAction, title);
        }

        private static void DrawFromFloat(Clip _clip , Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            float from = HSTweenUtility.ConvertToFloat(_clip.From);
            from = EditorGUI.FloatField(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, from);
            _clip.From = HSTweenUtility.ConvertToVector4(from);
        }
        private static void DrawToFloat(Clip _clip,  Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            float to = HSTweenUtility.ConvertToFloat(_clip.To);
            to = EditorGUI.FloatField(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, to);
            _clip.To = HSTweenUtility.ConvertToVector4(to);
        }
        private static void DrawFromToFloat(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromFloat(_clip, fromAction, title);
            DrawToFloat(_clip, toAction, title);
        }

        private static void DrawFromVector2(Clip _clip, Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.From = EditorGUI.Vector2Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.From);
        }
        private static void DrawToVector2(Clip _clip, Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.To = EditorGUI.Vector2Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.To);
        }
        private static void DrawFromToVector2(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromVector2(_clip, fromAction, title);
            DrawToVector2(_clip, toAction, title);
        }

        private static void DrawFromVector3(Clip _clip, Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.From = EditorGUI.Vector3Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.From);
        }
        private static void DrawToVector3(Clip _clip, Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.To = EditorGUI.Vector3Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.To);
        }
        private static void DrawFromToVector3(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromVector3(_clip, fromAction, title);
            DrawToVector3(_clip, toAction, title);
        }

        private static void DrawFromVector4(Clip _clip, Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.From = EditorGUI.Vector4Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.From);
        }
        private static void DrawToVector4(Clip _clip, Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.To = EditorGUI.Vector4Field(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.To);
        }
        private static void DrawFromToVector4(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromVector4(_clip, fromAction, title);
            DrawToVector4(_clip, toAction, title);
        }

        private static void DrawFromColor(Clip _clip, Action fromAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleFrom(rect, _clip, fromAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.From = EditorGUI.ColorField(Rect.MinMaxRect(rect.xMin + kContentGab, rect.yMin, rect.xMax, rect.yMax), title, _clip.From);
        }
        private static void DrawToColor(Clip _clip, Action toAction, string title = "")
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            SetTitleTo(rect, _clip, toAction);

            rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.To = EditorGUI.ColorField(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax), title, _clip.To);
        }
        private static void DrawFromToColor(Clip _clip, Action fromAction, Action toAction, string title = "")
        {
            DrawFromColor(_clip, fromAction, title);
            DrawToColor(_clip, toAction, title);
        }
        #endregion Draw From To

        private static void DrawAnimationCurve(Clip _clip)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 20);
            _clip.animationCurve = EditorGUI.CurveField(rect, "Animation Curve", _clip.animationCurve);
        }

        private static void DrawTimePoint(HSTweener tweener, Track track, Clip _clip)
        {
            Clip beforeClip = _clip.BeforeClip;
            Clip nextClip = _clip.NextClip;

            float viewMinTime = 0f;
            float viewMaxTime = tweener.AnimationTime;

            if (beforeClip != null)
            {
                viewMinTime = beforeClip.EndTime;
            }
            if (nextClip != null)
            {
                viewMaxTime = nextClip.StartTime;
            }
            viewMaxTime = viewMaxTime - _clip.MinGab;

            float _timeMin = _clip.StartTime;
            float _timeMax = _clip.EndTime;

            Rect rect = EditorGUILayout.GetControlRect(true, 18);
            GUI.Label(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + (rect.width / 3), rect.yMax), "Start");
            GUI.Label(Rect.MinMaxRect(rect.xMin + rect.width / 3, rect.yMin, rect.xMax - (rect.width / 3), rect.yMax), "During");
            GUI.Label(Rect.MinMaxRect(rect.xMax - (rect.width / 3), rect.yMin, rect.xMax, rect.yMax), "End");

            rect = EditorGUILayout.GetControlRect(true, 18);
            _timeMin = EditorGUI.FloatField(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + (rect.width / 3), rect.yMax), _timeMin);

            float during = _timeMax - _timeMin;
            EditorGUI.LabelField(Rect.MinMaxRect(rect.xMin + rect.width / 3, rect.yMin, rect.xMax - (rect.width / 3), rect.yMax), during.ToString("0.0#"));
            _timeMax = EditorGUI.FloatField(Rect.MinMaxRect(rect.xMax - (rect.width / 3), rect.yMin, rect.xMax, rect.yMax), _timeMax);

            rect = EditorGUILayout.GetControlRect(true, 18);
            _timeMin = EditorGUI.Slider(rect, _timeMin, viewMinTime, viewMaxTime);

            rect = EditorGUILayout.GetControlRect(true, 18);
            GUI.Label(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + 30, rect.yMax), viewMinTime.ToString("0.##"));
            GUI.Label(Rect.MinMaxRect(rect.xMax - 30, rect.yMin, rect.xMax, rect.yMax), viewMaxTime.ToString("0.##"));

            uint milliSec = HSTweenUtility.ConvertSecondToMillisecond(_timeMin);
            float time = HSTweenUtility.ConvertMillisecondToSecond(milliSec);
            if (_clip.StartTime != time)
            {
                _clip.MoveStartTime(time);
            }

            HSTweenerEditorUtility.Select(HSTweenerEditorUtility.SelectGroup, HSTweenerEditorUtility.SelectTrack, (_clip.StartTime + _clip.EndTime) / 2f);
        }

        private static void DrawTime(HSTweener tweener, Track track, Clip _clip)
        {
            Clip beforeClip = _clip.BeforeClip;
            Clip nextClip = _clip.NextClip;

            float viewMinTime = 0f;
            float viewMaxTime = tweener.AnimationTime;

            if (beforeClip != null)
            {
                viewMinTime = beforeClip.EndTime;
            }
            if (nextClip != null)
            {
                viewMaxTime = nextClip.StartTime;
            }

            float _timeMin = _clip.StartTime;
            float _timeMax = _clip.EndTime;

            Rect rect = EditorGUILayout.GetControlRect(true, 18);
            GUI.Label(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + (rect.width / 3), rect.yMax), "Start");
            GUI.Label(Rect.MinMaxRect(rect.xMin + rect.width / 3, rect.yMin, rect.xMax - (rect.width / 3), rect.yMax), "During");
            GUI.Label(Rect.MinMaxRect(rect.xMax - (rect.width / 3), rect.yMin, rect.xMax, rect.yMax), "End");

            rect = EditorGUILayout.GetControlRect(true, 18);
            _timeMin = EditorGUI.DelayedFloatField(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + (rect.width / 3), rect.yMax), _timeMin);
            _timeMax = EditorGUI.DelayedFloatField(Rect.MinMaxRect(rect.xMax - (rect.width / 3), rect.yMin, rect.xMax, rect.yMax), _timeMax);
            //_timeMin = Mathf.Clamp(_timeMin, viewMinTime, viewMaxTime - _clip.MinGab);
            //_timeMax = Mathf.Clamp(_timeMax, viewMinTime + _clip.MinGab, viewMaxTime);

            float during = _timeMax - _timeMin;
            EditorGUI.LabelField(Rect.MinMaxRect(rect.xMin + rect.width / 3, rect.yMin, rect.xMax - (rect.width / 3), rect.yMax), during.ToString("0.0#"));

            rect = EditorGUILayout.GetControlRect(true, 18);
            EditorGUI.MinMaxSlider(rect, ref _timeMin, ref _timeMax, viewMinTime, viewMaxTime);
            _timeMin = Mathf.Clamp(_timeMin, viewMinTime, viewMaxTime - _clip.MinGab);
            _timeMax = Mathf.Clamp(_timeMax, viewMinTime + _clip.MinGab, viewMaxTime);

            rect = EditorGUILayout.GetControlRect(true, 18);
            GUI.Label(Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMin + 30, rect.yMax), viewMinTime.ToString("0.##"));
            GUI.Label(Rect.MinMaxRect(rect.xMax - 30, rect.yMin, rect.xMax, rect.yMax), viewMaxTime.ToString("0.##"));

            uint milliSec = HSTweenUtility.ConvertSecondToMillisecond(_timeMin);
            float time = HSTweenUtility.ConvertMillisecondToSecond(milliSec);
            milliSec = HSTweenUtility.ConvertSecondToMillisecond(_timeMax);
            float timeM = HSTweenUtility.ConvertMillisecondToSecond(milliSec);
            if (_clip.StartTime != time)
            {
                _clip.MoveStartTime(time);
            }

            else if (_clip.EndTime != timeM)
            {
                _clip.MoveEndTime(timeM);
            }

            HSTweenerEditorUtility.Select(HSTweenerEditorUtility.SelectGroup, HSTweenerEditorUtility.SelectTrack, (_clip.StartTime + _clip.EndTime) / 2f);
        }

        static void DrawContentTexture(Rect titleRect, Color backColor, string text)
        {
            DrawerBasic.DrawTexture(titleRect, backColor);

            text = string.Format("<b>{0}</b>", text);
            GUI.Label(titleRect, text);
        }
    }
}