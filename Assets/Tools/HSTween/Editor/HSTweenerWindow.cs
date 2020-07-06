using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HSTween.HSEditor
{
    public class HSTweenerWindow : EditorWindow
    {
        public static void ShowWindow(HSTweenerEditor sTweener)
        {
            if (HSTweenerEditorUtility.TargetWindow == null)
            {
                HSTweenerEditorUtility.TargetWindow = EditorWindow.GetWindow(typeof(HSTweenerWindow)) as HSTweenerWindow;
                HSTweenerEditorUtility.TargetWindow.minSize = new Vector2(600, 300);
            }
            HSTweenerEditorUtility.TargetWindow.Initialize(sTweener);
            HSTweenerEditorUtility.TargetWindow.Show();
        }

        void Initialize(HSTweenerEditor editor)
        {
            titleContent = new GUIContent("HSTweener");

            HSTweenerEditorUtility.TweenEditor = editor;
            if (HSTweenerEditorUtility.TweenEditor != null)
            {
                if (HSTweenerEditorUtility.TweenEditor.mTargetScript != null)
                {
                    if (!Application.isPlaying)
                        Stop(eStopMode.Rewind);
                }
            }
            TimeLine.Init(HSTweenerEditorUtility.TweenEditor.mTargetScript);
        }

        void OnEditorUpdate()
        {
            if (HSTweenerEditorUtility.TargetScript == null)
                return;

            if (EditorApplication.isCompiling)
            {
                Stop(eStopMode.Rewind);
                return;
            }

            if (HSTweenerEditorUtility.TargetScript.IsPlaying)
            {
                HSTweenerEditorUtility.OnUpdateTweener(HSTweenerEditorUtility.TargetScript, 0.5f);
            }
        }

        private void OnEnable()
        {
            HSTweenerEditorUtility.TargetWindow = this;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnDestroy()
        {
            HSTweenerEditorUtility.TargetWindow = null;
            HSTweenerEditorUtility.Select();
        }

        [NonSerialized] private Vector2 scrollPos           = Vector2.zero;
        [NonSerialized] private float totalHeight           = 0;
        [NonSerialized] private bool isResizingLeftMargin   = false;
        [NonSerialized] private string searchString         = null;

        //Layout variables
        float trackListLeftMargin                   = 300f;
        private float leftMargin
        { //margin on the left side. The width of the group/tracks list.
            get { return trackListLeftMargin; }
            set { trackListLeftMargin = Mathf.Clamp(value, 240, Screen.width / 2); }
        }

        private const float OUT_MARGIN             = 4;
        private const float CONTENT_GAB            = 2;
        private const float TOOLBAR_HEIGHT         = 40; //top margin AFTER the toolbar
        private const float FIRST_GROUP_TOP_MARGIN = 30; //initial top margin

        private Color ContentColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.black : Color.white;
            }
        }
        private Color BackgroundColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.black : Color.gray;
            }
        }

        private Color fGroupTitleColor
        { get { return new Color(0.50f, 0.50f, 0.50f, 0.5f); } }
        private Color fGroupSubTitleColor
        { get { return EditorGUIUtility.isProSkin ? new Color(0.10f, 0.10f, 0.10f, 0.5f) : new Color(0.90f, 0.90f, 0.90f, 0.5f); } }
        private Color fGroupContentColor
        { get { return EditorGUIUtility.isProSkin ? new Color(0.05f, 0.05f, 0.05f, 0.5f) : new Color(0.95f, 0.95f, 0.95f, 0.5f); } }
        private Color fTrackOddColor
        { get { return EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f, 0.5f) : new Color(0.85f, 0.85f, 0.85f, 0.5f); } }
        private Color fTrackEvenColor
        { get { return EditorGUIUtility.isProSkin ? new Color(0.20f, 0.20f, 0.20f, 0.5f) : new Color(0.80f, 0.80f, 0.80f, 0.5f); } }

        private Rect topLeftRect;   //for playback controls
        private Rect topMiddleRect; //for time info
        private Rect leftRect;      //for group/track list
        private Rect centerRect;    //for timeline
        private Rect clipRect;      //for select Clip

        private void InitRect()
        {
            topLeftRect = Rect.MinMaxRect(OUT_MARGIN, OUT_MARGIN, OUT_MARGIN + leftMargin, TOOLBAR_HEIGHT);
            centerRect = Rect.MinMaxRect(topLeftRect.xMax + CONTENT_GAB, topLeftRect.yMax + CONTENT_GAB, screenWidth - OUT_MARGIN, screenHeight - OUT_MARGIN);
            clipRect = Rect.zero;

            leftRect = Rect.MinMaxRect(topLeftRect.xMin, topLeftRect.yMax + CONTENT_GAB, topLeftRect.xMax, screenHeight - OUT_MARGIN);
            topMiddleRect = Rect.MinMaxRect(centerRect.xMin, topLeftRect.yMin, centerRect.xMax, topLeftRect.yMax);
        }

        private DrawerTimeLine mTimeLine;
        private DrawerTimeLine TimeLine
        {
            get
            {
                if (mTimeLine == null)
                {
                    mTimeLine = new DrawerTimeLine();
                    mTimeLine.onSelectTrackAndTime = SelectTrackAndTime;
                    mTimeLine.onGetSelectClip = GetSelectClip;
                }
                return mTimeLine;
            }
        }

        private float screenWidth { get { return Screen.width; } }
        private float screenHeight { get { return Screen.height; } }

        public Clip GetSelectClip()
        {
            if (HSTweenerEditorUtility.SelectTrack != null)
            {
                return HSTweenerEditorUtility.SelectTrack.GetClip(HSTweenerEditorUtility.SelectTime);
            }
            return null;
        }

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.Label("Compiling...");
                return;
            }

            if (HSTweenerEditorUtility.TargetScript == null)
            {
                GUILayout.Label("Select to [HSTweener] and [Edit] Button.");
                return;
            }

            var scrollRect1 = Rect.MinMaxRect(0, centerRect.yMin, screenWidth, screenHeight - 5);
            var scrollRect2 = Rect.MinMaxRect(0, centerRect.yMin, screenWidth, totalHeight + 150);
            scrollPos = GUI.BeginScrollView(scrollRect1, scrollPos, scrollRect2);

            var cEvent = Event.current;

            InitRect();
            MouseEvent(cEvent);

            ////Timelines
            TimeLine.SetRect(topMiddleRect, centerRect);

            ShowGroupsAndTracksList(leftRect, cEvent);
            TimeLine.DrawScroll();

            GUI.EndScrollView();

            HSTweenerEditorUtility.DrawPlayButtons(topLeftRect, HSTweenerEditorUtility.TargetScript, Play, Stop);
            TimeLine.DrawTime();

            DrawerBasic.BackGUIAllColors();
        }

        void SelectTrackAndTime(Group group = null, Track track = null, float time = -1)
        {
            HSTweenerEditorUtility.Select(group, track, time);
        }

        void MouseEvent(Event cEvent)
        {
            //allow resize list width
            var scaleRect = new Rect(leftRect.xMax, leftRect.yMin, CONTENT_GAB, float.MaxValue);
            DrawerBasic.DrawTexture(scaleRect, HSTweenerEditorUtility.colorGrayHalf);
            AddCursorRect(scaleRect, MouseCursor.ResizeHorizontal);

            if (cEvent.type == EventType.MouseDown &&
                cEvent.button == 0 &&
                scaleRect.Contains(cEvent.mousePosition))
            { isResizingLeftMargin = true; cEvent.Use(); }
            if (isResizingLeftMargin)
            { leftMargin = cEvent.mousePosition.x; Repaint(); }
            if (cEvent.rawType == EventType.MouseUp)
            { isResizingLeftMargin = false; }
        }

        void AddCursorRect(Rect rect, MouseCursor type)
        {
            EditorGUIUtility.AddCursorRect(rect, type);
            //willRepaint = true;
        }

        void ShowGroupsAndTracksList(Rect baseRect, Event cEvent)
        {
            //starting height && search.
            bool wasEnabled = GUI.enabled;
            GUI.enabled = true;
            Rect collapseAllRect = new Rect(baseRect.x + 4, baseRect.y + 4, 20, 30);
            Rect searchRect = new Rect(collapseAllRect.xMax, collapseAllRect.y, baseRect.xMax - (collapseAllRect.width + collapseAllRect.xMax), collapseAllRect.height);
            Rect searchCancelRect = Rect.MinMaxRect(searchRect.xMax, searchRect.y, baseRect.xMax - 4, searchRect.yMax);
            bool anyExpanded = true;
            AddCursorRect(collapseAllRect, MouseCursor.Link);
            GUI.color = Color.black;
            if (DrawerBasic.DrawToggleLabel(collapseAllRect, ref anyExpanded, Color.white))
            {
            }
            GUI.color = Color.white;
            searchString = EditorGUI.TextField(searchRect, searchString, (GUIStyle) "ToolbarSeachTextField");
            if (GUI.Button(searchCancelRect, "", (GUIStyle) "ToolbarSeachCancelButton"))
            {
                searchString = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            GUI.enabled = wasEnabled;

            float nextYPos = FIRST_GROUP_TOP_MARGIN;

            ShowRootGroup(baseRect, cEvent, ref nextYPos);
            totalHeight = nextYPos;

            var addButtonY = totalHeight + OUT_MARGIN + TOOLBAR_HEIGHT + 20;
            var addRect = Rect.MinMaxRect(baseRect.xMin + 5, addButtonY, baseRect.xMax - 5, addButtonY + 20);
            GUI.color = Color.white;
            if (GUI.Button(addRect, "Add Actor Group"))
            {
                HSTweenerEditorUtility.TargetScript.AddGroup(null);
            }
            addButtonY += 24;
            addRect = Rect.MinMaxRect(baseRect.xMin + 5, addButtonY, baseRect.xMax - 5, addButtonY + 20);
            if (GUI.Button(addRect, "Add Group (Selection)"))
            {
                AddActorGroupSelect();
            }
        }

        void AddActorGroupSelect()
        {
            GameObject[] objects = Selection.gameObjects;
            if (objects.Length == 0 || (objects.Length == 1 && objects[0] == HSTweenerEditorUtility.TargetScript.gameObject))
            {
                if (HSTweenerEditorUtility.TargetScript.IsGetObj(objects[0]) == false)
                {
                    HSTweenerEditorUtility.TargetScript.AddGroup(null);
                }
            }
            else
            {
                for (ushort i = 0; i < objects.Length; ++i)
                {
                    if (HSTweenerEditorUtility.TargetScript.IsGetObj(objects[i]) == false)
                    {
                        HSTweenerEditorUtility.TargetScript.AddGroup(objects[i]);
                    }
                }
            }
        }

        void AddTrack(Group group, eTrackType trackType)
        {
            group.AddTrack(trackType);
        }

        void ShowRootGroup(Rect baseRect, Event cEvent, ref float nextYPos)
        {
            for (ushort i = 0; i < HSTweenerEditorUtility.TargetScript.GroupCount; ++i)
            {
                ShowGroup(baseRect, cEvent, HSTweenerEditorUtility.TargetScript.Groups[i], i, ref nextYPos);
                nextYPos += 6;
            }
        }

        void ShowGroup(Rect baseRect, Event cEvent, Group group, int index, ref float nextYPos)
        {
            Rect groupRect = GetRect(baseRect, 20, ref nextYPos);
            AddCursorRect(groupRect, HSTweenerEditorUtility.SelectGroup == null ? MouseCursor.Link : MouseCursor.MoveArrow);

            var groupSelected = ReferenceEquals(group, HSTweenerEditorUtility.SelectGroup);
            DrawContentTexture(groupRect, fGroupTitleColor, 0);

            bool back = group.mViewAll;
            DrawerBasic.DrawToggleLabel(new Rect(groupRect.x, groupRect.y, 16, groupRect.height), ref group.mViewAll, Color.white);
            if (group.mLockEditor)
            {
                Rect lockRect = Rect.MinMaxRect(0, 0, 16, 16);
                lockRect.center = new Vector2(groupRect.xMin + 16 + 8, groupRect.yMin + 8);
                DrawerBasic.DrawTexture(lockRect, Color.white, Styles.lockIcon);
            }
            GUI.Label(new Rect(groupRect.x + 32, groupRect.y, groupRect.width - 16, groupRect.height), group.GroupName);

            // Plus Button
            bool plusClicked = false;
            DrawerBasic.GUIColor(EditorGUIUtility.isProSkin ? Color.white : Color.black);
            Rect plusRect = new Rect(groupRect.xMax - 14, groupRect.y + 5, 8, 8);
            if (GUI.Button(plusRect, Styles.plusIcon, GUIStyle.none))
            {
                plusClicked = true;
                group.mViewAll = back;
            }

            DrawerBasic.GUIColor(Color.white);
            // Plus Button or Right Click Content
            if (plusClicked || (cEvent.type == EventType.ContextClick && groupRect.Contains(cEvent.mousePosition)))
            {
                GenericMenu menu = new GenericMenu();

                if (!group.mLockEditor)
                {
                    menu.AddItem(new GUIContent("Editor Lock"), group.mLockEditor, () =>
                    {
                        group.mLockEditor = true;
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Editor UnLock"), group.mLockEditor, () =>
                    {
                        group.mLockEditor = false;
                    });
                }

                if (group.mTargetObject != null)
                {
                    List<eTrackType> trackTypes = group.GetCanAdditiveTypes();
                    for (byte i = 0; i < trackTypes.Count; ++i)
                    {
                        eTrackType type = trackTypes[i];
                        menu.AddItem(new GUIContent(string.Format("Track/{0}", type.ToString())), false, () => { AddTrack(group, type); });
                    }
                }

                menu.AddSeparator("/");
                menu.AddItem(new GUIContent("Delete Group"), false, () => { HSTweenerEditorUtility.TargetScript.Groups.RemoveAt(index); });

                menu.ShowAsContext();
                cEvent.Use();
            }

            if (group.mViewAll)
            {
                // Base Info
                {
                    Rect titleRect = GetRect(baseRect, 20, ref nextYPos);
                    DrawTitle(titleRect, fGroupSubTitleColor, ref group.mViewBasic, "Basic");

                    if (group.mViewBasic)
                    {
                        Rect groupNameRect = GetRect(baseRect, 16, ref nextYPos);
                        Rect objectRect = GetRect(baseRect, 16, ref nextYPos);

                        //DrawerBasic.DrawTexture(Rect.MinMaxRect(titleRect.xMin, titleRect.yMax, objectRect.xMax, objectRect.yMax), fGroupContentColor);

                        groupNameRect = DrawContentTexture(groupNameRect, fGroupContentColor, 2);
                        group.GroupName = EditorGUI.TextField(groupNameRect, "Group Name", group.GroupName);
                        objectRect = DrawContentTexture(objectRect, fGroupContentColor, 2);
                        group.mTargetObject = (GameObject) EditorGUI.ObjectField(objectRect, "Object", group.mTargetObject, typeof(GameObject), true);
                    }
                }

                // Tracks
                if(group.mTargetObject != null)
                {
                    Rect titleRect = GetRect(baseRect, 20, ref nextYPos);
                    titleRect = DrawContentTexture(titleRect, fGroupSubTitleColor, 1);
                    DrawerBasic.DrawToggleLabel(new Rect(titleRect.x, titleRect.y, 16, titleRect.height), ref group.mViewTracks, Color.white);
                    GUI.Label(new Rect(titleRect.x + 16, titleRect.y, titleRect.width - 16, titleRect.height), "Tracks");

                    plusClicked = false;
                    DrawerBasic.GUIColor(EditorGUIUtility.isProSkin ? Color.white : Color.black);
                    plusRect = new Rect(titleRect.xMax - 14, titleRect.y + 5, 8, 8);
                    if (GUI.Button(plusRect, Styles.plusIcon, GUIStyle.none))
                    {
                        plusClicked = true;
                        group.mViewAll = back;
                    }
                    if (plusClicked || (cEvent.type == EventType.ContextClick && titleRect.Contains(cEvent.mousePosition)))
                    {
                        GenericMenu menu = new GenericMenu();

                        List<eTrackType> trackTypes = group.GetCanAdditiveTypes();
                        for (byte i = 0; i < trackTypes.Count; ++i)
                        {
                            eTrackType type = trackTypes[i];
                            menu.AddItem(new GUIContent(string.Format("{0}", type.ToString())), false, () => { AddTrack(group, type); });
                        }

                        menu.ShowAsContext();
                        cEvent.Use();
                    }

                    //DrawTitle(titleRect, fGroupContentColor, ref group.mViewTracks, "Tracks");

                    if (group.mViewTracks)
                    {
                        ShowTracks(baseRect, ref nextYPos, cEvent, group);
                    }
                }

                Rect endRect = GetRect(baseRect, 1, ref nextYPos);
                DrawContentTexture(endRect, fGroupTitleColor, 0);
            }
            DrawerBasic.BackGUIAllColors();
        }

        void ShowTracks(Rect baseRect, ref float nextYPos, Event cEvent, Group group)
        {
            Rect trackRect;
            for (byte i = 0; i < group.TrackCount; ++i)
            {
                Track track = group.Tracks[i];
                trackRect = GetRect(baseRect, 20, ref nextYPos);
                Color targetColor = i % 2 == 0 ? fTrackEvenColor : fTrackOddColor;

                trackRect = DrawContentTexture(trackRect, targetColor, 2);
                GUI.Label(trackRect, track.TrackType.ToString());

                if (cEvent.type == EventType.ContextClick && trackRect.Contains(cEvent.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Remove Track"), false, () => { group.RemoveTrack(track.TrackType); });
                    menu.ShowAsContext();
                }

                TimeLine.DrawTrack(trackRect.yMin, trackRect.yMax, group, track, cEvent);
            }
        }

        Rect DrawContentTexture(Rect titleRect, Color backColor, int depth = 1)
        {
            Rect contentRect;
            if (depth == 0)
            {
                contentRect = titleRect;
                DrawerBasic.DrawTexture(contentRect, backColor);
            }
            else
            {
                Rect lineRect = new Rect(titleRect.x, titleRect.y, 4 * depth, titleRect.height);
                contentRect = Rect.MinMaxRect(lineRect.xMax, titleRect.yMin, titleRect.xMax, titleRect.yMax);

                DrawerBasic.DrawTexture(lineRect, fGroupTitleColor);
                DrawerBasic.DrawTexture(contentRect, backColor);
            }
            TimeLine.DrawTexture(titleRect.yMin, titleRect.yMax, backColor);
            return contentRect;
        }

        Rect DrawTitle(Rect titleRect, Color backColor, ref bool viewer, string titleStr)
        {
            Rect contentRect = DrawContentTexture(titleRect, backColor);
            DrawerBasic.DrawToggleLabel(contentRect, ref viewer, Color.white, titleStr);
            return contentRect;
        }

        Rect GetRect(Rect baseRect, float height, ref float nextYPos)
        {
            Rect returnRect = Rect.MinMaxRect(baseRect.xMin, baseRect.yMin + nextYPos, baseRect.xMax, baseRect.yMin + nextYPos + height);
            nextYPos += (height);
            return returnRect;
        }

        void Play(ePlayMode playMode)
        {
            HSTweenerEditorUtility.TargetScript.Play(playMode);
        }

        void Stop(eStopMode stopMode)
        {
            HSTweenerEditorUtility.TargetScript.Stop(stopMode);
        }
    }
}