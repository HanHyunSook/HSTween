using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSTween
{
    [Serializable]
    public class Group
    {
        [SerializeField] [HideInInspector] public bool mLockEditor = false;
        [SerializeField] [HideInInspector] public bool mViewAll = true;
        [SerializeField] [HideInInspector] public bool mViewBasic = true;
        [SerializeField] [HideInInspector] public GameObject mTargetObject;
        [SerializeField] private string mGroupName;
        public string GroupName
        {
            get
            {
                if (mTargetObject != null && string.IsNullOrEmpty(mGroupName))
                {
                    return mTargetObject.name;
                }
                else
                {
                    return mGroupName;
                }
            }
            set
            {
                if (mTargetObject != null && string.Equals(mTargetObject.name, value))
                {
                    mGroupName = string.Empty;
                }
                else
                {
                    mGroupName = value;
                }
            }
        }

        #region Tracks
        [SerializeField] [HideInInspector] public bool mViewTracks = true;
        [SerializeField] [HideInInspector] private List<Track> mListTracks = new List<Track>(2);

        public List<Track> Tracks { get { return mListTracks; } }
        public int TrackCount { get { return Tracks.Count; } }

        public List<eTrackType> GetCanAdditiveTypes()
        {
            List<eTrackType> types = new List<eTrackType>();

            Array trackTypes = Enum.GetValues(typeof(eTrackType));
            for (byte typeIdx = 0; typeIdx < trackTypes.Length; ++typeIdx)
            {
                if (((eTrackType) trackTypes.GetValue(typeIdx)) == eTrackType.None)
                    continue;

                if (ContainsType((eTrackType) trackTypes.GetValue(typeIdx)) == false)
                {
                    types.Add((eTrackType) trackTypes.GetValue(typeIdx));
                }
            }

            return types;
        }

        public bool ContainsType(eTrackType trackType)
        {
            Track track = GetTrack(trackType);
            return track != null;
        }

        public void AddTrack(eTrackType trackType)
        {
            if (ContainsType(trackType))
                return;

            mListTracks.Add(new Track(trackType));
            mListTracks.Sort((a, b) => { return a.TrackType.CompareTo(b.TrackType); });
        }

        public void RemoveTrack(eTrackType trackType)
        {
            Track track = GetTrack(trackType);
            if (track == null)
                return;

            mListTracks.Remove(track);
            mListTracks.Sort((a, b) => { return a.TrackType.CompareTo(b.TrackType); });
        }

        public Track GetTrack(eTrackType trackType)
        {
            for (byte i = 0; i < mListTracks.Count; ++i)
            {
                if (mListTracks[i].TrackType == trackType)
                    return mListTracks[i];
            }
            return null;
        }
        #endregion Tracks

        #region Groups
        public void Refresh()
        {
            for (int i = 0; i < TrackCount; ++i)
            {
                Tracks[i].Refresh();
            }
        }

        public void SetTime(float second, ePlayMode playMode)
        {
            if (mTargetObject != null)
            {
                for (int i = 0; i < TrackCount; ++i)
                {
                    Tracks[i].SetTime(mTargetObject, second, playMode);
                }
            }
        }

        public void SetTimeEditor(float second, ePlayMode playMode)
        {
            if (mLockEditor)
                return;

            if (mTargetObject != null)
            {
                for (int i = 0; i < TrackCount; ++i)
                {
                    Tracks[i].SetTime(mTargetObject, second, playMode);
                }
            }
        }

        public bool IsGetObj(GameObject obj)
        {
            if (obj != null && mTargetObject != null)
            {
                if (obj == mTargetObject)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion Groups
    }
}