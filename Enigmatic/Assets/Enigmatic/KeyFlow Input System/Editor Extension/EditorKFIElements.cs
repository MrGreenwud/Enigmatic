using System.Collections.Generic;
using System;

namespace Enigmatic.KFInputSystem.Editor
{
    public class EditorKFInputSettings
    {
        public string Device { get; set; }

        public float Gravity { get; set; }
        public float Dead { get; set; }
        public float Sensitivity { get; set; }

        public EditorKFAxisSettings AxisXSettings { get; set; }
        public EditorKFAxisSettings AxisYSettings { get; set; }

        public string Button { get; set; }

        public EditorKFInputSettings(string device, EditorKFAxisSettings axisXSettings, EditorKFAxisSettings axisYSettings, string button, Guid guid)
        {
            Device = device;

            Gravity = 3;
            Dead = 0.001f;
            Sensitivity = 3;

            AxisXSettings = axisXSettings;
            AxisYSettings = axisYSettings;

            Button = button;
        }

        public EditorKFInputSettings()
        {
            Device = "None";

            Gravity = 3;
            Dead = 0.001f;
            Sensitivity = 3;

            AxisXSettings = new EditorKFAxisSettings();
            AxisYSettings = new EditorKFAxisSettings();

            Button = "None";
        }
    }

    public class EditorKFAxisSettings
    {
        public string Value;
        public string PosetiveButton;
        public string NegativeButton;

        public EditorKFAxisSettings(string value, string posetiveButton, string negativeButton)
        {
            Value = value;
            PosetiveButton = posetiveButton;
            NegativeButton = negativeButton;
        }

        public EditorKFAxisSettings()
        {
            Value = "None";
            PosetiveButton = "None";
            NegativeButton = "None";
        }
    }

    public class KFInputGrup
    {
        public string Name;

        private List<EditorKFInput> m_KFInputs = new List<EditorKFInput>();

        public EditorKFInput[] KFInputs => m_KFInputs.ToArray();

        public KFInputGrup(string name)
        {
            Name = name;
        }

        public void Add()
        {
            EditorKFInputSettings newInputKeyboardAndMouse = new EditorKFInputSettings();
            EditorKFInputSettings newInputJoystic = new EditorKFInputSettings();

            m_KFInputs.Add(new EditorKFInput(newInputKeyboardAndMouse, newInputJoystic));
        }

        public void Add(EditorKFInput kFInput)
        {
            if (kFInput == null)
                throw new ArgumentNullException(nameof(kFInput));

            m_KFInputs.Add(kFInput);
        }

        public void Remove(EditorKFInput kFInput) => m_KFInputs.Remove(kFInput);

        public void Clear() => m_KFInputs.Clear();
    }

    public class EditorKFInput
    {
        public string Tag;
        public string Type;

        private EditorKFInputSettings m_KeyboardAndMouseInput;
        private EditorKFInputSettings m_Joystic;

        public EditorKFInput(EditorKFInputSettings keyboardAndMouseInput, EditorKFInputSettings joystic)
        {
            Tag = "New Input";
            Type = "None";

            m_KeyboardAndMouseInput = keyboardAndMouseInput;
            m_Joystic = joystic;

            joystic.Device = "Joystick";
        }

        public EditorKFInputSettings GetInputSettings(Device device)
        {
            if (device == Device.Keyboard_and_Mouse)
                return m_KeyboardAndMouseInput;
            else
                return m_Joystic;
        }
    }
}
