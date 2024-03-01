using UnityEngine;

namespace KFInputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputMap m_InputMap;

        private void Update()
        {
            m_InputMap.Update();
        }

        public KFInputButton GetInputButtonDown(InputTag tag) => m_InputMap.GetInputButtonDown(tag);
        public KFInputButton GetInputButtonUp(InputTag tag) => m_InputMap.GetInputButtonUp(tag);
        public KFInputButton GetInputButtonPress(InputTag tag) => m_InputMap.GetInputButtonPress(tag);

        public KFInputVec2 GetInputVec2(InputTag tag) => m_InputMap.GetInputVec2(tag);
        public KFInputAxis GetInputAxis(InputTag tag) => m_InputMap.GetInputAxis(tag);
    }
}
