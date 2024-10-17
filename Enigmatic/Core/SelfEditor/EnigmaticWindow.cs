using UnityEditor;

using Enigmatic.Core;
using Enigmatic.Core.Editor;

namespace Enigmatic.Experimental.Window.Editor
{
    public class EnigmaticWindow : EditorWindow
    {
        protected bool IsInit { get; private set; }

        private void OnEnable()
        {
            IsInit = false;
        }

        private void OnDisable()
        {
            OnClose();
        }

        private void OnGUI()
        {
            if (IsInit == false)
            {
                OnOpen();
                IsInit = true;
            }

            EditorInput.UpdateInput();
            EnigmaticGUILayout.Reset();

            Draw();
        }

        protected virtual void OnOpen() { }

        protected virtual void OnClose() { }

        protected virtual void Draw() { }
    }
}