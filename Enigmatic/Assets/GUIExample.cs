using UnityEditor;
using UnityEngine;

public class GUIExample : MonoBehaviour 
{
    [SerializeField] private GUIStyle m_Style;

    private void OnGUI()
    {
        m_Style = new GUIStyle(); //EditorStyles.toolbarButton;
        m_Style.normal.textColor = GUI.backgroundColor;
        
        //m_Style.normal.scaledBackgrounds = EditorStyles.toolbarButton.normal.scaledBackgrounds;
        //m_Style.hover.scaledBackgrounds = EditorStyles.toolbarButton.hover.scaledBackgrounds;
        //m_Style.normal = EditorStyles.toolbarButton.normal;

        GUILayout.Button("Example", m_Style);
    }
} 
