using System.Collections.Generic;
using UnityEngine;

namespace TabularFrameSystem
{
    public class Frame : VisualElement
    {
        [SerializeField] private FrameTag m_Tag;
        [SerializeField] private bool m_IsStatic;
        [SerializeField] private List<Window> m_Windows = new List<Window>();

        public FrameTag Tag => m_Tag;
        public bool IsStatic => m_IsStatic;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public Window GetWindowWithTag(WindowTag tag)
        {
            foreach (Window window in m_Windows)
                if(window.Tag == tag)
                    return window;

            return null;
        }

        public bool TryGetWindowWithTag(WindowTag tag, out Window window)
        {
            window = GetWindowWithTag(tag);
            return window != null;
        }
    }
}
