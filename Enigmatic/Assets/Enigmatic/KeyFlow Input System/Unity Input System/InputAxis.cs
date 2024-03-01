namespace KFInputSystem.Utility
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

        public InputAxis(string tag, Axis axis)
        {
            Tag = tag;

            Gravity = axis.Gravity;
            Dead = axis.Dead;
            Sensitivity = axis.Sensitivity;

            if (axis.InputType == InputType.Button)
            {
                if (axis.Device == Device.KeyboardOrMouse)
                {
                    PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axis.KeyboardPosetiveButton.ToString());
                    NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axis.KeyboardNegativeButton.ToString());
                }
                else if (axis.Device == Device.Joystick)
                {
                    PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axis.JoystickPosetiveButton.ToString());
                    NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axis.JoystickNegativeButton.ToString());
                }

                Type = 0;
                Axis = 0;
            }
            else if (axis.InputType == InputType.Value)
            {
                if (axis.Device == Device.MouseMovement)
                {
                    Type = 1;
                    Axis = (int)axis.MouseAxis - 1;
                }
                else if (axis.Device == Device.Joystick)
                {
                    Type = 2;

                    if (axis.JoystickAxis == JoystickAxis.LeftStickX
                        && axis.JoystickAxis == JoystickAxis.LeftStickY)
                        Axis = (int)axis.JoystickAxis - 1;
                    else
                        Axis = (int)axis.JoystickAxis + 1;
                }
            }
        }

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
    }
}
