using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    [SerializebleObject]
    internal class KFInputSettings
    {
        [SerializebleProperty] public string Device;

        [SerializebleProperty] public float Gravity;
        [SerializebleProperty] public float Dead;
        [SerializebleProperty] public float Sensitivity;

        [SerializebleProperty] public string Button;
        [SerializebleProperty] public string ControlGUID;

        [SerializebleProperty] private KFAxisSettings m_HorizontalAxis;
        [SerializebleProperty] private KFAxisSettings m_VerticalAxis;

        public KFAxisSettings HorizontalAxis => m_HorizontalAxis;
        public KFAxisSettings VerticalAxis => m_VerticalAxis;

        public KFInputSettings()
        {
            Gravity = 3;
            Dead = 0.0001f;
            Sensitivity = 3;

            Button = "None";

            m_HorizontalAxis = new KFAxisSettings();
            m_VerticalAxis = new KFAxisSettings();
        }
    }
}
