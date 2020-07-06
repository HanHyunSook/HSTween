using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSTween
{
    [Serializable]
    public class Track : ITrack
    {
        public Track(eTrackType trackType)
        {
            TrackType = trackType;
            mListClips = new List<Clip>(2);
        }

        [SerializeField] [HideInInspector] private eTrackType mTrackType;
        public eTrackType TrackType { get { return mTrackType; } private set { mTrackType = value; } }
        [SerializeField] [HideInInspector] public List<Clip> mListClips;

        #region ITrack
        public Clip this[int index] { get { return mListClips[index]; } }
        public int Count { get { return mListClips.Count; } }

        public void AddClip(float second)
        {
            second = HSTweenUtility.CutCentisecond(second);
            Clip addClip = GetClip(second);
            if (addClip != null)
                second -= addClip.MinGab;

            if (second < 0)
                return;

            addClip = new Clip(this, second);

            if (addClip != null)
            {
                Clip nextClip = GetClip(addClip.EndTime);

                if (nextClip != null && nextClip.StartTime <= addClip.EndTime)
                {
                    addClip.MoveEndTime(nextClip.StartTime);
                }
                mListClips.Add(addClip);
            }
            Refresh();
        }

        public void RemoveClip(float second)
        {
            Clip clip = GetClip(second);

            if (clip != null)
            {
                mListClips.Remove(clip);
            }
            Refresh();
        }
        #endregion

        public void Refresh()
        {
            SortByPlayMode(ePlayMode.Forwards);
            for (byte i = 0; i < Count; ++i)
            {
                this[i].SetTrack(this);
            }
        }

        public void SetTime(GameObject target, float second, ePlayMode playMode)
        {
            Clip currentClip = GetClip(second, playMode);

            if (currentClip != null)
            {
                currentClip.SetTime(target, second);
            }
        }

        public Clip GetClip(float second)
        {
            for (byte i = 0; i < Count; ++i)
            {
                if (second >= this[i].StartTime && second <= this[i].EndTime)
                    return this[i];
            }
            return null;
        }

        private void SortByPlayMode(ePlayMode playMode)
        {
            switch (playMode)
            {
                case ePlayMode.Forwards:
                case ePlayMode.Forwards_At_Current:
                case ePlayMode.FirstFrame:
                    mListClips.Sort((a, b) => { return a.StartTime.CompareTo(b.StartTime); });
                    break;

                case ePlayMode.Backwards:
                case ePlayMode.Backwards_At_Current:
                case ePlayMode.LastFrame:
                    mListClips.Sort((b, a) => { return a.StartTime.CompareTo(b.StartTime); });
                    break;
            }
        }

        public Clip GetClip(float second, ePlayMode playMode)
        {
            if (Count <= 0)
                return null;

            Clip clip = GetClip(second);

            if (clip != null)
                return clip;

            switch (playMode)
            {
                case ePlayMode.Forwards:
                case ePlayMode.Forwards_At_Current:
                case ePlayMode.FirstFrame:
                    SortByPlayMode(ePlayMode.Forwards);
                    break;

                case ePlayMode.Backwards:
                case ePlayMode.Backwards_At_Current:
                case ePlayMode.LastFrame:
                    SortByPlayMode(ePlayMode.Backwards);
                    break;
            }
            if (playMode == ePlayMode.Forwards)
            {
                clip = this[0];

                for (ushort i = 0; i < Count; ++i)
                {
                    if (clip.EndTime < second && second < this[i].StartTime)
                    {
                        break;
                    }
                    if (this[i].EndTime > second)
                    {
                        break;
                    }
                    else
                    {
                        clip = this[i];
                    }
                }
            }
            else
            {
                clip = this[0];

                for (ushort i = 0; i < Count; ++i)
                {
                    if (clip.StartTime > second && second > this[i].EndTime)
                    {
                        break;
                    }
                    if (this[i].StartTime < second)
                    {
                        break;
                    }
                    else
                    {
                        clip = this[i];
                    }
                }
            }

            SortByPlayMode(ePlayMode.Forwards);
            return clip;
        }
    }
}