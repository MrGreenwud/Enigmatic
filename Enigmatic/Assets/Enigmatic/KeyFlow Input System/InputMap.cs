using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using KFInputSystem.Utility;

namespace KFInputSystem
{
    [CreateAssetMenu(fileName = "KFInput Map", menuName = "KFInputMap", order = 0)]
    public class InputMap : ScriptableObject
    {
        [SerializeField] private List<KFInputVec2> m_InputsVec2 = new List<KFInputVec2>();
        [SerializeField] private List<KFInputAxis> m_InputsAxis = new List<KFInputAxis>();

        [SerializeField] private List<KFInputButtonDown> m_InputButtonDown = new List<KFInputButtonDown>();
        [SerializeField] private List<KFInputButtonUp> m_InputButtonUp = new List<KFInputButtonUp>();
        [SerializeField] private List<KFInputButtonPress> m_InputButtonPress = new List<KFInputButtonPress>();

        [Space(10)]

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

        public void GenerateTags()
        {
            string path = $"{Application.dataPath}/Tags/InputTags.cs";
            
            CodeGen.CodeGenerator.AddEnum("InputTag", GetTags().ToArray(), nameof(KFInputSystem));
            CodeGen.CodeGenerator.GenerateCode("InputTag", path);
        }

        public void ApplyAxisToUnity()
        {
            UnityInputManager.Clear();

            foreach(KFInputVec2 lFInputVec2 in m_InputsVec2)
            {
                Axis axisX = lFInputVec2.AxisX;
                Axis axisY = lFInputVec2.AxisY;

                InputAxis inputAxisX = new InputAxis($"{lFInputVec2.Tag} X", axisX);
                InputAxis inputAxisY = new InputAxis($"{lFInputVec2.Tag} Y", axisY);

                UnityInputManager.AddAxis(inputAxisX);
                UnityInputManager.AddAxis(inputAxisY);
            }
        }

        public void ApplyAll()
        {
            GenerateTags();
            ApplyAxisToUnity();
        }

        public List<string> GetTags()
        {
            List<string> tags = new List<string>();

            foreach (KFInputVec2 lFInputVec2 in m_InputsVec2)
                tags.Add(lFInputVec2.Tag);

            foreach (KFInputAxis lFInputAxis2 in m_InputsAxis)
                tags.Add(lFInputAxis2.Tag);

            foreach (KFInputButtonDown lFInputButtonDown in m_InputButtonDown)
                tags.Add(lFInputButtonDown.Tag);

            foreach (KFInputButtonUp lFInputButtonUp in m_InputButtonUp)
                tags.Add(lFInputButtonUp.Tag);

            foreach (KFInputButtonPress lFInputButtonPress in m_InputButtonPress)
                tags.Add(lFInputButtonPress.Tag);

            return tags;
        }
    }
}
