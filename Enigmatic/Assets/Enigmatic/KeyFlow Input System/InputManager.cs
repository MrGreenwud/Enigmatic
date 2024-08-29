using UnityEngine;

namespace Enigmatic.KFInputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private KFInputMapProvider[] m_InputMapProviders;

        private void Update()
        {
            string[] names = Input.GetJoystickNames();

            foreach (string name in names)
            {
                Debug.Log(name);
            }

            foreach (KFInputMapProvider provider in m_InputMapProviders)
                provider.Update();
        }

        public KFInputButton GetInputButtonDown(InputGrup grup, InputTag tag)  
        { 
            foreach(KFInputMapProvider provider in m_InputMapProviders)
            {
                if (provider.GrupName == grup)
                    return provider.GetInputButtonDown(tag);
            }

            throw new System.InvalidOperationException();
        }

        public KFInputButton GetInputButtonUp(InputGrup grup, InputTag tag)
        {
            foreach (KFInputMapProvider provider in m_InputMapProviders)
            {
                if (provider.GrupName == grup)
                    return provider.GetInputButtonUp(tag);
            }

            throw new System.InvalidOperationException();
        }

        public KFInputButton GetInputButtonPress(InputGrup grup, InputTag tag) 
        {
            foreach (KFInputMapProvider provider in m_InputMapProviders)
            {
                if (provider.GrupName == grup)
                    return provider.GetInputButtonPress(tag);
            }

            throw new System.InvalidOperationException();
        }

        public KFInputVec2 GetInputVec2(InputGrup grup, InputTag tag) 
        {
            foreach (KFInputMapProvider provider in m_InputMapProviders)
            {
                if (provider.GrupName == grup)
                    return provider.GetInputVec2(tag);
            }

            throw new System.InvalidOperationException();
        }

        public KFInputAxis GetInputAxis(InputGrup grup, InputTag tag) 
        {
            foreach (KFInputMapProvider provider in m_InputMapProviders)
            {
                if (provider.GrupName == grup)
                    return provider.GetInputAxis(tag);
            }

            throw new System.InvalidOperationException();
        }
    }
}
