using UnityEngine;

namespace Enigmatic.Experimental.KFInputSystem
{
    public class InputUpdator : MonoBehaviour
    {
        private void Update()
        {
            InputManager.Update();
        }
    }
}
