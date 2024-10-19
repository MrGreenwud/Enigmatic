using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    [SerializebleObject]
    internal class KFInput
    {
        [SerializebleProperty] public string Tag;
        [SerializebleProperty] public string Type;

        [SerializebleProperty] private KFInputSettings m_KeyboardAndMouse;
        [SerializebleProperty] private KFInputSettings m_Joystick;
        [SerializebleProperty] private KFInputSettings m_Mobile;

        public KFInput()
        {
            Tag = "None";
            Type = "None";

            m_KeyboardAndMouse = new KFInputSettings();
            m_KeyboardAndMouse.Device = "None";

            m_Joystick = new KFInputSettings();
            m_Joystick.Device = "Joystick";

            m_Mobile = new KFInputSettings();
            m_Mobile.Device = "Mobile";
        }

        public KFInputSettings GetInputSettings(Device device)
        {
            if (device == Device.KeyboardAndMouse)
                return m_KeyboardAndMouse;
            else if (device == Device.Joystick)
                return m_Joystick;
            else if (device == Device.Mobile)
                return m_Mobile;

            return null;
        }
    }
}
