using System;

using Enigmatic.Experimental.SearchedWindowUtility;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal static class InputUtility
    {
        public static int TypeParse(string device, string inputType)
        {
            string type = SearchedTreeUtility.DeCompileTree(inputType, 0);

            if (device == "Keyboard" || type == "Button")
                return 0;
            else if (device == "Mouse" && type == "Axis")
                return 1;
            else if (device == "Joystick" && type == "Axis")
                return 2;

            throw new InvalidOperationException();
        }

        public static int MouseAxisParse(string value)
        {
            int axis = 0;

            switch (value)
            {
                case "MouseX":
                    axis = 0;
                    break;
                case "MouseY":
                    axis = 1;
                    break;
            }

            return axis;
        }

        public static int JosticAxisParse(string value)
        {
            int axis = 0;

            switch (value)
            {
                case "LeftStickX":
                    axis = 0;
                    break;
                case "LeftStickY":
                    axis = 1;
                    break;
                case "RightStickX":
                    axis = 3;
                    break;
                case "RightStickY":
                    axis = 4;
                    break;
            }

            return axis;
        }

        public static string KeyboardButtonParse(string button)
        {
            if (button == "None")
                return button;

            button = SearchedTreeUtility.DeCompileTree(button, 1);
            string result = UnityInputManager.ConvertToUnityInputReadable(button);

            return result;
        }

        public static string GetInputName(string inputTag, string mapName)
        {
            return $"{inputTag} ({mapName})";
        }
    }
}
