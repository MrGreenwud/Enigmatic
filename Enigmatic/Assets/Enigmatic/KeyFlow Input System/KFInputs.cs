using System;
using UnityEngine;

namespace KFInputSystem
{
    public class KFInput<T>
    {
        [SerializeField] private string m_Tag = "New input";
        
        public event Action Action;

        public string Tag => m_Tag;
        public T Value { get; protected set; }
        
        public virtual void OnAction() => Action?.Invoke();
    }

    public class KFInputButton : KFInput<bool>
    {
        [SerializeField] private KeyCode m_KeyCode;

        public KeyCode KeyCode => m_KeyCode;

        public KFInputButton(KeyCode keyCode)
        {
            m_KeyCode = keyCode;
        }

        public override void OnAction()
        {
            if (Value)
                base.OnAction();
        }
    }

    [Serializable]
    public class KFInputVec2 : KFInput<Vector2>
    {
        [SerializeField] private Axis m_AxisX;
        [SerializeField] private Axis m_AxisY;

        public Axis AxisX => m_AxisX;
        public Axis AxisY => m_AxisY;

        public override void OnAction()
        {
            float x = Input.GetAxis($"{Tag} X");
            float y = Input.GetAxis($"{Tag} Y");

            Value = new Vector2(x, y);

            if (Value.x != 0 && Value.y != 0)
                base.OnAction();
        }
    }

    [Serializable]
    public class KFInputAxis : KFInput<float>
    {
        [SerializeField] private string m_Axis;

        public KFInputAxis(string axis)
        {
            m_Axis = axis;
        }

        public override void OnAction()
        {
            Value = Input.GetAxis(m_Axis);

            if (Value != 0)
                base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonDown : KFInputButton
    {
        public KFInputButtonDown(KeyCode keyCode) : base(keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKeyDown(KeyCode);
            base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonUp : KFInputButton
    {
        public KFInputButtonUp(KeyCode keyCode) : base(keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKeyUp(KeyCode);
            base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonPress : KFInputButton
    {
        public KFInputButtonPress(KeyCode keyCode) : base(keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKey(KeyCode);
            base.OnAction();
        }
    }
}
