using System;
using System.Collections.Generic;

using UnityEngine;

namespace Enigmatic.Core.Editor
{
    public class ReorderableList<T>
    {
        private List<T> m_List;

        private int m_SelectedElement = -1 ;
        private int m_TargetElement = -1;
        private int m_LastSelectinElement = -1 ;

        private Vector2 m_ElementSize;
        private Vector2 m_SelectedOffset;

        public event Action<int> OnDrawElement;

        public int LastSelectinElement => m_LastSelectinElement;

        public ReorderableList(List<T> list)
        {
            m_List = list;
        }

        public void Draw(float width)
        {
            EditorInput.UpdateInput();

            EnigmaticGUILayout.BeginVertical(EnigmaticGUILayout.Padding(0), EnigmaticGUILayout.Width(width), 
                EnigmaticGUILayout.ExpandHeight(true));
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    if (m_ElementSize != Vector2.zero)
                    {
                        m_ElementSize.x = width;

                        if (m_SelectedElement == i || (m_LastSelectinElement == i && m_SelectedElement == -1))
                            EnigmaticGUILayout.Rect(m_ElementSize, m_SelectedOffset, EnigmaticStyles.DarkThemeBlueElementSelected);
                        else if(m_TargetElement == i)
                            EnigmaticGUILayout.Rect(m_ElementSize, m_SelectedOffset, EnigmaticStyles.DarkedBackgroundColor);
                    }

                    EnigmaticGUILayout.BeginHorizontal(EnigmaticGUILayout.Width(width),
                        EnigmaticGUILayout.Height(22), EnigmaticGUILayout.ExpandHeight(true), EnigmaticGUILayout.Padding(0));
                    {
                        EnigmaticGUILayout.Image(new Vector2(16, 16), EnigmaticStyles.elementMoveIcon);

                        OnDrawElement?.Invoke(i);
                    }
                    EnigmaticGUILayout.EndHorizontal();

                    Rect rect = EnigmaticGUILayout.GetLastGUIRect();
                    rect = EnigmaticGUIClip.UnClip(rect);

                    if (m_ElementSize == Vector2.zero)
                    {
                        m_ElementSize = new Vector2(width, rect.size.y + 8);
                        m_SelectedOffset = new Vector2(0, -4 / 2);
                    }

                    if(rect.Contains(EditorInput.GetMousePosition()) 
                        && EditorInput.GetMouseButtonDown(0) && m_SelectedElement == -1)
                    {
                        m_SelectedElement = i;
                        GUI.FocusControl(null);
                    }
                    else if(rect.Contains(EditorInput.GetMousePosition()))
                    {
                        m_TargetElement = i;
                    }

                    if (EditorInput.Current.type == EventType.MouseUp 
                        && EditorInput.Current.button == 0 
                        && m_SelectedElement > -1 && m_TargetElement > -1)
                    {
                        GUI.FocusControl(null);
                        MoveElements();

                        m_LastSelectinElement = m_SelectedElement;

                        m_SelectedElement = -1;
                        m_TargetElement = -1;
                    }

                    if(m_SelectedElement > -1)
                        EnigmaticGUIUtility.Repaint();
                }
            }
            EnigmaticGUILayout.EndVertical();
        }

        private void MoveElements()
        {
            T tempList = m_List[m_SelectedElement];

            m_List[m_SelectedElement] = m_List[m_TargetElement];
            m_List[m_TargetElement] = tempList;

            m_SelectedElement = m_TargetElement;
            m_TargetElement = -1;

            EnigmaticGUIUtility.Repaint();
        }
    }
}
