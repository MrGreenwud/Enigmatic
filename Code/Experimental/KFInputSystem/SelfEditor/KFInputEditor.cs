using System;
using System.Collections.Generic;

using UnityEngine;

using Enigmatic.Core;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal class KFInputEditor
    {
        private InputMaps m_InputMaps = new InputMaps();

        public InputMapSettings SelectionMap { get; private set; }
        public KFInput SelectedInput { get; private set; }

        public Action OnSelectedElement;

        public Action OnAddedElement;

        public Action OnAddedMap;
        public Action OnAddedInput;

        public Action OnRemovedMap;
        public Action OnRemovedInput;

        public Action OnRemovedElement;

        public Action OnChangeMapName;
        public Action OnChangeInputTag;

        public KFInputEditor()
        {
            OnAddedMap += OnValidateMapName;
            OnRemovedMap += OnValidateMapName;
            OnChangeMapName += OnValidateMapName;

            OnAddedInput += OnValidateInputTag;
            OnRemovedInput += OnValidateInputTag;
            OnRemovedElement += OnValidateInputTag;
        }

        public void SaveMap()
        {
            InputMapResuresManager.SaveEditorMaps(m_InputMaps);
            InputMapResuresManager.SeveMap(m_InputMaps);
        }

        public void LoadMap()
        {
            m_InputMaps = InputMapResuresManager.LoadEditorMaps();

            if (m_InputMaps == null)
                m_InputMaps = new InputMaps();
        }

        public void ApplyMap()
        {
            List<InputAxis> axies = InputAxisGenerator.GenerateAxis(m_InputMaps);
            
            UnityInputManager.Clear();

            foreach (InputAxis axis in axies)
                UnityInputManager.AddAxis(axis);
        }

        public void DrawInputMaps()
        {
            m_InputMaps.ForEach(OnInputMapAction);
        }

        public void DrawInputs()
        {
            if (SelectionMap == null)
                return;

            SelectionMap.ForEach(OnInput);
        }

        public void AddInputMap()
        {
            InputMapSettings inputMap = new InputMapSettings();
            inputMap.Name = $"New InputMap {m_InputMaps.Count + 1}";
            m_InputMaps.Add(inputMap);

            OnAddedElement?.Invoke();
        }

        public void RemoveSelectionInputMap()
        {
            if (SelectionMap == null)
                return;

            int selectionMapIndex = m_InputMaps.IndexOf(SelectionMap);
            m_InputMaps.Remove(SelectionMap);

            if (m_InputMaps.Count > 0)
            {
                if (m_InputMaps.Count - 1 >= selectionMapIndex)
                    SelectionMap = m_InputMaps.Get(selectionMapIndex);
                else
                    SelectionMap = m_InputMaps.Get(selectionMapIndex - 1);
            }
            else
            {
                SelectionMap = null;
            }

            OnRemovedElement?.Invoke();
        }

        public void AddInput()
        {
            if (SelectionMap == null)
                return;

            KFInput input = new KFInput();
            input.Tag = "New Input";
            SelectionMap.AddInput(input);

            OnAddedElement?.Invoke();
        }

        public void RemoveSelectionInput()
        {
            if (SelectedInput == null)
                return;

            int selectionInputIndex = SelectionMap.IndexOf(SelectedInput);
            SelectionMap.Remove(SelectedInput);

            if (SelectionMap.Count > 0)
            {
                if (SelectionMap.Count - 1 >= selectionInputIndex)
                    SelectedInput = SelectionMap.GetInput(selectionInputIndex);
                else
                    SelectedInput = SelectionMap.GetInput(selectionInputIndex - 1);
            }
            else
            {
                SelectedInput = null;
            }

            OnRemovedElement?.Invoke();
        }

        private void OnInputMapAction(InputMapSettings inputMap)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;
            Vector2 size = new Vector2(width, 20);

            GUIStyle style = EnigmaticGUIUtility.GetHasSelected(SelectionMap == inputMap,
                EnigmaticStyles.columBackground, EnigmaticStyles.columBackgroundSelected);

            if (EnigmaticGUILayout.Button(inputMap.Name, size, style))
            {
                SelectionMap = inputMap;
                SelectedInput = null;

                OnSelectedElement?.Invoke();
            }
        }

        private void OnInput(KFInput kFInputSettings)
        {
            float width = EnigmaticGUILayout.GetActiveGrup().GetFreeArea().x;
            Vector2 size = new Vector2(width, 20);

            GUIStyle style = EnigmaticGUIUtility.GetHasSelected(SelectedInput == kFInputSettings,
                EnigmaticStyles.columBackground, EnigmaticStyles.columBackgroundSelected);

            if (EnigmaticGUILayout.Button(kFInputSettings.Tag, size, style))
            {
                SelectedInput = kFInputSettings;
                OnSelectedElement?.Invoke();
            }
        }

        private void OnValidateMapName()
        {

        }

        private void OnValidateInputTag()
        {

        }

    }
}
