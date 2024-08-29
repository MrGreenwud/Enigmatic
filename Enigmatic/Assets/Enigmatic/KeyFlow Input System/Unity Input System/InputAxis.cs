using System;
using Enigmatic.Experemental.SearchedWindowUtility;

namespace Enigmatic.KFInputSystem.Editor
{
    public class InputAxis
    {
        public string Tag { get; private set; }
        public string PosetiveButton { get; private set; }
        public string NegativeButton { get; private set; }

        public float Gravity { get; private set; }
        public float Dead { get; private set; }
        public float Sensitivity { get; private set; }

        public int Type { get; private set; }
        public int Axis { get; private set; }

        public InputAxis(string tag, string posetiveButton, string negativeButton, float gravity,
            float dead, float sensitivity, int type, int axis)
        {
            Tag = tag;
            PosetiveButton = posetiveButton;
            NegativeButton = negativeButton;
            Gravity = gravity;
            Dead = dead;
            Sensitivity = sensitivity;
            Type = type;
            Axis = axis;
        }

        public InputAxis(string tag, string type, EditorKFInputSettings inputSettings)
        {
            if (SearchedTreeUtility.DeCompileTree(type, 0) == "Axis")
                throw new InvalidOperationException();

            Tag = tag;
            Type = DeviceParse(inputSettings.Device);

            Gravity = inputSettings.Gravity;
            Dead = inputSettings.Dead;
            Sensitivity = inputSettings.Sensitivity;
            
            PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(inputSettings.Button);
        }

        public InputAxis(string tag, string type, EditorKFInputSettings inputSettings, EditorKFAxisSettings axisSettings)
        {
            if (SearchedTreeUtility.DeCompileTree(type, 0) == "Button")
                throw new InvalidOperationException();

            Tag = tag;
            Type = DeviceParse(inputSettings.Device);

            Gravity = inputSettings.Gravity;
            Dead = inputSettings.Dead;
            Sensitivity = inputSettings.Sensitivity;

            if (inputSettings.Device == "Keyboard")
            {
                PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.PosetiveButton);
                NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.NegativeButton);
            }
            else if (inputSettings.Device == "Mouse")
            {
                Axis = MosusAxisParse(axisSettings.Value);
            }
            else
            {
                Axis = JosticAxisParse(axisSettings.Value);
            }
        }

        public int JosticAxisParse(string value)
        {
            int axis = 0;

            switch(value)
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

        public int MosusAxisParse(string value)
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

        public int DeviceParse(string device)
        {
            if (device == "Keyboard")
                return 0;
            else if (device == "Mouse")
                return 1;
            else if(device == "Joystick")
                return 2;

            throw new InvalidOperationException();
        }
    }
}
