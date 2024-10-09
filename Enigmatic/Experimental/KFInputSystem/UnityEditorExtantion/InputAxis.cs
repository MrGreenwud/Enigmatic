using System;
using System.Collections.Generic;

using UnityEngine;

using Enigmatic.Experimental.SearchedWindowUtility;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal class InputAxis
    {
        public string Tag { get; private set; }

        public string PosetiveButton { get; private set; }
        public string NegativeButton { get; private set; }

        public float Gravity { get; private set; }
        public float Dead { get; private set; }
        public float Sensitivity { get; private set; }

        public int Type{ get; private set; }
        public int Axis { get; private set; }

        public InputAxis(string tag, string type, KFInputSettings inputSettings, bool isHorizontal)
        {
            Tag = tag;
            Type = InputUtility.TypeParse(inputSettings.Device, type);

            Gravity = inputSettings.Gravity;
            Dead = inputSettings.Dead;
            Sensitivity = inputSettings.Sensitivity;

            if (SearchedTreeUtility.DeCompileTree(type, 0) == "Axis")
            {
                KFAxisSettings axisSettings = isHorizontal ? inputSettings.HorizontalAxis : inputSettings.VerticalAxis;

                if (inputSettings.Device == "Mouse")
                {
                    Axis = InputUtility.MouseAxisParse(axisSettings.Axis);
                }
                else if (inputSettings.Device == "Joystick")
                {
                    if(axisSettings.Type == "Value")
                    {
                        Axis = InputUtility.JosticAxisParse(axisSettings.Axis);
                    }
                    else
                    {
                        PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.PosetiveButton);
                        NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.NegativeButton);
                    }
                }
                else if(inputSettings.Device == "Keyboard")
                {
                    PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.PosetiveButton);
                    NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axisSettings.NegativeButton);
                }
            }
            else
            {
                if (inputSettings.Device == "Joystick")
                    PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(inputSettings.Button);
                else
                    PosetiveButton = InputUtility.KeyboardButtonParse(inputSettings.Button);

                Debug.Log(PosetiveButton);
            }
        }

        public static InputAxis[] CreateAxis(KFInput input, Device device)
        {
            List<InputAxis> result = new List<InputAxis>(2);

            if (SearchedTreeUtility.DeCompileTree(input.Type, 1) == "Vector2")
            {
                result.Add(new InputAxis($"{input.Tag} X", input.Type, input.GetInputSettings(device), true));
                result.Add(new InputAxis($"{input.Tag} Y", input.Type, input.GetInputSettings(device), false));
            }
            else if(SearchedTreeUtility.DeCompileTree(input.Type, 1) == "Value")
            {
                result.Add(new InputAxis(input.Tag, input.Type, input.GetInputSettings(device), true));
            }

            return null;
        }
    }
}
