using UnityEngine;

namespace TabularFrameSystem
{
    public class Element : VisualElement
    {
        [SerializeField] private ElementTag m_Tag;

        public ElementTag Tag => m_Tag;
    }
}
