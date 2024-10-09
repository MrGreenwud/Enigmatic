using System;
using System.Collections.Generic;

using UnityEngine;

using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem
{
    [SerializebleObject]
    internal class InputMap
    {
        [SerializebleProperty] private Dictionary<string, KFInputButtonDown> m_InputButtonDown = new Dictionary<string, KFInputButtonDown>();
        [SerializebleProperty] private Dictionary<string, KFInputButtonUp> m_InputButtonUp = new Dictionary<string, KFInputButtonUp>();
        [SerializebleProperty] private Dictionary<string, KFInputButtonHold> m_InputButtonHold = new Dictionary<string, KFInputButtonHold>();
        [SerializebleProperty] private Dictionary<string, KFInputAxis> m_InputAxis = new Dictionary<string, KFInputAxis>();
        [SerializebleProperty] private Dictionary<string, KFInputAxis2D> m_InputAxis2D = new Dictionary<string, KFInputAxis2D>();

        public void Update()
        {
            foreach (KFInputButton input in m_InputButtonDown.Values)
                input.OnInput();

            foreach (KFInputButton input in m_InputButtonUp.Values)
                input.OnInput();

            foreach (KFInputButton input in m_InputButtonHold.Values)
                input.OnInput();

            foreach (KFInputAxis input in m_InputAxis.Values)
                input.OnInput();

            foreach (KFInputAxis2D input in m_InputAxis2D.Values)
                input.OnInput();
        }

        public void AddInput(object input, string tag)
        {
            if (input.GetType().IsAssignableFrom(typeof(KFInput<>)))
                throw new Exception(" ");

            if (input is KFInputButtonDown kFInputButtonDown)
            {
                if (ContainsTag(tag))
                    throw new Exception();

                m_InputButtonDown.Add(tag, kFInputButtonDown);
            }
            else if (input is KFInputButtonUp kFInputButtonUp)
            {
                if (ContainsTag(tag))
                    throw new Exception();

                m_InputButtonUp.Add(tag, kFInputButtonUp);
            }
            else if (input is KFInputButtonHold kFInputButtonHold)
            {
                if (ContainsTag(tag))
                    throw new Exception();

                m_InputButtonHold.Add(tag, kFInputButtonHold);
            }
            else if (input is KFInputAxis kFInputAxis)
            {
                if (ContainsTag(tag))
                    throw new Exception();

                m_InputAxis.Add(tag, kFInputAxis);
            }
            else if (input is KFInputAxis2D kFInputAxis2D)
            {
                if (ContainsTag(tag))
                    throw new Exception();

                m_InputAxis2D.Add(tag, kFInputAxis2D);
            }
        }

        public bool ContainsTag(string tag)
        {
            return m_InputButtonDown.ContainsKey(tag) || m_InputButtonUp.ContainsKey(tag)
                || m_InputButtonHold.ContainsKey(tag) || m_InputAxis.ContainsKey(tag)
                || m_InputAxis2D.ContainsKey(tag);
        }

        public bool TryGetButtonDown(string inputTag, out bool value)
        {
            value = false;

            if (m_InputButtonDown.ContainsKey(inputTag))
            {
                value = m_InputButtonDown[inputTag].Value;
                return true;
            }

            return false;
        }

        public bool TryGetButtonUp(string inputTag, out bool value)
        {
            value = false;

            if (m_InputButtonUp.ContainsKey(inputTag))
            {
                value = m_InputButtonUp[inputTag].Value;
                return true;
            }

            return false;
        }

        public bool TryGetButtonHold(string inputTag, out bool value)
        {
            value = false;

            if (m_InputButtonHold.ContainsKey(inputTag))
            {
                value = m_InputButtonHold[inputTag].Value;
                return true;
            }

            return false;
        }

        public bool TryGetAxis(string inputTag, out float value)
        {
            value = 0;

            if (m_InputAxis.ContainsKey(inputTag))
            {
                value = m_InputAxis[inputTag].Value;
                return true;
            }

            return false;
        }

        public bool TryGetAxis2D(string inputTag, out Vector2 value)
        {
            value = Vector2.zero;

            if (m_InputAxis2D.ContainsKey(inputTag))
            {
                value = m_InputAxis2D[inputTag].Value;
                return true;
            }

            return false;
        }

        public bool TrySubscribeButton(string inputTag, Action<bool> action)
        {
            if (m_InputButtonDown.ContainsKey(inputTag))
            {
                m_InputButtonDown[inputTag].OnAction += action;
                return true;
            }
            else if (m_InputButtonUp.ContainsKey(inputTag))
            {
                m_InputButtonUp[inputTag].OnAction += action;
                return true;
            }
            else if (m_InputButtonHold.ContainsKey(inputTag))
            {
                m_InputButtonHold[inputTag].OnAction += action;
                return true;
            }

            return false;
        }

        public bool TrySubscribeAxis(string inputTag, Action<float> action)
        {
            if (m_InputAxis.ContainsKey(inputTag))
            {
                m_InputAxis[inputTag].OnAction += action;
                return true;
            }

            return false;
        }

        public bool TrySubscribeAxis2D(string inputTag, Action<Vector2> action)
        {
            if (m_InputAxis2D.ContainsKey(inputTag))
            {
                m_InputAxis2D[inputTag].OnAction += action;
                return true;
            }

            return false;
        }
    }
}
