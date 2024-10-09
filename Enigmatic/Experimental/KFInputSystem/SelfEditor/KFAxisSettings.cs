using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    [SerializebleObject]
    internal class KFAxisSettings
    {
        [SerializebleProperty] public string Axis;
        [SerializebleProperty] public string PosetiveButton;
        [SerializebleProperty] public string NegativeButton;

        [SerializebleProperty] public string Type;

        public KFAxisSettings()
        {
            Axis = "None";

            PosetiveButton = "None";
            NegativeButton = "None";

            Type = "Value";
        }
    }
}
