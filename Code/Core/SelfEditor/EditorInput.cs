using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Enigmatic.Core.Editor
{
    public static class EditorInput
    {
        public static ProfilerMarker EditorInputHandler = new ProfilerMarker(ProfilerCategory.Input, "EditorInputHandler");

        public static Event Current { get; private set; }
        public static Event Last { get; private set; }

        private static Vector2 sm_LastMousePosition;

        private static Dictionary<KeyCode, bool> sm_IsKeyPressed = new Dictionary<KeyCode, bool>();
        public static EventType EventType => Current.type;

        public static void UpdateInput()
        {
            EditorInputHandler.Begin();

            Last = Current;
            Current = Event.current;

            if (Current.type == EventType.KeyDown || Current.type == EventType.KeyUp
                && Current.isKey)
            {
                KeyCode keyCode = Current.keyCode;

                if (GetButtonDown(keyCode))
                {
                    if (sm_IsKeyPressed.ContainsKey(keyCode) == false)
                        sm_IsKeyPressed.Add(keyCode, false);

                    sm_IsKeyPressed[keyCode] = true;
                }
                else if (GetButtonUp(keyCode)
                    && sm_IsKeyPressed.ContainsKey(keyCode))
                {
                    sm_IsKeyPressed[keyCode] = false;
                }
            }

            EditorInputHandler.End();
        }

        public static Vector2 GetMousePosition()
        {
            return Current.mousePosition;
        }

        public static float GetMouseScrollWheel()
        {
            if (Current.type != EventType.ScrollWheel)
                return 0f;

            return Current.delta.y;
        }

        public static Vector2 GetLastMousePosition()
        {
            return sm_LastMousePosition;
        }

        public static void UpdateLastMousePosition()
        {
            sm_LastMousePosition = GetMousePosition();
        }

        public static bool GetMouseDrag(int button)
        {
            return Current.isMouse && Current.button == button
                && Current.type == EventType.MouseDrag;
        }

        public static bool GetMouseButtonPress(int button)
        {
            EditorInputHandler.Begin();

            if (button == 0)
                return GetMouseLeftButtonPress();
            else if (button == 1)
                return GetMouseRightButtonPress();

            EditorInputHandler.End();

            return false;
        }

        public static bool GetMouseButtonDown(int button)
        {
            return Current.isMouse && Current.button == button
                && Current.type == EventType.MouseDown;
        }

        public static bool GetMouseButtonUp(int button)
        {
            return Current.isMouse && Current.button == button
                && Current.type == EventType.MouseUp;
        }

        public static bool GetButtonPress(KeyCode keyCode)
        {
            if (sm_IsKeyPressed.ContainsKey(keyCode) == false)
                return false;

            if (sm_IsKeyPressed[keyCode] == false)
            {
                sm_IsKeyPressed.Remove(keyCode);
                return false;
            }

            return sm_IsKeyPressed[keyCode];
        }

        public static bool GetButtonDown(KeyCode keyCode)
        {
            return Current.isKey && Current.keyCode == keyCode
                && Current.type == EventType.KeyDown;
        }

        public static bool GetButtonUp(KeyCode keyCode)
        {
            return Current.isKey && Current.keyCode == keyCode
                && Current.type == EventType.KeyUp;
        }

        private static bool GetMouseLeftButtonPress()
        {
            if (sm_IsKeyPressed.ContainsKey(KeyCode.Mouse0) == false)
                sm_IsKeyPressed.Add(KeyCode.Mouse0, false);

            if (GetMouseButtonDown(0) && sm_IsKeyPressed[KeyCode.Mouse0] == false)
                sm_IsKeyPressed[KeyCode.Mouse0] = true;
            else if (GetMouseButtonUp(0))
                sm_IsKeyPressed[KeyCode.Mouse0] = false;

            return sm_IsKeyPressed[KeyCode.Mouse0];
        }

        private static bool GetMouseRightButtonPress()
        {
            if (sm_IsKeyPressed.ContainsKey(KeyCode.Mouse1) == false)
                sm_IsKeyPressed.Add(KeyCode.Mouse1, false);

            if (GetMouseButtonDown(1))
            {
                sm_IsKeyPressed[KeyCode.Mouse1] = true;
            }
            else if (GetMouseButtonUp(1))
            {
                sm_IsKeyPressed.Remove(KeyCode.Mouse1);
                return false;
            }

            return sm_IsKeyPressed[KeyCode.Mouse1];
        }

    }
}
