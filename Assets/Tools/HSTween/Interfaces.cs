using System;
using UnityEngine;
using UnityEngine.Events;

namespace HSTween
{
    public interface IHSTweener
    {
        /// <summary>Animation Key</summary>
        string AnimationKey { get; }
        /// <summary>Animation Total Time</summary>
        float AnimationTime { get; }

        ePlayMode PlayMode { get; }
        bool IsPlaying { get; }
        bool IsFirstFrame { get; }
        bool IsLastFrame { get; }

        void Play(ePlayMode playMode);
        void Play(ePlayMode playMode, UnityAction actionFinish);
        void Stop(eStopMode stopMode);

        void SetFirstFrame();
        void SetLastFrame();
    }

    public interface ITrack
    {
        Clip this[int index] { get; }
        int Count { get; }

        void AddClip(float second);
        void RemoveClip(float second);
    }

    public interface IClip
    {
        float DuringTime { get; }
        float StartTime { get; }
        float EndTime { get; }

        void MoveStartTime(float second);
        void MoveEndTime(float second);
    }

    public interface IDrawerTimeLine
    {
        float CurrentTime { get; set; }
        float AnimationTime { get; set; }
        void SetTimeEditor(float time);
    }
}