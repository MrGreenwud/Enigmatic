using System;
using UnityEngine;

namespace TabularFrameSystem
{
    public class VisualElement : MonoBehaviour
    {
        public event Action OnShowing;
        public event Action OnHiding;

        public virtual void Show() 
        {
            gameObject.SetActive(true);
            OnShowing?.Invoke(); 
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnHiding?.Invoke(); 
        }
    }
}
