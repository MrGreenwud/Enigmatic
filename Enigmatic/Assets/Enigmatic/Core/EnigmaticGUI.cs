using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using Enigmatic.Experemental.ENIX;

namespace Enigmatic.Core
{
    public class GUIElement
    {
        public Rect Rect;

        public GUIElement() { }

        public GUIElement(Rect rect)
        {
            Rect = rect;
        }
    }

    public enum GrupSortType
    {
        None,
        Veritical,
        Horizontal,
    }

    public class GUIGrup : GUIElement
    {
        public GrupSortType SortType = GrupSortType.None;
        public float ElementSpacing = 3;
        public float Pudding = 3;

        public bool IsExpandWidth = false;
        public bool IsExpandHeight = false;

        public bool IsClicable = false;

        public GUIStyle Style = GUIStyle.none;

        private GUIGrup m_ParentGrup;
        private List<GUIElement> m_GUIElements = new List<GUIElement>();

        public GUIGrup ParentGrup => m_ParentGrup;
        
        public GUIGrup(Rect rect, float pudding = 3, GrupSortType sortType = GrupSortType.None,
            bool isExpandedWidth = false, bool isExpandedHeight = false,
            float elementSpacing = 3) : base(rect)
        {
            SortType = sortType;
            ElementSpacing = elementSpacing;
            Pudding = pudding;
            IsExpandWidth = isExpandedWidth;
            IsExpandHeight = isExpandedHeight;
        } //Delete

        public GUIGrup(Rect rect, GrupSortType sortType) : base(rect)
        {
            SortType = sortType;
        }

        public void ApplyOptions(params EnigmaticGUILayoutOption[] options)
        {
            foreach (EnigmaticGUILayoutOption option in options)
            {
                switch (option.Type)
                {
                    case EnigmaticGUILayoutOption.TypeOption.SetPudding:
                        Pudding = (float)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetElementSpacing:
                        ElementSpacing = (float)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetExpandWidth:
                        IsExpandWidth = (bool)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetExpandHeight:
                        IsExpandHeight = (bool)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetWidth:
                        Rect.width = (float)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetHeight:
                        Rect.height = (float)option.Value;
                        break;
                    case EnigmaticGUILayoutOption.TypeOption.SetClickable:
                        IsClicable = (bool)option.Value;
                        break;
                }
            }
        }

        public void SetParentGrup(GUIGrup parent)
        {
            if (m_ParentGrup != null)
                return;

            m_ParentGrup = parent;
        }

        public void AddElement(GUIElement element)
        {
            if (m_GUIElements.Contains(element))
                throw new Exception();

            m_GUIElements.Add(element);

            if (element is GUIGrup grup)
                grup.SetParentGrup(this);

            ColculateSize(element);
        }

        public virtual Rect GetNext()
        {
            if (m_GUIElements.Count == 0)
            {
                return new Rect(Rect.position + Vector2.one * Pudding,
                        Rect.size - Vector2.one * (Pudding * 2));
            }

            return Sort(m_GUIElements.Last());
        }

        public Rect Sort(GUIElement element)
        {
            if (m_GUIElements.Contains(element) == false)
                throw new Exception();

            Rect rect = new Rect();

            if (SortType == GrupSortType.Horizontal)
            {
                rect.x = element.Rect.x + element.Rect.width + ElementSpacing;
                rect.y = Rect.y + Pudding;
            }
            else if (SortType == GrupSortType.Veritical)
            {
                rect.x = Rect.x + Pudding;
                rect.y = element.Rect.y + element.Rect.height + ElementSpacing;
            }

            return rect;
        }

        public Vector2 GetFreeArea()
        {
            float x = Rect.width - Pudding * 2;
            float y = Rect.height - Pudding * 2;

            if (m_GUIElements.Count > 0)
            {
                if (SortType == GrupSortType.Horizontal)
                {
                    x = Rect.width - Pudding * 2 - (m_GUIElements.Last().Rect.position
                        - Rect.position + Vector2.right * m_GUIElements.Last().Rect.width).x - ElementSpacing;
                }
                else if (SortType == GrupSortType.Veritical)
                {
                    y = Rect.height - Pudding * 2 - (m_GUIElements.Last().Rect.position
                        - Rect.position + Vector2.up * m_GUIElements.Last().Rect.width).y - ElementSpacing;
                }
            }

            return new Vector2(x, y);
        }

        public void ReconculateSize()
        {
            foreach(GUIElement element in m_GUIElements)
                ColculateSize(element);
        }

        public void ColculateSize(GUIElement element)
        {
            if (IsExpandHeight)
            {
                float extremePointElement = element.Rect.position.y + element.Rect.height;
                float extremePointThis = Rect.position.y + Rect.height;

                if (extremePointElement > extremePointThis)
                {
                    float expandValue = extremePointElement - extremePointThis + (Pudding * 2);
                    Rect.height += expandValue;
                }
            }

            if (IsExpandWidth)
            {
                float extremePointElement = element.Rect.position.x + element.Rect.width;
                float extremePointThis = Rect.position.x + Rect.width;

                if(extremePointElement > extremePointThis)
                {
                    float expandValue = extremePointElement - extremePointThis + (Pudding * 2);
                    Rect.width += expandValue;
                }
            }

            if(ParentGrup != null)
                ParentGrup.ReconculateSize();
        }
    }

    public class ScrollView : GUIGrup
    {
        public Rect ViewRect {  get; private set; }
        public Vector2 ScrollPosition { get; private set; }

        public bool AlwaysShowHorizontal { get; private set; }
        public bool AlwaysShowVertical { get; private set; }

        public GUIStyle HorizontalScrollbar { get; private set; } 
        public GUIStyle VerticalScrollbar { get; private set; } 
        public GUIStyle Background { get; private set; }

        public ScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, bool alwaysShowHorizontal, 
            bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, 
            GUIStyle background, GrupSortType sortType) 
            : base(rect, sortType) 
        {
            ViewRect = viewRect;
            ScrollPosition = scrollPosition;

            AlwaysShowHorizontal = alwaysShowHorizontal;
            AlwaysShowVertical = alwaysShowVertical;

            HorizontalScrollbar = horizontalScrollbar;
            VerticalScrollbar = verticalScrollbar;
            Background = background;
        }

        public void UpdatePosition()
        {
            Rect.position += ScrollPosition;
        }
    }

    public class PostActionCaller
    {
        protected object[] Parameters { get; private set; }
        private Action<object[]> m_Action;

        public void Init(object[] parameters, Action<object[]> action)
        {
            Parameters = parameters;
            m_Action = action;
        }

        public virtual void Call() 
        {
            m_Action?.Invoke(Parameters);
        }
    }

    public class Lable : PostActionCaller
    {
        public override void Call()
        {
            if(Parameters.Length < 3)
                return;

            string name = Parameters[0].ToString();
            Vector2 position = (Vector2)Parameters[1];
            GUIStyle style = Parameters[2] as GUIStyle;

            EnigmaticGUI.Label(name, position, style);
        }
    }

    public class Image : PostActionCaller
    {
        public override void Call()
        {
            if (Parameters.Length < 2)
                return;

            Rect rect = (Rect)Parameters[0];
            GUIStyle style = Parameters[1] as GUIStyle;

            EnigmaticGUI.Image(rect, style);
        }
    }

    public class Button : PostActionCaller
    {
        public override void Call()
        {
            if (Parameters.Length < 3)
                return;

            string text = (string)Parameters[0];
            Rect rect = (Rect)Parameters[1];
            GUIStyle style = Parameters[2] as GUIStyle;

            EnigmaticGUI.Button(text, rect.position, rect.size, style);
        }
    }

    public class Property : PostActionCaller
    {
        public override void Call()
        {
            if (Parameters.Length < 3)
                return;

            SerializedProperty property = (SerializedProperty)Parameters[0];
            Vector2 position = (Vector2)Parameters[1];
            float widthFieldArea = (float)Parameters[2];

            EnigmaticGUI.PropertyField(property, position, widthFieldArea);
        }
    }

    public class Grup : PostActionCaller
    {
        public override void Call()
        {
            if (Parameters.Length < 3)
                return;

            GUIGrup grup = Parameters[0] as GUIGrup;
            GUIStyle style = Parameters[1] as GUIStyle;
            float border = (float)Parameters[2];

            if (style != null)
            {
                grup.ReconculateSize();

                EnigmaticGUI.Image(EnigmaticGUI.GetFixedBox(grup.Rect, (int)border), style);
            }
        }
    }

    public static class GUIElementDrawer
    {
        /// <summary>
        /// 0 - text, 1 - position, 2 - style
        /// </summary>
        public static void Lable(object[] parameters)
        {
            if (parameters.Length < 3)
                return;

            string name = parameters[0].ToString();
            Vector2 position = (Vector2)parameters[1];
            GUIStyle style = parameters[2] as GUIStyle;

            EnigmaticGUI.Label(name, position, style);
        }

        /// <summary>
        /// 0 - rect, 1 - style
        /// </summary>
        public static void Image(object[] parameters)
        {
            if (parameters.Length < 2)
                return;

            Rect rect = (Rect)parameters[0];
            GUIStyle style = parameters[1] as GUIStyle;

            EnigmaticGUI.Image(rect, style);
        }

        /// <summary>
        /// 0 - rect, 1 - text, 2 - style
        /// </summary>
        public static void Button(object[] parameters)
        {
            if (parameters.Length < 3)
                return;

            Rect rect = (Rect)parameters[0];
            string text = (string)parameters[1];
            GUIStyle style = parameters[2] as GUIStyle;

            EnigmaticGUI.Button(text, rect.position, rect.size, style);
        }

        /// <summary>
        /// 0 - rect, 1 - value, 2 - value guid, 3! - name 
        /// </summary>
        public static void FloatField(object[] parameters)
        {
            if(parameters.Length < 4)
                return;

            Rect rect = (Rect)parameters[0];
            int guid = (int)parameters[1];
            float value = (int)parameters[2];
            string name = (string)parameters[3];

            float newValue = value;

            if (string.IsNullOrEmpty(name))
                newValue = EnigmaticGUI.FloatField(name, value, rect.position, rect.width);
            else
                newValue = EnigmaticGUI.FloatField(value, rect.position, rect.width);

            GUIValueCasher.CashValue(guid, newValue);
        }

        /// <summary>
        /// 0 - rect, 1 - value, 2 - value guid, 3! - name 
        /// </summary>
        public static void IntField(object[] parameters)
        {
            if (parameters.Length < 4)
                return;

            Rect rect = (Rect)parameters[0];
            int guid = (int)parameters[1];
            int value = (int)parameters[2];
            string name = (string)parameters[3];

            int newValue = value;

            if (string.IsNullOrEmpty(name))
                newValue = EnigmaticGUI.IntField(name, value, rect.position, rect.width);
            else
                newValue = EnigmaticGUI.IntField(value, rect.position, rect.width);

            GUIValueCasher.CashValue(guid, newValue);
        }

        /// <summary>
        /// 0 - rect, 1 - value, 2 - value guid, 3! - name 
        /// </summary>
        public static void TextField(object[] parameters)
        {
            if (parameters.Length < 4)
                return;

            Rect rect = (Rect)parameters[0];
            int guid = (int)parameters[1];

            string value = (string)parameters[2];

            //if (GUIValueCasher.TryGetValue(guid, out object v))
            //{
            //    value = (string)v;
            //}

            string name = (string)parameters[3];

            string newValue = value;

            if (string.IsNullOrEmpty(name))
                newValue = EnigmaticGUI.TextField(newValue, rect.position, rect.width);
            else
                newValue = EnigmaticGUI.TextField(name, newValue, rect.position, rect.width);

            if(value != newValue)
                GUIValueCasher.CashValue(guid, newValue);
        }

        /// <summary>
        /// 0 - GUIGrup, 1 - border
        /// </summary>
        public static void Grup(object[] parameters)
        {
            if (parameters.Length < 2)
                return;

            GUIGrup grup = parameters[0] as GUIGrup;
            GUIStyle style = grup.Style;
            int border = (int)parameters[1];

            if (style != null)
            {
                grup.ReconculateSize();

                EnigmaticGUI.Image(EnigmaticGUI.GetFixedBox(grup.Rect, border), style);
            }
        }

        /// <summary>
        /// 0 - SerializedProperty, 1 - Rect
        /// </summary>
        public static void PropertyField(object[] parameters)
        {
            if (parameters.Length < 3)
                return;

            UnityEditor.SerializedProperty property = (UnityEditor.SerializedProperty)parameters[0];
            Rect rect = (Rect)parameters[1];
            string fieldName = (string)parameters[2];

            EnigmaticGUI.PropertyField(rect.position, property, rect.width, fieldName);
        }

        /// <summary>
        /// 0 - SerializedProperty, 1 - Rect
        /// </summary>
        public static void EPropertyField(object[] parameters)
        {
            if (parameters.Length < 2)
                return;

            SerializedProperty property = (SerializedProperty)parameters[0];
            Rect rect = (Rect)parameters[1];

            EnigmaticGUI.PropertyField(property, rect.position, rect.width);
        }

        /// <summary>
        /// 0 - Rect, 1 - IsExpanded, 2 - displayName
        /// </summary>
        public static void Foldout(object[] parameters)
        {
            if (parameters.Length < 3)
                return;

            Rect rect = (Rect)parameters[0];
            bool isExpanded = (bool)parameters[1];
            string displayName = (string)parameters[2];

            EnigmaticGUI.Foldout(rect.position, isExpanded, displayName, rect.width);
        }
    }

    public static class EnigmaticGUILayoutDrawerUtility
    {
        private static Queue<PostActionCaller> sm_DrawerCash = new Queue<PostActionCaller>();

        public static PostActionCaller GetDrawer(object[] parameters, Action<object[]> action)
        {
            if (sm_DrawerCash.Count == 0)
                sm_DrawerCash.Enqueue(new PostActionCaller());

            PostActionCaller drawer = sm_DrawerCash.Dequeue();
            drawer.Init(parameters, action);
            return drawer;
        }

        public static void ReturnDrawer(PostActionCaller drawer)
        {
            if (sm_DrawerCash.Count > 50)
                return;

            drawer.Init(null, null);
            sm_DrawerCash.Enqueue(drawer);
        }

        public static PostActionCaller Lable(string text, Vector2 position, GUIStyle style = null)
        {
            object[] parametors = { text, position, style };
            Action<object[]> action = GUIElementDrawer.Lable;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller Image(Rect rect, GUIStyle style = null)
        {
            object[] parametors = { rect, style };
            Action<object[]> action = GUIElementDrawer.Image;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller Button(Rect rect, string text = "", GUIStyle style = null)
        {
            object[] parametors = { rect, text, style };
            Action<object[]> action = GUIElementDrawer.Button;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller FloatField(Rect rect, int guid, float value, string name = "")
        {
            object[] parametors = { rect, guid, value, name };
            Action<object[]> action = GUIElementDrawer.FloatField;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller IntField(Rect rect, int guid, int value, string name = "")
        {
            object[] parametors = { rect, guid, value, name };
            Action<object[]> action = GUIElementDrawer.IntField;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller TextField(Rect rect, int guid, string value, string name = "")
        {
            object[] parametors = { rect, guid, value, name };
            Action<object[]> action = GUIElementDrawer.TextField;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller Grup(GUIGrup grup, int border = 0)
        {
            object[] parametors = { grup, border };
            Action<object[]> action = GUIElementDrawer.Grup;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller PropertyField(UnityEditor.SerializedProperty property, Rect rect, string fieldName = "")
        {
            object[] parametors = { property, rect, fieldName };
            Action<object[]> action = GUIElementDrawer.PropertyField;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller PropertyField(SerializedProperty property, Rect rect)
        {
            object[] parametors = { property, rect };
            Action<object[]> action = GUIElementDrawer.EPropertyField;
            return GetDrawer(parametors, action);
        }

        public static PostActionCaller Foldout(Rect rect, bool isExpanded, string displayName)
        {
            object[] parametors = { rect, isExpanded, displayName };
            Action<object[]> action = GUIElementDrawer.Foldout;
            return GetDrawer(parametors, action);
        }
    }

    public static class EnigmaticGUILayout
    {
        private static GUIGrup GUIActiveGrup;
        private static GUIGrup LastGUIGrup;
        private static Rect sm_LastGUIRect;

        private static bool sm_IsClick;
 
        private static Queue<PostActionCaller> sm_DrawQueue = new Queue<PostActionCaller>();

        public static GUIGrup GetActiveGrup()
        {
            return GUIActiveGrup;
        }

        public static GUIGrup GetLastGrup()
        {
            return LastGUIGrup;
        }

        public static void BeginHorizontal(params EnigmaticGUILayoutOption[] options)
        {
            Rect rect = new Rect(Vector2.zero, Vector2.zero);

            if (GUIActiveGrup != null)
                rect = new Rect(GUIActiveGrup.GetNext().position, Vector2.zero);

            BeginHorizontal(rect, options);
        }

        public static void BeginHorizontal(GUIStyle style, params EnigmaticGUILayoutOption[] options)
        {
            BeginHorizontal(options);
            GUIActiveGrup.Style = style;

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Grup(GUIActiveGrup);
            AddDrawerQueue(drawer);
        }

        public static void BeginHorizontal(Rect rect, GUIStyle style, int border = 0, params EnigmaticGUILayoutOption[] options)
        {
            BeginHorizontal(rect, options);
            GUIActiveGrup.Style = style;

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Grup(GUIActiveGrup, border);
            AddDrawerQueue(drawer);
        }

        public static void BeginHorizontal(GUIStyle style, int border = 0, params EnigmaticGUILayoutOption[] options)
        {
            BeginHorizontal(options);
            GUIActiveGrup.Style = style;

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Grup(GUIActiveGrup, border);
            AddDrawerQueue(drawer);
        }

        public static void BeginHorizontal(Rect rect, params EnigmaticGUILayoutOption[] options)
        {
            GUIGrup grup = new GUIGrup(rect, GrupSortType.Horizontal);
            grup.ApplyOptions(options);

            if (GUIActiveGrup != null)
                GUIActiveGrup.AddElement(grup);

            ChangeActiveGrup(grup);
        }

        public static bool EndHorizontal()
        {
            return EndGrup();
        }

        public static void BeginVertical(Rect rect, params EnigmaticGUILayoutOption[] options)
        {
            GUIGrup grup = new GUIGrup(rect, GrupSortType.Veritical);
            grup.ApplyOptions(options);

            if (GUIActiveGrup != null)
                GUIActiveGrup.AddElement(grup);

            ChangeActiveGrup(grup);
        }

        public static void BeginVertical(params EnigmaticGUILayoutOption[] options)
        {
            Vector2 position = Vector2.zero;

            if (GUIActiveGrup != null)
                position = GUIActiveGrup.GetNext().position;

            Rect rect = new Rect(position, Vector2.zero);
            BeginVertical(rect, options);
        }

        public static void BeginVertical(Rect rect, GUIStyle style, int border = 0, params EnigmaticGUILayoutOption[] options)
        {
            BeginVertical(rect, options);
            GUIActiveGrup.Style = style;

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Grup(GUIActiveGrup, border);
            AddDrawerQueue(drawer);
        }

        public static void BeginVertical(GUIStyle style, int border = 0, params EnigmaticGUILayoutOption[] options)
        {
            BeginVertical(options);
            GUIActiveGrup.Style = style;

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Grup(GUIActiveGrup, border);
            AddDrawerQueue(drawer);
        }

        public static bool EndVertical()
        {
            return EndGrup();
        }

        public static void BeginScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, bool alwaysShowHorizontal,
            bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, 
            params EnigmaticGUILayoutOption[] options)
        {
            ScrollView scrollView = new ScrollView(rect, viewRect, scrollPosition, 
                alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, 
                background, GrupSortType.Veritical);

            scrollView.ApplyOptions(ExpandHeight(true), ExpandWidth(true));
            scrollView.ApplyOptions(options);
            scrollView.UpdatePosition();

            if (GUIActiveGrup != null)
                GUIActiveGrup.AddElement(scrollView);

            GUIActiveGrup = scrollView;
        }

        public static void BeginScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, 
            bool alwaysShowHorizontal = true, bool alwaysShowVertical = true, params EnigmaticGUILayoutOption[] options)
        {
            BeginScrollView(rect, viewRect, scrollPosition, alwaysShowHorizontal, alwaysShowVertical, 
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
        }

        public static void BeginHorizontalScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, 
            params EnigmaticGUILayoutOption[] options)
        {
            BeginScrollView(rect, viewRect, scrollPosition, true, false,
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
        }

        public static void BeginVerticalScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, 
            params EnigmaticGUILayoutOption[] options)
        {
            BeginScrollView(rect, viewRect, scrollPosition, false, true,
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
        }

        public static Vector2 EndScrollView(Action action)
        {
            if (GUIActiveGrup is ScrollView scrollView == false)
                throw new Exception();

            Vector2 result = EnigmaticGUI.ScrollView(scrollView.Rect, scrollView.ViewRect, scrollView.ScrollPosition, 
                scrollView.AlwaysShowHorizontal, scrollView.AlwaysShowVertical, scrollView.HorizontalScrollbar, 
                scrollView.VerticalScrollbar, scrollView.Background);

            if(result != scrollView.ScrollPosition)
                action?.Invoke();

            EndGrup();
            return result;
        }

        public static bool EndGrup()
        {
            bool onClick = false;

            if (GUIActiveGrup.IsClicable || sm_IsClick == false)
                onClick = EnigmaticGUIUtility.OnClick(GUIActiveGrup.Rect, 0);

            if (GUIActiveGrup.ParentGrup != null)
                ChangeActiveGrup(GUIActiveGrup.ParentGrup);
            else
                ChangeActiveGrup(null);

            if(GUIActiveGrup == null)
            {
                while(sm_DrawQueue.Count > 0)
                {
                    PostActionCaller drawer = sm_DrawQueue.Dequeue();
                    drawer.Call();
                    EnigmaticGUILayoutDrawerUtility.ReturnDrawer(drawer);
                }

                sm_DrawQueue.Clear();
            }

            if(onClick == true)
                EnigmaticGUIUtility.Repaint();

            return onClick;
        }

        public static bool Button(string text, GUIStyle style = null)
        {
            Vector2 size = CalculateSize(text) + Vector2.right * 6;
            return Button(text, size, style);
        }

        public static bool Button(string text, Vector2 size, GUIStyle style = null)
        {
            Vector2 position = GUIActiveGrup.GetNext().position;
            Rect rect = new Rect(position, size);
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Button(rect, text, style);
            AddDrawerQueue(drawer);

            return EnigmaticGUIUtility.OnClick(rect, 0);
        }

        public static void Lable(string name, GUIStyle style = null)
        {
            Vector2 position = GUIActiveGrup.GetNext().position;
            Vector2 size = CalculateSize(name, style);
            Rect rect = new Rect(position, size);
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Lable(name, position, style);
            AddDrawerQueue(drawer);
        }

        public static void Image(Vector2 size, GUIStyle style = null)
        {
            Vector2 position = GUIActiveGrup.GetNext().position;
            Rect rect = new Rect(position, size);
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Image(rect, style);
            AddDrawerQueue(drawer);
        }

        public static void Port(Vector2 size, GUIStyle style = null)
        {
            float x = GUIActiveGrup.GetNext().position.x;
            float y = GUIActiveGrup.GetNext().position.y + 9 - size.y / 2;

            Vector2 position = new Vector2(x, y);
            Rect rect = new Rect(position, CalculateSize(size));
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Image(rect, style);
            AddDrawerQueue(drawer);
        }

        public static int FloatField(string name, float value, float widthFieldArea = -1)
        {
            Vector2 position = GUIActiveGrup.GetNext().position;
            Vector2 size = CalculateSize(widthFieldArea);
            Rect rect = new Rect(position, size);
            GUIElement element = new GUIElement(rect);
            
            GUIActiveGrup.AddElement(element);

            int guid = GUID.Next();
            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.FloatField(rect, guid, value, name);
            AddDrawerQueue(drawer);

            return guid;
        }

        public static int IntField(string name, int value, float widthFieldArea)
        {
            Vector2 position = GUIActiveGrup.GetNext().position;
            Vector2 size = CalculateSize(widthFieldArea);
            Rect rect = new Rect(position, size);
            GUIActiveGrup.AddElement(new GUIElement(rect));

            int guid = GUID.Next();
            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.IntField(rect, guid, value, name);
            AddDrawerQueue(drawer);

            return guid;
        }

        public static string TextField(string name, string value, float widthFieldArea)
        {
            Vector2 position = GetNextPosition() + Vector2.up;
            Vector2 size = CalculateSize(widthFieldArea);
            Rect rect = new Rect(position, size);
            GUIActiveGrup.AddElement(new GUIElement(rect));

            int guid = GUID.Next(name, rect, value);
            
            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.TextField(rect, guid, value, name);
            AddDrawerQueue(drawer);

            if (GUIValueCasher.TryGetValue(guid, out object result) == false)
                result = value;

            return (string)result;
        }

        public static void Space(float space)
        {
            Vector2 position = GetNextPosition();

            Rect rect = new Rect();

            if (GUIActiveGrup.SortType == GrupSortType.Horizontal)
                rect = new Rect(position, Vector2.right * space);
            else if (GUIActiveGrup.SortType == GrupSortType.Veritical)
                rect = new Rect(position, Vector2.up * space);

            GUIActiveGrup.AddElement(new GUIElement(rect));

            UpdateLastGUIRect(rect);
        }

        public static void Property(SerializedProperty property, float widthFieldArea = -1)
        {
            if (widthFieldArea == -1)
                widthFieldArea = GUIActiveGrup.GetFreeArea().x - 6;

            Vector2 position = GetNextPosition();
            GUIElement element = new GUIElement(new Rect(position, CalculateSize(property, widthFieldArea)));
            GUIActiveGrup.AddElement(element);
        } //Old

        public static void PropertyField(UnityEditor.SerializedProperty property, float widthFieldArea = -1, string fieldName = "")
        {
            if (widthFieldArea == -1)
                widthFieldArea = GUIActiveGrup.GetFreeArea().x - 6;

            Vector2 position = GetNextPosition();
            Rect rect = new Rect(position, CalculateSize(property, widthFieldArea));
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.PropertyField(property, rect, fieldName);
            AddDrawerQueue(drawer);
        }

        public static void PropertyField(SerializedProperty property, float widthFieldArea = -1)
        {
            if (widthFieldArea == -1)
                widthFieldArea = GUIActiveGrup.GetFreeArea().x - 6;

            Vector2 position = GetNextPosition();
            Rect rect = new Rect(position, CalculateSize(property, widthFieldArea));
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);

            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.PropertyField(property, rect);
            AddDrawerQueue(drawer);
        } //Old

        public static bool Foldout(bool isExpanded, string displayName, float width = -1)
        {
            if (width == -1)
                width = GUIActiveGrup.GetFreeArea().x - 6;

            Vector2 position = GetNextPosition();
            Rect rect = new Rect(position, CalculateSize(width));
            GUIElement element = new GUIElement(rect);
            GUIActiveGrup.AddElement(element);

            UpdateLastGUIRect(rect);
            
            PostActionCaller drawer = EnigmaticGUILayoutDrawerUtility.Foldout(rect, isExpanded, displayName);
            AddDrawerQueue(drawer);

            return EnigmaticGUIUtility.OnClick(rect, 0);
        }

        public static Vector2 GetNextPosition()
        {
            if(GUIActiveGrup == null)
                return Vector2.zero;

            return GUIActiveGrup.GetNext().position;
        }

        public static EnigmaticGUILayoutOption Padding(float pudding)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetPudding, pudding);
        }

        public static EnigmaticGUILayoutOption ElementSpacing(float spacing)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetElementSpacing, spacing);
        }

        public static EnigmaticGUILayoutOption ExpandWidth(bool isExpand)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetExpandWidth, isExpand);
        }

        public static EnigmaticGUILayoutOption ExpandHeight(bool isExpand)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetExpandHeight, isExpand);
        }

        public static EnigmaticGUILayoutOption Width(float width)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetWidth, width);
        }

        public static EnigmaticGUILayoutOption Height(float height)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetHeight, height);
        }

        public static EnigmaticGUILayoutOption Clickable(bool isClicable)
        {
            return new EnigmaticGUILayoutOption(EnigmaticGUILayoutOption.TypeOption.SetClickable, isClicable);
        }

        public static Vector2 CalculateSize(float widthFieldArea)
        {
            return new Vector2(widthFieldArea, 18);
        }

        public static Vector2 CalculateSize(string name, GUIStyle style = null)
        {
            if (style == null)
                style = new GUIStyle(GUI.skin.label);

            return style.CalcSize(new GUIContent(name)) + Vector2.one * 2;
        }

        public static Vector2 CalculateSize(Vector2 size)
        {
            return size;
        }

        public static Vector2 CalculateSize(SerializedProperty property, float widthFieldArea)
        {
            Vector2 result = Vector2.zero;

            result += CalculateSize(widthFieldArea);

            if(property.IsArray == true || property.IsList == true)
            {
                if (property.IsExpanded)
                {
                    for (int i = 0; i < property.ChildPropertes.Length; i++)
                    {
                        result.y += CalculateSize(property.ChildPropertes[i], widthFieldArea).y;

                        if (i < property.ChildPropertes.Length)
                            result.y += 6;
                    }
                }
            }

            return result;
        } //Old

        public static Vector2 CalculateSize(UnityEditor.SerializedProperty property, float widthFieldArea)
        {
            return new Vector2(widthFieldArea, EditorGUI.GetPropertyHeight(property, true));
        } 

        public static float GetElementSize(Vector2 size)
        {
            if (GUIActiveGrup.SortType == GrupSortType.Horizontal)
                return size.x;
            else if (GUIActiveGrup.SortType == GrupSortType.Veritical)
                return size.y;

            return -1;
        }

        public static void UpdateLastGUIRect(Rect rect) => sm_LastGUIRect = rect;

        public static Rect GetLastGUIRect() => sm_LastGUIRect;

        private static void ChangeActiveGrup(GUIGrup grup)
        {
            LastGUIGrup = GUIActiveGrup;
            GUIActiveGrup = grup;
        }

        private static void AddDrawerQueue(PostActionCaller drawer)
        {
            sm_DrawQueue.Enqueue(drawer);
        }
    }

    public class EnigmaticGUILayoutOption
    {
        public enum TypeOption
        {
            SetPudding,
            SetElementSpacing,
            SetExpandWidth,
            SetExpandHeight,
            SetWidth,
            SetHeight,
            SetClickable
        }

        public TypeOption Type { get; private set; }
        public object Value { get; private set; }

        public EnigmaticGUILayoutOption(TypeOption type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    public class GUID
    {
        private static System.Random sm_Random = new System.Random();

        public static int Next()
        {
            return sm_Random.Next(int.MinValue, int.MaxValue);
        }

        public static int Next(int guid)
        {
            if (guid == 0)
                return Next();

            return guid;
        }

        public static int Next(string guidControllName, Rect rect, string value)
        {
            string combinedInput = $"{guidControllName}_{rect.x}_{rect.y}_{rect.width}_{rect.height}_{value}";
            return combinedInput.GetHashCode();
        }
    }

    public static class GUIValueCasher
    {
        private static Dictionary<int, object> sm_ValueCashed = new Dictionary<int, object>();
        
        public static void CashValue(int guid, object value)
        {
            if(sm_ValueCashed.ContainsKey(guid))
                sm_ValueCashed[guid] = value;
            else 
                sm_ValueCashed.Add(guid, value);
        }

        public static object GetValue(int guid)
        {
            if (sm_ValueCashed.ContainsKey(guid) == false)
                throw new Exception("");

            object value = sm_ValueCashed[guid];
            sm_ValueCashed.Remove(guid);
            return value;
        }

        public static bool TryGetValue(int guid, out object value)
        {
            value = null;

            if (sm_ValueCashed.ContainsKey(guid) == false)
                return false;

            value = sm_ValueCashed[guid];
            sm_ValueCashed.Remove(guid);
            return true;
        }

        public static void Clear() => sm_ValueCashed.Clear();
    }

    public static class EnigmaticGUI
    {
        public readonly static Color SelectionColor = new Color(0.6039f, 0.8117f, 1f);

        private readonly static int s_WindowTopPadding = 19;

        private static Matrix4x4 s_GUIMatrixOrigin;

        private static Rect sm_LastGUIRect;

        public static void BeginSelectedGrup(bool selected)
        {
            if (selected)
                GUI.backgroundColor = SelectionColor;
            //GUI.contentColor = SelectionColor;
        }

        public static void EndSelectedGrup()
        {
            GUI.backgroundColor = Color.white;
        }

        public static bool Button(string text, Vector2 position, params GUILayoutOption[] gUILayoutOptions)
        {
            bool result;

            GUILayout.BeginArea(new Rect(position.x, position.y, 100, 100));
            result = GUILayout.Button(text, gUILayoutOptions);
            GUILayout.EndArea();

            return result;
        } // Check

        public static void Image(Vector2 size, string text, GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            {
                float offset = size.y / 2;

                GUILayout.Space(offset);

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(offset);

                    GUILayout.Box(text, EnigmaticStyles.Port,
                        GUILayout.Width(size.x), GUILayout.Height(size.y));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        } //Delete

        public static void sLabel(string text, GUIStyle style = null, float s = 0)
        {
            if(style == null)
                style = new GUIStyle(GUI.skin.label);

            float width = EnigmaticGUILayout.CalculateSize(text).x;
            GUILayout.Label(text, style, GUILayout.MinWidth(width));
        }  //Delete

        public static void PropertyField(UnityEditor.SerializedProperty property, float fieldWidth)
        {
            if(property.isArray)
            {
                EditorGUILayout.PropertyField(property, new GUIContent(property.displayName), GUILayout.MinWidth(30));
                return;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal(GUILayout.Width(fieldWidth / 2), GUILayout.MaxWidth(fieldWidth / 2));
                {
                    sLabel(property.displayName);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Width(fieldWidth / 2), GUILayout.MaxWidth(fieldWidth / 2), GUILayout.MinWidth(30));
                {
                    EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.MinWidth(60));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        } //Delete

        public static void PropertyField(Vector2 position, UnityEditor.SerializedProperty property, float fieldWidth, string fieldName = "")
        {
            float height = EditorGUI.GetPropertyHeight(property, true);
            Rect fieldRect = new Rect(position, new Vector2(fieldWidth, height));

            if(string.IsNullOrEmpty(fieldName))
                fieldName = property.displayName;

            if (property.isArray)
            {
                EditorGUI.PropertyField(fieldRect, property, new GUIContent(fieldName));
                return;
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldWidth / 2;

            EditorGUI.PropertyField(fieldRect, property, new GUIContent(fieldName));

            UpdateLastGUIRect(new Rect(position, new Vector2(fieldWidth, height)));

            EditorGUIUtility.labelWidth = labelWidth;
        } //Delete

        public static bool Foldout(Vector2 position, bool isExpanded, string displayName, float width, 
            GUIStyle background = null, GUIStyle lable = null, GUIStyle foldout = null)
        {
            bool result = false;

            Rect rectBackground = new Rect(position, new Vector2(width, 18));
            Rect rectFoldout = new Rect(position + Vector2.right * 6, Vector2.one * 16);

            Image(rectBackground, background);
            Image(rectFoldout, foldout);
            //Label(displayName, )

            //result = EditorGUI.Foldout(rect, isExpanded, displayName);
            //UpdateLastGUIRect(rect);

            return result;
        }

        public static bool Button(string text, Vector2 position, GUIStyle style = null)
        {
            if (style == null)
                style = GUI.skin.button;

            Vector2 size = EnigmaticGUILayout.CalculateSize(text, style) + Vector2.one * 6;
            return Button(text, position, size, style);
        }

        public static bool Button(string text, Vector2 position, Vector2 size, GUIStyle style = null)
        {
            if (style == null)
                style = GUI.skin.button;

            GUIStyle styleToUse = style;

            if (EnigmaticGUIUtility.IsHover(new Rect(position, size)))
            {
                GUIStyle hoveredStyle = new GUIStyle(style);
                hoveredStyle.normal.background = style.hover.background;

                GUI.Box(new Rect(position, size), "", EnigmaticStyles.whiteBox);
            }

            GUI.Box(new Rect(position, size), text, styleToUse);
            //EnigmaticGUIUtility.Repaint();

            return EnigmaticGUIUtility.OnClick(new Rect(position, size), 0);
        }

        public static bool Button(string text, float x, float y, params GUILayoutOption[] gUILayoutOptions)
        {
            return Button(text, new Vector2(x, y), gUILayoutOptions);
        } // Delete

        public static void BeginZoomedArea(float zoomSacle, Rect zoomendArea)
        {
            GUI.EndGroup();

            Rect area = zoomendArea.ScaleSize(1 / zoomSacle);
            area.y += s_WindowTopPadding;// * zoomSacle;
            GUI.BeginGroup(area);

            s_GUIMatrixOrigin = GUI.matrix;
            Matrix4x4 tramslation = Matrix4x4.TRS(area.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomSacle, zoomSacle, 0.1f));
            GUI.matrix = tramslation * scale * tramslation.inverse * GUI.matrix;
        }

        public static void EndZoomedArea(float zoomSacle, Rect windowRect)
        {
            GUI.matrix = s_GUIMatrixOrigin;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, s_WindowTopPadding, Screen.width, Screen.height));
        }

        public static Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords, Rect zoomedArea, float zoomScale, Vector2 zoomCoordsOrigin)
        {
            return (screenCoords - zoomedArea.TopLeft()) / zoomScale + zoomCoordsOrigin;
        }

        public static float DrawFloatField(string fieldName, float value, float space = 3)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(fieldName);
            GUILayout.Space(space);
            float result = EditorGUILayout.FloatField(value);

            EditorGUILayout.EndHorizontal();

            return result;
        }

        public static void Label(string name, Vector2 position, GUIStyle style = null)
        {
            if (style == null)
                style = new GUIStyle(EditorStyles.label);

            Vector2 lableSize = style.CalcSize(new GUIContent(name)) + Vector2.one * 2;
            GUI.Label(new Rect(position, lableSize), name, style);
            UpdateLastGUIRect(new Rect(position, lableSize));
        }

        public static void Image(Rect rect, GUIStyle style = null)
        {
            if (style == null)
                style = new GUIStyle(GUI.skin.box);

            GUI.Box(rect, "", style);
            UpdateLastGUIRect(rect);
        }

        public static float FloatField(string name, float value, Vector2 position, float widthFieldArea)
        {
            Vector2 lableSize = new Vector2(widthFieldArea / 2, 18f);
            GUI.Label(new Rect(position, lableSize), name);

            Vector2 fieldPosition = position + Vector2.right * (widthFieldArea / 2);
            Vector2 fieldSize = new Vector2(widthFieldArea / 2, 18f);
            float result = EditorGUI.FloatField(new Rect(fieldPosition, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static float FloatField(float value, Vector2 position, float widthFieldArea)
        {
            Vector2 fieldSize = new Vector2(widthFieldArea, 18f);
            float result = EditorGUI.FloatField(new Rect(position, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static int IntField(string name, int value, Vector2 position, float widthFieldArea)
        {
            Vector2 lableSize = new Vector2(widthFieldArea / 2, 18f);
            GUI.Label(new Rect(position, lableSize), name);

            Vector2 fieldPosition = position + Vector2.right * (widthFieldArea / 2);
            Vector2 fieldSize = new Vector2(widthFieldArea / 2, 18f);
            int result = EditorGUI.IntField(new Rect(fieldPosition, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static int IntField(int value, Vector2 position, float widthFieldArea)
        {
            Vector2 fieldSize = new Vector2(widthFieldArea, 18f);
            int result = EditorGUI.IntField(new Rect(position, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static string TextField(string name, string value, Vector2 position, float widthFieldArea)
        {
            Vector2 lableSize = new Vector2(widthFieldArea / 2, 18f);
            GUI.Label(new Rect(position, lableSize), name);

            Vector2 fieldPosition = position + Vector2.right * (widthFieldArea / 2);
            Vector2 fieldSize = new Vector2(widthFieldArea / 2, 18f);
            string result = EditorGUI.TextField(new Rect(fieldPosition, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static string TextField(string value, Vector2 position, float widthFieldArea)
        {
            Vector2 fieldSize = new Vector2(widthFieldArea, 18f);   
            string result = EditorGUI.TextField(new Rect(position, fieldSize), value);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return result;
        }

        public static string EnumField(string name, int value, Type enumType, Vector2 position, float widthFieldArea)
        {
            if (enumType.IsEnum == false)
                return string.Empty;

            string[] values = Enum.GetNames(enumType);
            int result = value;

            Vector2 lableSize = new Vector2(widthFieldArea / 2, 18f);
            GUI.Label(new Rect(position, lableSize), name);

            Vector2 fieldPosition = position + Vector2.right * (widthFieldArea / 2);
            Vector2 fieldSize = new Vector2(widthFieldArea / 2, 18f);
            result = EditorGUI.Popup(new Rect(fieldPosition, fieldSize), result, values);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return values[result];
        } //Make varialtin without name arg. 

        public static bool Foldout(string name, bool foldDown, Vector2 position, float widthFieldArea)
        {
            float newWidthFieldArea = widthFieldArea - (18f + 6f);
            bool resultFoldDown = foldDown;

            if (resultFoldDown == false)
                Image(new Rect(position, Vector2.one * 18f), EnigmaticStyles.foldoutButtonClose);
            else
                Image(new Rect(position, Vector2.one * 18f), EnigmaticStyles.foldoutButtonOpen);

            if (EnigmaticGUIUtility.OnClick(new Rect(position, new Vector2(widthFieldArea, 18)), 0))
                resultFoldDown = !resultFoldDown;

            Vector2 lablePostion = position + Vector2.right * (18f);
            Label(name, lablePostion);

            return resultFoldDown;
        }

        public static bool FoldoutArray(string name, bool foldDown, Vector2 position, float widthFieldArea, int count, out int outCount)
        {
            float newWidthFieldArea = widthFieldArea - 18f;
            bool resultFoldDown = foldDown;

            if (resultFoldDown == false)
                Image(new Rect(position, Vector2.one * 18f), EnigmaticStyles.foldoutButtonClose);
            else
                Image(new Rect(position, Vector2.one * 18f), EnigmaticStyles.foldoutButtonOpen);

            Vector2 lablePostion = position + Vector2.right * 18f;
            Label(name, lablePostion);

            Vector2 fieldPosition = lablePostion + Vector2.right * (newWidthFieldArea / 2);
            Vector2 fieldSize = new Vector2(newWidthFieldArea / 4, 18f);
            outCount = EditorGUI.IntField(new Rect(fieldPosition, fieldSize), count);

            if (GUI.Button(new Rect(fieldPosition + Vector2.right * (fieldSize.x + 3),
                new Vector2(16, 14)), "", EnigmaticStyles.addButton))
            {
                outCount += 1;
            }

            if (GUI.Button(new Rect(fieldPosition + Vector2.right * (fieldSize.x + 24),
                new Vector2(16, 14)), "", EnigmaticStyles.substractButton))
            {
                outCount -= 1;
            }

            outCount = Math.Clamp(outCount, 0, 1000);
            UpdateLastGUIRect(new Rect(position, new Vector2(widthFieldArea, 18f)));

            return resultFoldDown;
        }

        public static void PropertyField(SerializedProperty property, Vector2 position, float widthFieldArea)
        {
            if (PropertyDrawerRegister.GetDrawer(property.Type) != null)
            {
                PropertyDrawer drawer = PropertyDrawerRegister.GetDrawer(property.Type);
                drawer.Draw(property, position, widthFieldArea);
            }
            else if (property.IsArray == true || property.IsList == true)
            {
                property.IsExpanded = FoldoutArray(property.Name, property.IsExpanded, position,
                    widthFieldArea, property.ChildPropertes.Length, out int count);

                if (count != property.ChildPropertes.Length)
                {
                    if (count > property.ChildPropertes.Length)
                    {
                        int delta = count - property.ChildPropertes.Length;

                        for (int i = 0; i < delta; i++)
                            property.AddChildProperty();
                    }
                    else
                    {
                        int delta = property.ChildPropertes.Length - count;

                        for (int i = 0; i < delta; i++)
                            property.RemoveChildProperty();
                    }
                }

                if (property.IsExpanded)
                {
                    for (int i = 0; i < property.ChildPropertes.Length; i++)
                    {
                        Vector2 newPosition = new Vector2(position.x + 18f, GetLastGUIRect().position.y + 24);
                        PropertyField(property.ChildPropertes[i], newPosition, widthFieldArea - 18f);
                    }
                }

                float width = widthFieldArea;
                float height = (GetLastGUIRect().position - position).y + 16;
                Vector2 size = new Vector2(width, height);

                UpdateLastGUIRect(new Rect(position, size));
            }
            else if (property.IsClass == true)
            {
                property.IsExpanded = Foldout(property.Name, property.IsExpanded, position, widthFieldArea);

                if (property.IsExpanded)
                {
                    for (int i = 0; i < property.ChildPropertes.Length; i++)
                    {
                        Vector2 newPosition = new Vector2(position.x + 18f, GetLastGUIRect().position.y + 24);
                        PropertyField(property.ChildPropertes[i], newPosition, widthFieldArea);
                    }
                }
            }
            else
            {
                object value = property.Value;

                if (property.Type == typeof(string))
                    value = TextField(property.Name, (string)property.Value, position, widthFieldArea);
                else if (property.Type == typeof(int))
                    value = IntField(property.Name, (int)property.Value, position, widthFieldArea);
                else if (property.Type == typeof(float))
                    value = FloatField(property.Name, (float)property.Value, position, widthFieldArea);

                property.Value = value;
            }

            property.ApplyValue();
        }

        public static void DrawProperties(SerializedProperty[] propertes, float widthFieldArea)
        {
            Vector2 lastRectPosition = GetLastGUIRect().position;

            foreach (SerializedProperty property in propertes)
            {
                Vector2 position = new Vector2(lastRectPosition.x, GetLastGUIRect().position.y + 24);
                PropertyField(property, position, widthFieldArea);
                property.ApplyValue();
            }
        }

        public static void DrawProperty(UnityEditor.SerializedProperty property, bool drawChildren)
        {
            string lastPropertyPath = string.Empty;

            foreach (UnityEditor.SerializedProperty p in property)
            {
                if (p.isArray == true && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperty(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(lastPropertyPath) == false
                        && p.propertyPath.Contains(lastPropertyPath))
                        continue;

                    lastPropertyPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        public static Vector2 ScrollView(Rect rect, Rect viewRect, Vector2 scrollPosition, bool alwaysShowHorizontal, 
            bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
        {
            //GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView

            Vector2 result = scrollPosition;

            if(viewRect.Contains(EditorInput.GetMousePosition()))
            {
                EditorInput.UpdateInput();

                if (EditorInput.GetButtonPress(KeyCode.LeftShift) && alwaysShowHorizontal)
                    result.x -= EditorInput.GetMouseScrollWheel() * 10;
                else if(alwaysShowVertical)
                    result.y -= EditorInput.GetMouseScrollWheel() * 10;

                float clampWidth = Mathf.Clamp(rect.width / viewRect.width, 1, float.MaxValue);
                float clampHeight = Mathf.Clamp(rect.height / viewRect.height, 1, float.MaxValue);

                float x = Mathf.Clamp(result.x, -rect.width, 0);
                float y = Mathf.Clamp(result.y, -(rect.height - viewRect.height), 0);

                result = new Vector2(x, y);
            }

            return result;
        }

        public static Rect GetFixedBox(Rect rect, int border)
        {
            return GetFixedBox(rect, new RectOffset(border, border, border, border));
        }

        public static Rect GetFixedBox(Rect rect, GUIStyle style)
        {
            int left = style.border.left - style.padding.left;
            int right = style.border.right - style.padding.right;
            int top = style.border.top - style.padding.top;
            int bottom = style.border.bottom - style.padding.bottom;

            RectOffset offset = new RectOffset(left, right, top, bottom);

            return GetFixedBox(rect, offset);
        }

        public static Rect GetFixedBox(Rect rect, RectOffset offset)
        {
            Vector2 position = new Vector2(rect.x - offset.left, rect.y - offset.top);

            Vector2 size = new Vector2(rect.width + (offset.right + offset.left), 
                rect.height + (offset.bottom + offset.top));

            return new Rect(position, size);
        }

        public static Rect GetLastGUIRect() => sm_LastGUIRect;

        public static void UpdateLastGUIRect(Rect rect) => sm_LastGUIRect = rect;
    }

    public static class EnigmaticGUIUtility
    {
        public static bool OnClick(Rect rect, int mouseButton)
        {
            Event e = Event.current;

            if(rect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseUp && e.button == mouseButton)
                    return true;
            }

            return false;
        }

        public static bool IsHover(Rect rect)
        {
            Event e = Event.current;
            return rect.Contains(e.mousePosition);
        }

        public static bool TryGetActioveWindow(out EditorWindow window)
        {
            window = GetActioveWindow();
            //Debug.Log(window != null);
            return window != null;
        }

        public static EditorWindow GetActioveWindow()
        {
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            Event e = Event.current;

            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);

            foreach (EditorWindow window in windows)
            {
                if (window.position.Contains(mousePosition))
                    return window;
            }

            return null;
        }

        public static void Repaint()
        {
            if(TryGetActioveWindow(out EditorWindow window))
                window.Repaint();

            //Debug.Log("Repaint");
        }
    }

    public static class PropertyDrawerRegister
    {
        private static Dictionary<Type, PropertyDrawer> s_PropertyDrawers = new Dictionary<Type, PropertyDrawer>();

        private static void Init()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(PropertyDrawer).IsAssignableFrom(type))
                    RegisterPropertyDrawer(type);
            }
        }

        private static void RegisterPropertyDrawer(Type drawerType)
        {
            CustomPropertyDrawer customPropertyDrawer = (CustomPropertyDrawer)Attribute.GetCustomAttribute
                (drawerType, typeof(CustomPropertyDrawer));

            if (customPropertyDrawer == null)
                return;

            PropertyDrawer propertyDrawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
            s_PropertyDrawers.Add(customPropertyDrawer.PropertyType, propertyDrawer);
        }

        public static PropertyDrawer GetDrawer(Type type)
        {
            if (s_PropertyDrawers.ContainsKey(type) == false)
                return null;

            return s_PropertyDrawers[type];
        }
    }

    public class PropertyDrawer
    {
        public virtual void Draw(object instence, FieldInfo fieldInfo, Vector2 position, float widthFieldArea) { }
        public virtual void Draw(SerializedProperty property, Vector2 position, float widthFieldArea) { }
    }

    public class SerializedProperty
    {
        public bool IsExpanded;

        public object Value;
        private List<SerializedProperty> m_ChildPropertes = new List<SerializedProperty>();

        public object Instance { get; private set; }
        public FieldInfo FieldInfo { get; private set; }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsStruct { get; private set; }
        public bool IsGenericType { get; private set; }
        public bool IsList { get; private set; }
        public bool IsArray { get; private set; }
        public bool IsEnum { get; private set; }
        public bool IsArrayElement { get; private set; }

        public SerializedProperty[] ChildPropertes => m_ChildPropertes.ToArray();

        public SerializedProperty(FieldInfo fieldInfo, object instance)
        {
            FieldInfo = fieldInfo;
            Instance = instance;

            if (instance != null)
                Value = FieldInfo.GetValue(Instance);

            Type = FieldInfo.FieldType;
            Name = FieldInfo.Name;

            InitType();

            IsArrayElement = false;
            InitChildren();
        }

        public SerializedProperty(string name, object instance, Type type, object value)
        {
            Instance = instance;

            Type = type;
            Name = name;

            InitType();

            IsArrayElement = true;
            Value = value;

            InitChildren();
        }

        public void AddChildProperty()
        {
            if (m_ChildPropertes.Count == 0)
                m_ChildPropertes.Add(CreateChildProperty());
            else
                m_ChildPropertes.Add(m_ChildPropertes.Last().Clone());
        }

        public void RemoveChildProperty()
        {
            m_ChildPropertes.Remove(m_ChildPropertes.Last());
        }

        private void InitType()
        {
            IsClass = Type.IsClass && Type != typeof(string) && IsGenericType == false && IsArray == false;
            IsStruct = Type.IsStruct();
            IsGenericType = Type.IsGenericType;
            IsList = Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(List<>);
            IsArray = Type.IsArray;
            IsEnum = Type.IsEnum;
        }

        public void InitChildren()
        {
            m_ChildPropertes.Clear();

            if (IsArray)
            {
                if (Value == null)
                    return;

                Array array = (Array)Value;
                Type elementType = Type.GetElementType();

                if (array == null)
                    return;

                for (int i = 0; i < array.Length; i++)
                {
                    SerializedProperty property = new SerializedProperty
                        ($"Element {i}", array, elementType, array.GetValue(i));

                    m_ChildPropertes.Add(property);
                }
            }
            else if (IsList)
            {
                if (Value == null)
                    return;

                IList list = (IList)Value;
                Type elementType = Type.GetGenericArguments()[0];

                int iteration = 0;
                foreach (object element in list)
                {
                    SerializedProperty property = new SerializedProperty($"Element {iteration}", list, elementType, element);
                    m_ChildPropertes.Add(property);
                    iteration++;
                }
            }
            else if (IsClass || IsStruct)
            {
                FieldInfo[] fieldInfos = Type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fieldInfos)
                {
                    SerializebleProperty serializebleProperty = field.GetAttribute<SerializebleProperty>();
                    SerializeField serializeField = field.GetAttribute<SerializeField>();

                    if (serializebleProperty == null
                        && serializeField == null)
                        continue;

                    SerializedProperty property = new SerializedProperty(field, Value);
                    m_ChildPropertes.Add(property);
                }
            }
        }

        public void ApplyValue()
        {
            if (IsArray == true)
            {
                List<SerializedProperty> tempChildProperty = m_ChildPropertes.Clone();

                Type typeElement = Type.GetElementType();

                foreach (SerializedProperty childProperty in m_ChildPropertes)
                {
                    if (childProperty.Type != typeElement)
                    {
                        tempChildProperty.Remove(childProperty);
                        continue;
                    }

                    childProperty.ApplyValue();
                }

                Array newArray = Array.CreateInstance(typeElement, tempChildProperty.Count);

                for (int i = 0; i < newArray.Length; i++)
                    newArray.SetValue(tempChildProperty[i].Value, i);

                Value = newArray;
            }
            else if (IsList == true)
            {
                IList list = (IList)Value;
                Type typeElement = Type.GetGenericArguments()[0];

                foreach (SerializedProperty childProperty in m_ChildPropertes)
                {
                    if (childProperty.Type != typeElement)
                        continue;

                    childProperty.ApplyValue();
                    list.Add(childProperty.Value);
                }

                Value = list;
            }
            else if (IsClass == true || IsStruct == true)
            {
                foreach (SerializedProperty childProperty in m_ChildPropertes)
                    childProperty.ApplyValue();
            }

            if (IsArrayElement == false && Instance != null)
                FieldInfo.SetValue(Instance, Value);
        }

        public SerializedProperty Clone()
        {
            if (IsArrayElement == false)
                throw new InvalidOperationException();

            object value = null;

            if (IsClass == true)
                value = Activator.CreateInstance(Type);
            else
                value = Value;

            SerializedProperty property = new SerializedProperty($"Element {int.Parse(Name.Split(" ")[1]) + 1}", Instance, Type, value);
            return property;
        }

        private SerializedProperty CreateChildProperty()
        {
            if (IsArray == false && IsList == false)
                throw new InvalidOperationException();

            object value = null;
            Type elementType = null;

            if (IsArray)
                elementType = Type.GetElementType();
            else
                elementType = Type.GetGenericArguments()[0];

            if (elementType == typeof(string))
                value = string.Empty;
            else
                value = Activator.CreateInstance(elementType);

            SerializedProperty property = new SerializedProperty("Element 0", Instance, elementType, value);
            return property;
        }
    }
}