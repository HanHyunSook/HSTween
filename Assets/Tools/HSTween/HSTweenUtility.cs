using System;
using UnityEngine;
using UnityEngine.UI;

namespace HSTween
{
    public static class HSTweenUtility
    {
        public static float AnimTime(GameObject obj, string animKey)
        {
            if (obj == null)
                return 0f;

            float play = 0f;
            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();

            for (byte i = 0; i < tweeners.Length; ++i)
            {
                if (string.Equals(tweeners[i].AnimationKey, animKey))
                {
                    play = Mathf.Max(play, tweeners[i].AnimationTime);
                }
            }
            return play;
        }
        public static bool Play(GameObject obj, string animKey, ePlayMode playMode)
        {
            if (obj == null)
                return false;

            bool play = false;
            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();

            for (byte i = 0; i < tweeners.Length; ++i)
            {
                if (string.Equals(tweeners[i].AnimationKey, animKey))
                {
                    tweeners[i].Play(playMode);
                    play = true;
                }
            }
            return play;
        }
        public static bool Play(GameObject obj, string animKey, ePlayMode playMode, Action<GameObject> actionFinish)
        {
            if (obj == null)
                return false;

            bool play = false;
            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();

            for (byte i = 0; i < tweeners.Length; ++i)
            {
                if (string.Equals(tweeners[i].AnimationKey, animKey))
                {
                    tweeners[i].Play(playMode, actionFinish);
                    play = true;
                }
            }
            return play;
        }
        public static bool Stop(GameObject obj, string animKey, eStopMode stopMode)
        {
            if (obj == null)
                return false;

            bool stop = false;
            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();

            for (byte i = 0; i < tweeners.Length; ++i)
            {
                if (string.Equals(tweeners[i].AnimationKey, animKey))
                {
                    tweeners[i].Stop(stopMode);
                    stop = true;
                }
            }
            return stop;
        }
        public static bool AllStop(GameObject obj, eStopMode stopMode)
        {
            if (obj == null)
                return false;

            bool stop = false;
            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();

            for (byte i = 0; i < tweeners.Length; ++i)
            {
                tweeners[i].Stop(stopMode);
                stop = true;
            }
            return stop;
        }

        public static bool ToggleAnim(GameObject obj, string animKey, Action<GameObject> actionFinish)
        {
            if (obj == null)
                return false;

            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();
            bool isForward = IsForward(obj, animKey);
            bool play = Play(obj, animKey, isForward ? ePlayMode.Forwards_At_Current : ePlayMode.Backwards_At_Current, actionFinish);
            return play;
        }

        public static bool IsForward(GameObject obj, string animKey)
        {
            if (obj == null)
                return false;

            IHSTweener[] tweeners = obj.GetComponents<IHSTweener>();
            bool isForward = true;

            if (tweeners.Length > 0)
            {
                for (int i = 0; i < tweeners.Length; ++i)
                {
                    if (tweeners[i].AnimationKey.Equals(animKey))
                    {
                        if (tweeners[i].IsPlaying)
                        {
                            switch (tweeners[i].PlayMode)
                            {
                                case ePlayMode.LastFrame:
                                case ePlayMode.Forwards:
                                case ePlayMode.Forwards_At_Current:
                                    isForward = false;
                                    break;
                            }
                        }
                        else if (tweeners[i].IsLastFrame)
                        {
                            isForward = false;
                        }
                        break;
                    }
                }
            }
            return isForward;
        }

        #region Time Convert
        public static float CutCentisecond(float second)
        {
            uint centiSec = (uint)Mathf.RoundToInt(second * 100f);
            return ((float)centiSec / 100f);
        }

        public static float ConvertMillisecondToSecond(uint milliSec)
        {
            return ((float)milliSec / 1000f);
        }

        public static uint ConvertSecondToMillisecond(float second, bool round = true)
        {
            if (round)
                return (uint)Mathf.RoundToInt(second * 1000f);
            else
                return (uint)Mathf.FloorToInt(second * 1000f);
        }
        #endregion Time Convert

        #region Tween
        public static float GetFactor(float current, float startTime, float endTime)
        {
            if (current < startTime)
                return 0f;
            if (current > endTime)
                return 1f;

            return (current - startTime) / (endTime - startTime);
        }
        #endregion Tween

        public static void SetActivate(GameObject target, bool value)
        {
            if (target == null)
                return;

            target.SetActive(value);
        }
        public static void SetActivate(GameObject target, Vector4 value)
        {
            SetActivate(target, ConvertToBool(value));
        }
        public static bool GetActivate(GameObject target)
        {
            if (target == null)
                return false;

            return target.activeSelf;
        }
        public static Vector4 GetActivateToVector4(GameObject target)
        {
            bool value = GetActivate(target);
            return ConvertToVector4(value);
        }

        public static void SetLocalPosition(GameObject target, Vector3 value)
        {
            if (target == null)
                return;

            if (target.GetComponent<RectTransform>())
                target.GetComponent<RectTransform>().anchoredPosition = value;
            else
                target.transform.localPosition = value;
        }
        public static void SetLocalPosition(GameObject target, Vector4 value)
        {
            SetLocalPosition(target, (Vector3)value);
        }
        public static Vector3 GetLocalPosition(GameObject target)
        {
            if (target == null)
                return Vector3.zero;

            if (target.GetComponent<RectTransform>())
                return target.GetComponent<RectTransform>().anchoredPosition;
            else
                return target.transform.localPosition;
        }
        public static Vector4 GetLocalPositionToVector4(GameObject target)
        {
            Vector3 value = GetLocalPosition(target);
            return value;
        }

        public static void SetLocalRotation(GameObject target, Vector3 value)
        {
            if (target == null)
                return;

            if (target.GetComponent<RectTransform>())
                target.GetComponent<RectTransform>().eulerAngles = value;
            else
                target.transform.eulerAngles = value;
        }
        public static void SetLocalRotation(GameObject target, Vector4 value)
        {
            SetLocalRotation(target, (Vector3)value);
        }
        public static Vector3 GetLocalRotation(GameObject target)
        {
            if (target == null)
                return Vector3.zero;

            if (target.GetComponent<RectTransform>())
                return target.GetComponent<RectTransform>().localRotation.eulerAngles;
            else
                return target.transform.eulerAngles;
        }
        public static Vector4 GetLocalRotationToVector4(GameObject target)
        {
            Vector3 value = GetLocalRotation(target);
            return value;
        }

        public static void SetLocalScale(GameObject target, Vector3 value)
        {
            if (target == null)
                return;

            if (target.GetComponent<RectTransform>())
                target.GetComponent<RectTransform>().localScale = value;
            else
                target.transform.localScale = value;
        }
        public static void SetLocalScale(GameObject target, Vector4 value)
        {
            SetLocalScale(target, (Vector3)value);
        }
        public static Vector3 GetLocalScale(GameObject target)
        {
            if (target == null)
                return Vector3.zero;

            if (target.GetComponent<RectTransform>())
                return target.GetComponent<RectTransform>().localScale;
            else
                return target.transform.localScale;
        }
        public static Vector4 GetLocalScaleToVector4(GameObject target)
        {
            Vector3 value = GetLocalScaleToVector4(target);
            return value;
        }

        public static void SetRectSizeDelta(GameObject target, Vector3 value)
        {
            if (target == null)
                return;

            if (target.GetComponent<RectTransform>())
                target.GetComponent<RectTransform>().sizeDelta = value;
        }
        public static void SetRectSizeDelta(GameObject target, Vector4 value)
        {
            SetRectSizeDelta(target, (Vector3)value);
        }
        public static Vector2 GetRectSizeDelta(GameObject target)
        {
            if (target == null)
                return Vector2.zero;

            if (target.GetComponent<RectTransform>())
                return target.GetComponent<RectTransform>().sizeDelta;
            return Vector2.zero;
        }
        public static Vector4 GetRectSizeDeltaToVector4(GameObject target)
        {
            Vector2 value = GetRectSizeDelta(target);
            return value;
        }

        public static void SetColor(GameObject target, Color value)
        {
            if (target == null)
                return;

            if (target.GetComponent<Graphic>())
                target.GetComponent<Graphic>().color = value;
        }
        public static void SetColor(GameObject target, Vector4 value)
        {
            SetColor(target, (Color)value);
        }
        public static Color GetColor(GameObject target)
        {
            if (target == null)
                return Color.white;

            if (target.GetComponent<Graphic>())
                return target.GetComponent<Graphic>().color;
            return Color.white;
        }
        public static Vector4 GetColorToVector4(GameObject target)
        {
            Color value = GetColor(target);
            return value;
        }

        public static void SetAlpha(GameObject target, float value)
        {
            if (target == null)
                return;

            if (target.GetComponent<CanvasGroup>())
                target.GetComponent<CanvasGroup>().alpha = value;
            else if (target.GetComponent<Graphic>())
            {
                Graphic graphic = target.GetComponent<Graphic>();
                Color color = graphic.color;
                color.a = value;
                graphic.color = color;
            }
            else if (target.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
                Color color = renderer.color;
                color.a = value;
                renderer.color = color;
            }
        }
        public static void SetAlpha(GameObject target, Vector4 value)
        {
            SetAlpha(target, ConvertToFloat(value));
        }
        public static float GetAlpha(GameObject target)
        {
            if (target == null)
                return 0f;

            if (target.GetComponent<CanvasGroup>())
                return target.GetComponent<CanvasGroup>().alpha;
            else if (target.GetComponent<Graphic>())
                return target.GetComponent<Graphic>().color.a;
            else if (target.GetComponent<SpriteRenderer>())
                return target.GetComponent<SpriteRenderer>().color.a;
            return 0f;
        }
        public static Vector4 GetAlphaToVector4(GameObject target)
        {
            float value = GetAlpha(target);
            return ConvertToVector4(value);
        }

        public static bool ConvertToBool(Vector4 value)
        {
            return value.x != 0;
        }
        public static Vector4 ConvertToVector4(bool value)
        {
            return new Vector4(value ? 1 : 0, 0);
        }
        public static float ConvertToFloat(Vector4 value)
        {
            return value.x;
        }
        public static Vector4 ConvertToVector4(float value)
        {
            return new Vector4(value, 0);
        }
    }
}