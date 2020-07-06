using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HSTween
{
    [Serializable]
    public class Clip : IClip
    {
        public Clip(Track track, float second)
        {
            mParentTrack = track;
            StartTimeToMilliSecond = HSTweenUtility.ConvertSecondToMillisecond(second);
            MoveEndTimeAtCentiSecond(StartTimeToMilliSecond + MinGabToMilliSecond);
        }

        public void SetTrack(Track track)
        {
            mParentTrack = track;
        }

        [SerializeField] [HideInInspector] private Track mParentTrack;
        public Track ParentTrack { get { return mParentTrack; } }

        private Action<GameObject, Vector4> SetAction
        {
            get
            {
                switch (ParentTrack.TrackType)
                {
                    // GameObject
                    case eTrackType.Activate:
                        return HSTweenUtility.SetActivate;

                    // Transform
                    case eTrackType.Position:
                        return HSTweenUtility.SetLocalPosition;
                    case eTrackType.Rotation:
                        return HSTweenUtility.SetLocalRotation;
                    case eTrackType.Scale:
                        return HSTweenUtility.SetLocalScale;

                    // RectTransform
                    case eTrackType.RectSize:
                        return HSTweenUtility.SetRectSizeDelta;

                    // Graphic
                    case eTrackType.Color:
                        return HSTweenUtility.SetColor;
                    case eTrackType.Alpha:
                        return HSTweenUtility.SetAlpha;
                }
                return null;
            }
        }
        private Func<GameObject, Vector4> GetFunc
        {
            get
            {
                switch (ParentTrack.TrackType)
                {
                    // GameObject
                    case eTrackType.Activate:
                        return HSTweenUtility.GetActivateToVector4;

                    // Transform
                    case eTrackType.Position:
                        return HSTweenUtility.GetLocalPositionToVector4;
                    case eTrackType.Rotation:
                        return HSTweenUtility.GetLocalRotationToVector4;
                    case eTrackType.Scale:
                        return HSTweenUtility.GetLocalScaleToVector4;

                    // RectTransform
                    case eTrackType.RectSize:
                        return HSTweenUtility.GetRectSizeDeltaToVector4;

                    // Graphic
                    case eTrackType.Color:
                        return HSTweenUtility.GetColorToVector4;
                    case eTrackType.Alpha:
                        return HSTweenUtility.GetAlphaToVector4;
                }
                return null;
            }
        }

        [SerializeField] private bool mLinkBeforeValue = false;
        public bool IsLinkBeforeValue { get { return mLinkBeforeValue; } set { mLinkBeforeValue = value; } }
        public bool IsLinkNextValue
        {
            get
            {
                Clip nextClip = NextClip;
                if (nextClip != null)
                {
                    return nextClip.IsLinkBeforeValue;
                }
                return false;
            }
            set
            {
                Clip nextClip = NextClip;
                if (nextClip != null)
                {
                    nextClip.IsLinkBeforeValue = value;
                }
            }
        }

        public Clip BeforeClip
        {
            get
            {
                Clip clip = ParentTrack.GetClip(StartTime - 0.001f, ePlayMode.Forwards);
                if (clip != null && clip != this)
                {
                    return clip;
                }
                return null;
            }
        }
        public Clip NextClip
        {
            get
            {
                Clip clip = ParentTrack.GetClip(EndTime + 0.001f, ePlayMode.Backwards);
                if (clip != null && clip != this)
                {
                    return clip;
                }
                return null;
            }
        }

        #region From & To
        [SerializeField] [HideInInspector] private Vector4 from;
        [SerializeField] [HideInInspector] private Vector4 to;

        public Vector4 From { get { return from; } set { SetFrom(value); } }
        public Vector4 To { get { return to; } set { SetTo(value); } }

        private void SetFrom(Vector4 vector)
        {
            from = vector;

            if (IsLinkBeforeValue)
            {
                Clip clip = BeforeClip;
                if (clip != null)
                {
                    clip.to = vector;
                }
            }
        }
        private void SetTo(Vector4 vector)
        {
            to = vector;

            Clip clip = NextClip;
            if (clip != null)
            {
                if (clip.IsLinkBeforeValue)
                    clip.from = vector;
            }
        }
        #endregion From & To

        [SerializeField]
        [HideInInspector]
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        [SerializeField] [HideInInspector] protected uint mStartTime = 0;
        [SerializeField] [HideInInspector] protected uint mGabTime = 0;

        public float DuringTime { get { return HSTweenUtility.ConvertMillisecondToSecond(DuringTimeAtMilliSecond); } }

        public float MinGab { get { return HSTweenUtility.ConvertMillisecondToSecond(MinGabToMilliSecond); } }
        public float MaxGab { get { return HSTweenUtility.ConvertMillisecondToSecond(MaxGabToMilliSecond); } }

        private uint MinGabToMilliSecond { get { return 10; } }
        private uint MaxGabToMilliSecond
        {
            get
            {
                switch (ParentTrack.TrackType)
                {
                    case eTrackType.Activate:
                        return 10;
                    default:
                        return ushort.MaxValue;
                }
            }
        }
        private uint DuringTimeAtMilliSecond
        {
            get { return mGabTime; }
            set { mGabTime = value; }
        }

        public float StartTime { get { return HSTweenUtility.ConvertMillisecondToSecond(StartTimeToMilliSecond); } }
        public float EndTime { get { return HSTweenUtility.ConvertMillisecondToSecond(EndTimeToMilliSecond); } }

        /// <summary>0.001 sec == 1</summary>
        private uint StartTimeToMilliSecond
        {
            get { return mStartTime; }
            set { mStartTime = value; }
        }
        /// <summary>0.001 sec == 1</summary>
        private uint EndTimeToMilliSecond
        {
            get { return (StartTimeToMilliSecond + DuringTimeAtMilliSecond); }
        }

        public void MoveStartTime(float second)
        {
            second = HSTweenUtility.CutCentisecond(second);
            MoveStartTimeAtCentiSecond(HSTweenUtility.ConvertSecondToMillisecond(second));
        }
        private void MoveStartTimeAtCentiSecond(uint milliSec)
        {
            DuringTimeAtMilliSecond = DuringTimeAtMilliSecond + (StartTimeToMilliSecond - milliSec);
            StartTimeToMilliSecond = milliSec;

            if (DuringTimeAtMilliSecond < MinGabToMilliSecond)
            {
                DuringTimeAtMilliSecond = MinGabToMilliSecond;
            }
            if (DuringTimeAtMilliSecond > MaxGabToMilliSecond)
            {
                DuringTimeAtMilliSecond = MaxGabToMilliSecond;
            }
        }
        public void MoveEndTime(float second)
        {
            second = HSTweenUtility.CutCentisecond(second);
            MoveEndTimeAtCentiSecond(HSTweenUtility.ConvertSecondToMillisecond(second));
        }
        private void MoveEndTimeAtCentiSecond(uint milliSec)
        {
            if (milliSec <= StartTimeToMilliSecond)
            {
                StartTimeToMilliSecond = milliSec - MinGabToMilliSecond;
                DuringTimeAtMilliSecond = MinGabToMilliSecond;
            }
            else
            {
                DuringTimeAtMilliSecond = milliSec - StartTimeToMilliSecond;
            }

            if (DuringTimeAtMilliSecond < MinGabToMilliSecond)
            {
                StartTimeToMilliSecond = EndTimeToMilliSecond - MinGabToMilliSecond;
                DuringTimeAtMilliSecond = MinGabToMilliSecond;
            }
            if (DuringTimeAtMilliSecond > MaxGabToMilliSecond)
            {
                StartTimeToMilliSecond = EndTimeToMilliSecond - MaxGabToMilliSecond;
                DuringTimeAtMilliSecond = MaxGabToMilliSecond;
            }
        }

        public void SetFromValue(GameObject target)
        {
            if (GetFunc == null)
                Debug.LogErrorFormat("GetFunc is NULL !! : {0}", ParentTrack.TrackType.ToString());
            else
                SetFrom(GetFunc(target));
        }
        public void SetToValue(GameObject target)
        {
            if (GetFunc == null)
                Debug.LogErrorFormat("GetFunc is NULL !! : {0}", ParentTrack.TrackType.ToString());
            else
                SetTo(GetFunc(target));
        }

        public void SetTime(GameObject target, float second)
        {
            if (SetAction == null)
                Debug.LogErrorFormat("SetAction is NULL !! : {0}", ParentTrack.TrackType.ToString());
            else
            {
                float factor = HSTweenUtility.GetFactor(second, StartTime, EndTime);
                factor = (animationCurve != null) ? animationCurve.Evaluate(factor) : factor;

                Vector4 value = Vector4.zero;
                if (ParentTrack.TrackType == eTrackType.Activate)
                {
                    bool toBool = HSTweenUtility.ConvertToBool(to);

                    value = HSTweenUtility.ConvertToVector4(StartTime <= second ? toBool : !toBool);
                }
                else
                    value = from * (1f - factor) + to * factor;

                SetAction(target, value);
            }
        }
    }
}