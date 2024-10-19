using System.Collections.Generic;

using Enigmatic.Experimental.KFInputSystem.Editor;
using Enigmatic.Experimental.SearchedWindowUtility;

namespace Enigmatic.Experimental.KFInputSystem
{
    internal static class InputMapGenerator
    {
        public static InputMapsProvider GenerateProvider(InputMaps inputMaps)
        {
            InputMapsProvider provider = new InputMapsProvider();
            inputMaps.ForEach((x) => provider.AddMap(x.Name, GenerateMap(x)));

            return provider;
        }

        public static List<InputMap> GenerateMaps(InputMaps inputMaps)
        {
            List<InputMap> result = new List<InputMap>(inputMaps.Count);
            inputMaps.ForEach((x) => result.Add(GenerateMap(x)));

            return result;
        }

        public static InputMap GenerateMap(InputMapSettings inputMapSettings)
        {
            InputMap map = new InputMap();

            inputMapSettings.ForEach((x) =>
            {
                object input = GenerateInput(x, inputMapSettings.Name);

                if (input != null)
                    map.AddInput(input, x.Tag);
            });

            return map;
        }

        public static object GenerateInput(KFInput input, string mapName)
        {
            object result;

            string inputName = InputUtility.GetInputName(input.Tag, mapName);

            if (input.Type == "None")
                return null;

            string inputType = SearchedTreeUtility.DeCompileTree(input.Type, 1);

            if (SearchedTreeUtility.DeCompileTree(input.Type, 0) == "Button")
            {
                if (inputType == "Hold")
                {
                    KFInputButtonHold KFInput = new KFInputButtonHold();
                    KFInput.Construct(inputName);
                    result = KFInput;
                }
                else if (inputType == "Down")
                {
                    KFInputButtonDown KFInput = new KFInputButtonDown();
                    KFInput.Construct(inputName);
                    result = KFInput;
                }
                else
                {
                    KFInputButtonUp KFInput = new KFInputButtonUp();
                    KFInput.Construct(inputName);
                    result = KFInput;
                }
            }
            else
            {
                if (inputType == "Vector2")
                {
                    KFInputAxis2D KFInput = new KFInputAxis2D();
                    KFInput.Construct(inputName);
                    result = KFInput;
                }
                else
                {
                    KFInputAxis KFInput = new KFInputAxis();
                    KFInput.Construct(inputName);
                    result = KFInput;
                }
            }

            return result;
        }
    }
}
