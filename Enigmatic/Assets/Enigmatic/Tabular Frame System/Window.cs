using System.Collections.Generic;
using UnityEngine;

namespace TabularFrameSystem
{
    public class Window : VisualElement
    {
        [SerializeField] private WindowTag m_Tag;
        [SerializeField] private List<Element> m_VisualElements;

        public WindowTag Tag => m_Tag;

        public Element GetElementWithTag(ElementTag tag)
        {
            foreach (Element element in m_VisualElements)
                if(element.Tag == tag)
                    return element;

            return null;
        }

        public bool TryGetElementWithTag(ElementTag tag, out Element element) 
        {
            element = GetElementWithTag(tag);
            return element != null;
        }
    }
}