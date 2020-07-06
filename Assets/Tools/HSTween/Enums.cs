using UnityEngine;
using System.Collections;
using System;

namespace HSTween
{
    ///How the cutscene wraps
    [Serializable]
    public enum eWrapMode
    {
        Once,
        Loop,
        PingPong
    }

    ///What happens when cutscene stops
    [Serializable]
    public enum eStopMode
    {
        /// <summary>Stop and Set Last Frame</summary>
        Skip,
        /// <summary>Stop and Current Frame</summary>
        Pause,
        /// <summary>Stop and Set First Frame</summary>
        Rewind,
    }

    ///The direction the cutscene can play. An enum for clarity.
    [Serializable]
    public enum ePlayMode
    {
        /// <summary>Play To Forward</summary>
        Forwards,
        Forwards_At_Current,
        /// <summary>Play To Reverse</summary>
        Backwards,
        Backwards_At_Current,

        FirstFrame,
        LastFrame,
    }

    [Serializable]
    public enum eTrackType
    {
        None,

        // GameObject
        Activate,

        // Transform
        Position,
        Rotation,
        Scale,

        // RectTransform
        RectSize,

        // Graphic
        Color,
        Alpha,

        // Component
        CircleGrid,
    }
}