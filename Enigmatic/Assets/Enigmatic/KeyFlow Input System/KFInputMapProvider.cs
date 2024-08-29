using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Enigmatic.Experemental.SearchedWindowUtility;

namespace Enigmatic.KFInputSystem
{
    public class KFInputMapProvider : ScriptableObject
    {
        public InputGrup GrupName;

        [SerializeField] private List<KFInputVec2> m_InputsVec2 = new List<KFInputVec2>();
        [SerializeField] private List<KFInputAxis> m_InputsAxis = new List<KFInputAxis>();

        [SerializeField] private List<KFInputButtonDown> m_InputButtonDown = new List<KFInputButtonDown>();
        [SerializeField] private List<KFInputButtonUp> m_InputButtonUp = new List<KFInputButtonUp>();
        [SerializeField] private List<KFInputButtonPress> m_InputButtonPress = new List<KFInputButtonPress>();

        private static ProfilerMarker s_InputUpdater = new ProfilerMarker("Input Updater");

        public void Update()
        {
            s_InputUpdater.Begin();

            foreach (KFInputVec2 lFInputVec2 in m_InputsVec2)
                lFInputVec2.OnAction();

            foreach (KFInputAxis lFInputAxis in m_InputsAxis)
                lFInputAxis.OnAction();

            foreach (KFInputButtonDown lFInputButton in m_InputButtonDown)
                lFInputButton.OnAction();

            foreach (KFInputButtonUp lFInputButton in m_InputButtonUp)
                lFInputButton.OnAction();

            foreach (KFInputButtonPress lFInputButton in m_InputButtonPress)
                lFInputButton.OnAction();

            s_InputUpdater.End();
        }

        public KFInputButtonDown GetInputButtonDown(InputTag tag)
        {
            return GetInputButton(tag, m_InputButtonDown) as KFInputButtonDown;
        }

#if UNITY_EDITOR
        public void AddInput(string tag, string type)
        {
            if (SearchedTreeUtility.DeCompileTree(type, 0) == "Button")
            {
                switch (SearchedTreeUtility.DeCompileTree(type, 1))
                {
                    case "Press":
                        KFInputButtonPress kFInputButtonPress = new KFInputButtonPress(tag);
                        m_InputButtonPress.Add(kFInputButtonPress);
                        break;
                    case "Up":
                        KFInputButtonUp kFInputButtonUp = new KFInputButtonUp(tag);
                        m_InputButtonUp.Add(kFInputButtonUp);
                        break;
                    case "Down":
                        KFInputButtonDown kFInputButtonDown = new KFInputButtonDown(tag);
                        m_InputButtonDown.Add(kFInputButtonDown);
                        break;
                }
            }
            else
            {
                if (SearchedTreeUtility.DeCompileTree(type, 1) == "Vector2")
                    m_InputsVec2.Add(new KFInputVec2(tag));
                else
                    m_InputsAxis.Add(new KFInputAxis(tag));
            }
        }
#endif

        public KFInputButtonUp GetInputButtonUp(InputTag tag)
        {
            return GetInputButton(tag, m_InputButtonUp) as KFInputButtonUp;
        }

        public KFInputButtonPress GetInputButtonPress(InputTag tag)
        {
            return GetInputButton(tag, m_InputButtonPress) as KFInputButtonPress;
        }

        public KFInputVec2 GetInputVec2(InputTag tag)
        {
            foreach (KFInputVec2 lFInputVec2 in m_InputsVec2)
                if (lFInputVec2.Tag == tag.ToString())
                    return lFInputVec2;

            return null;
        }

        public KFInputAxis GetInputAxis(InputTag tag)
        {
            foreach (KFInputAxis lFInputAxis in m_InputsAxis)
                if (lFInputAxis.Tag == tag.ToString())
                    return lFInputAxis;

            return null;
        }

        private KFInputButton GetInputButton<T>(InputTag tag, List<T> lFInputButtons) where T : KFInputButton
        {
            foreach (KFInputButton lFInputButton in lFInputButtons)
                if (lFInputButton.Tag == tag.ToString())
                    return lFInputButton;

            return null;
        }
    }
}
