using Enigmatic.Experemental.SearchedWindowUtility;

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

        public InputAxis(EditorKFInput input)
        {
            if (SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Button")
                throw new System.InvalidOperationException();

            if (input.Device == "Keyboard")
                Type = 0;
            else if(input.Device == "Mouse")
                Type = 1;
            else
                Type = 2;

            if (input.Device == "Keyboard")
            {
                PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(input.AxisX.PosetiveButton);
                NegativeButton = UnityInputManager.ConvertToUnityInputReadable(input.AxisX.NegativeButton);
            }
            else if (input.Device == "Mouse")
            {
                if (input.AxisX.Value == "MouseX")
                    Axis = 0;
                else
                    Axis = 1;
            }
            else
            {
                Axis = JosticAxisParse(input.AxisX.Value);
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

        public InputAxis(string tag, EditorKFInput input, EditorKFAxis axis) 
        {
            Tag = tag;

            Gravity = input.Gravity;
            Dead = input.Dead;
            Sensitivity = input.Sensitivity;

            if (input.Device == "Keyboard")
                Type = 0;
            else if (input.Device == "Mouse")
                Type = 1;
            else
                Type = 2;

            if (input.Device == "Keyboard")
            {
                PosetiveButton = UnityInputManager.ConvertToUnityInputReadable(axis.PosetiveButton);
                NegativeButton = UnityInputManager.ConvertToUnityInputReadable(axis.NegativeButton);
            }
            else if (input.Device == "Mouse")
            {
                Axis = MosusAxisParse(axis.Value);
            }
            else
            {
                Axis = JosticAxisParse(axis.Value);
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
                    axis = 2;
                    break;
                case "RightStickY":
                    axis = 3;
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
    }
}
