using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSTween
{
    [Serializable]
    public class HSTweener : MonoBehaviour, IHSTweener, IDrawerTimeLine
    {
        #region IHSTweener
        [SerializeField] [HideInInspector] public bool mAutoPlay = false;
        [SerializeField] [HideInInspector] public eWrapMode wrapMode = eWrapMode.Once;
        [SerializeField] [HideInInspector] public string mKey = string.Empty;
        [SerializeField] [HideInInspector] private uint mAnimationTime = 500;

        public Action<GameObject> onFinished;

        public bool AutoPlay { get { return mAutoPlay; } }

        /// <summary>Animation Key</summary>
        public string AnimationKey { get { return mKey; } set { mKey = value; } }

        /// <summary>Animation Total Time</summary>
        public float AnimationTime
        {
            get { return HSTweenUtility.ConvertMillisecondToSecond(mAnimationTime); }
            set
            {
                float second = Mathf.Clamp(value, 0, float.MaxValue);
                second = HSTweenUtility.CutCentisecond(second);
                mAnimationTime = HSTweenUtility.ConvertSecondToMillisecond(second);
            }
        }

        /// <summary>Play Animation</summary>
        public void Play(ePlayMode playMode)
        {
            switch (playMode)
            {
                case ePlayMode.Forwards:
                    {
                        SetFirstFrame();
                        PlayMode = ePlayMode.Forwards;
                        IsPlaying = true;
                        enabled = true;
                        break;
                    }
                case ePlayMode.Backwards:
                    {
                        SetLastFrame();
                        PlayMode = ePlayMode.Backwards;
                        IsPlaying = true;
                        enabled = true;
                        break;
                    }
                case ePlayMode.Forwards_At_Current:
                    {
                        PlayMode = ePlayMode.Forwards;
                        IsPlaying = true;
                        enabled = true;
                        break;
                    }
                case ePlayMode.Backwards_At_Current:
                    {
                        PlayMode = ePlayMode.Backwards;
                        IsPlaying = true;
                        enabled = true;
                        break;
                    }
                case ePlayMode.FirstFrame:
                    {
                        PlayMode = ePlayMode.Forwards;
                        SetFirstFrame();
                        break;
                    }
                case ePlayMode.LastFrame:
                    {
                        PlayMode = ePlayMode.Backwards;
                        SetLastFrame();
                        break;
                    }
            }
        }

        public void Play(ePlayMode playMode, Action<GameObject> actionFinish)
        {
            onFinished = actionFinish;
            Play(playMode);
        }

        /// <summary>Stop Animation</summary>
        public void Stop(eStopMode stopMode)
        {
            IsPlaying = false;

            switch (stopMode)
            {
                // Stop and Set Last Frame
                case eStopMode.Skip: {
                        SetLastFrame();
                        break;
                    }
                // Stop and Current Frame
                case eStopMode.Pause:
                    break;
                // Stop and Set First Frame
                case eStopMode.Rewind: {
                        SetFirstFrame();
                        break;
                    }
            }
        }

        /// <summary>Go To First Scene</summary>
        public void SetFirstFrame()
        {
            SetTime(0);
        }

        /// <summary>Go To Last Scene</summary>
        public void SetLastFrame()
        {
            SetTime(AnimationTime);
        }
        #endregion IHSTweener

        [SerializeField] [HideInInspector] public uint mPlayTime = 0;
        /// <summary>Play Current Time</summary>
        public float CurrentTime
        {
            get { return HSTweenUtility.ConvertMillisecondToSecond(mPlayTime); }
            set
            {
                float second = Mathf.Clamp(value, 0, AnimationTime);
                mPlayTime = HSTweenUtility.ConvertSecondToMillisecond(second);
            }
        }

        public ePlayMode PlayMode { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool IsFirstFrame { get { return mPlayTime == 0; } }
        public bool IsLastFrame { get { return mPlayTime >= AnimationTime; } }

        [SerializeField] [HideInInspector] public List<Group> mListGroups = new List<Group>(2);

        public List<Group> Groups { get { return mListGroups; } }
        public int GroupCount { get { return Groups.Count; } }

        private void OnEnable()
        {
            if (AutoPlay)
            {
                Play(ePlayMode.Forwards);
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < GroupCount; ++i)
            {
                Groups[i].Refresh();
            }
        }

        public void SetTime(float time)
        {
            CurrentTime = time;
            for (int i = 0; i < GroupCount; ++i)
            {
                Groups[i].SetTime(CurrentTime, PlayMode);
            }
        }

        public void SetTimeEditor(float time)
        {
            CurrentTime = time;
            for (int i = 0; i < GroupCount; ++i)
            {
                Groups[i].SetTimeEditor(CurrentTime, PlayMode);
            }
        }

        public bool IsGetObj(GameObject obj)
        {
            if (obj == null)
                return false;

            for (ushort i = 0; i < mListGroups.Count; ++i)
            {
                if (mListGroups[i].IsGetObj(obj))
                    return true;
            }
            return false;
        }

        private void OnFinished()
        {
            IsPlaying = false;
            if (onFinished != null)
            {
                onFinished(this.gameObject);
            }
        }

        public void Update()
        {
            if (IsPlaying)
            {
                float deltaTime = Time.unscaledDeltaTime;
                switch (PlayMode)
                {
                    case ePlayMode.Forwards:
                        {
                            deltaTime = CurrentTime + deltaTime;

                            if (deltaTime >= AnimationTime)
                            {
                                switch (wrapMode)
                                {
                                    case eWrapMode.Once:
                                        Stop(eStopMode.Skip);
                                        OnFinished();
                                        break;
                                    case eWrapMode.Loop:
                                        Play(ePlayMode.Forwards);
                                        break;
                                    case eWrapMode.PingPong:
                                        Play(ePlayMode.Backwards);
                                        break;
                                }
                            }
                            else
                            {
                                SetTime(deltaTime);
                            }
                            break;
                        }
                    case ePlayMode.Backwards:
                        {
                            deltaTime = CurrentTime - deltaTime;

                            if (deltaTime <= 0)
                            {
                                switch (wrapMode)
                                {
                                    case eWrapMode.Once:
                                        Stop(eStopMode.Rewind);
                                        OnFinished();
                                        break;
                                    case eWrapMode.Loop:
                                        Play(ePlayMode.Backwards);
                                        break;
                                    case eWrapMode.PingPong:
                                        Play(ePlayMode.Forwards);
                                        break;
                                }
                            }
                            else
                            {
                                SetTime(deltaTime);
                            }
                            break;
                        }
                }
            }
        }

        public Group AddGroup(GameObject obj)
        {
            Group newGroup = new Group();
            newGroup.mTargetObject = obj;
            mListGroups.Add(newGroup);
            return newGroup;
        }
    }
}