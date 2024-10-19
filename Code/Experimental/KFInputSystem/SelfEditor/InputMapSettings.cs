using System;
using System.Collections.Generic;

using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    [SerializebleObject]
    internal class InputMapSettings
    {
        [SerializebleProperty] public string Name;

        [SerializebleProperty] private List<KFInput> kFInputs = new List<KFInput>();

        public int Count => kFInputs.Count;

        public KFInput GetInput(int index)
        {
            return kFInputs[index];
        }

        public void AddInput(KFInput kFInput)
        {
            if (kFInputs.Contains(kFInput))
                return;

            kFInputs.Add(kFInput);
        }

        public void Remove(KFInput kFInput)
        {
            if (kFInputs.Contains(kFInput) == false)
                return;

            kFInputs.Remove(kFInput);
        }

        public void ForEach(Action<KFInput> action)
        {
            kFInputs.ForEach(action);
        }

        public int IndexOf(KFInput kFInput)
        {
            return kFInputs.IndexOf(kFInput);
        }
    }
}
