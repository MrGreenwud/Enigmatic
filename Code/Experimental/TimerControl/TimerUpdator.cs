using UnityEngine;

namespace Enigmatic.Experimental.TimerControl
{
    public class TimerUpdator : MonoBehaviour
    {
        private void Update()
        {
            TimerManager.Update();
        }

        private void LateUpdate()
        {
            TimerManager.LateUpdate();
        }
    }
}
