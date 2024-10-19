using UnityEditor;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    internal static class UnityInputManager
    {
        public static void AddAxis(InputAxis axis)
        {
            SerializedObject inputManager = new SerializedObject(AssetDatabase
                .LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

            SerializedProperty axesProperty = inputManager.FindProperty("m_Axes");

            axesProperty.arraySize++;
            inputManager.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.Tag;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.PosetiveButton;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.NegativeButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = "";
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = "";
            GetChildProperty(axisProperty, "gravity").floatValue = axis.Gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.Dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.Sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = true;
            GetChildProperty(axisProperty, "type").intValue = axis.Type;
            GetChildProperty(axisProperty, "axis").intValue = axis.Axis;

            inputManager.ApplyModifiedProperties();
        }

        public static void Clear()
        {
            SerializedObject inputManager = new SerializedObject(AssetDatabase
                .LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

            SerializedProperty axesProperty = inputManager.FindProperty("m_Axes");
            axesProperty.ClearArray();
            inputManager.ApplyModifiedProperties();
        }

        public static SerializedProperty GetChildProperty(SerializedProperty parentProperty, string name)
        {
            SerializedProperty childProperty = parentProperty.Copy();
            childProperty.Next(true);

            do
            {
                if (childProperty.name == name) return childProperty;
            }
            while (childProperty.Next(false));
            return null;
        }

        public static string ConvertToUnityInputReadable(string keyCode)
        {
            string keyChecked = keyCode.ToUpper();
            string keyLoaded = keyCode.ToString();

            string key = "";

            for (int i = 0; i < keyChecked.Length; i++)
            {
                if (keyLoaded[i] == keyChecked[i] && i > 0)
                    key += " ";

                key += keyLoaded[i];
            }

            return key.ToLower();
        }
    }
}
