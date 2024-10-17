using System.Collections.Generic;

using Enigmatic.Experimental.SearchedWindowUtility;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal static class InputAxisGenerator
    {
        public static List<InputAxis> GenerateAxis(InputMaps inputMaps)
        {
            List<InputAxis> result = new List<InputAxis>(inputMaps.Count);
            inputMaps.ForEach((x) => result.AddRange(GenerateAxis(x)));

            return result;
        }

        public static List<InputAxis> GenerateAxis(InputMapSettings inputMapSettings)
        {
            List<InputAxis> result = new List<InputAxis>(inputMapSettings.Count * 2);

            inputMapSettings.ForEach((x) =>
            {
                if (x != null)
                    result.AddRange(GenerateAxis(x, inputMapSettings.Name));
            });

            return result;
        }

        public static List<InputAxis> GenerateAxis(KFInput input, string mapName)
        {
            List<InputAxis> result = new List<InputAxis>(4);

            if (input.Type == "None")
                return null;

            string inputName = InputUtility.GetInputName(input.Tag, mapName);
            string type = SearchedTreeUtility.DeCompileTree(input.Type, 1);

            if (type == "Vector2")
            {
                result.Add(new InputAxis($"{inputName} X", input.Type, input.GetInputSettings(Device.KeyboardAndMouse), true));
                result.Add(new InputAxis($"{inputName} Y", input.Type, input.GetInputSettings(Device.KeyboardAndMouse), false));

                result.Add(new InputAxis($"{inputName} X", input.Type, input.GetInputSettings(Device.Joystick), true));
                result.Add(new InputAxis($"{inputName} Y", input.Type, input.GetInputSettings(Device.Joystick), false));
            }
            else
            {
                result.Add(new InputAxis($"{inputName}", input.Type, input.GetInputSettings(Device.KeyboardAndMouse), true));
                result.Add(new InputAxis($"{inputName}", input.Type, input.GetInputSettings(Device.Joystick), true));
            }

            return result;
        }
    }
}
