﻿using System;
using UnityEngine;

namespace Enigmatic.KFInputSystem
{
    public class KFInput<T>
    {
        [SerializeField] private string m_Tag = "New input";
        
        public event Action Action;

        public string Tag => m_Tag;
        public T Value { get; protected set; }

        public KFInput(string tag) 
        {
            m_Tag = tag;
        }
        
        public virtual void OnAction() => Action?.Invoke();
    }

    public class KFInputButton : KFInput<bool>
    {
        [SerializeField] private KeyCode m_KeyCode;

        public KeyCode KeyCode => m_KeyCode;

        public KFInputButton(string tag, KeyCode keyCode) : base(tag)
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
        public KFInputVec2(string tag) : base(tag) { }

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
        public KFInputAxis(string tag) : base (tag) { }

        public override void OnAction()
        {
            Value = Input.GetAxis(Tag);

            if (Value != 0)
                base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonDown : KFInputButton
    {
        public KFInputButtonDown(string tag, KeyCode keyCode) : base(tag, keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKeyDown(KeyCode);
            base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonUp : KFInputButton
    {
        public KFInputButtonUp(string tag, KeyCode keyCode) : base(tag, keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKeyUp(KeyCode);
            base.OnAction();
        }
    }

    [Serializable]
    public class KFInputButtonPress : KFInputButton
    {
        public KFInputButtonPress(string tag, KeyCode keyCode) : base(tag, keyCode) { }

        public override void OnAction()
        {
            Value = Input.GetKey(KeyCode);
            base.OnAction();
        }
    }
}