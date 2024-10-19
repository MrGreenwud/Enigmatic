using System;
using UnityEngine;

using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem
{
    [SerializebleObject]
    public abstract class KFInput<T>
    {
        [SerializebleProperty] protected string p_Tag;
        
        public string Tag => p_Tag;
        public T Value { get; protected set; }

        public event Action<T> OnAction;

        public void Construct(string tag)
        {
            p_Tag = tag;
        }

        public abstract T OnInput();

        protected void CallAction() => OnAction?.Invoke(Value);
    }

    public class KFInputButton : KFInput<bool>
    {
        public override bool OnInput() { return false; }
    }

    [SerializebleObject]
    public class KFInputButtonDown : KFInputButton
    {
        public override bool OnInput() 
        {
            bool result = Input.GetButtonDown(Tag);
            Value = result;

            if (Value == true)
                CallAction();

            return result;
        }
    }

    [SerializebleObject]
    public class KFInputButtonUp : KFInputButton
    {
        public override bool OnInput()
        {
            bool result = Input.GetButtonUp(Tag);
            Value = result;

            if (Value == true)
                CallAction();

            return result;
        }
    }

    [SerializebleObject]
    public class KFInputButtonHold : KFInputButton
    {
        public override bool OnInput()
        {
            bool result = Input.GetButton(Tag);
            Value = result;

            if(Value == true)
                CallAction();

            return result;
        }
    }

    [SerializebleObject]
    public class KFInputAxis : KFInput<float>
    {
        public override float OnInput()
        {
            float result = Input.GetAxis(Tag);
            Value = result;

            if(Value != 0)
                CallAction();

            return result;
        }
    }

    [SerializebleObject]
    public class KFInputAxis2D : KFInput<Vector2>
    {
        public override Vector2 OnInput()
        {
            float x = Input.GetAxis($"{Tag} X");
            float y = Input.GetAxis($"{Tag} Y");

            Vector2 result = new Vector2(x, y);
            Value = result;

            if(Value != Vector2.zero)
                CallAction();

            return result;
        }
    }
}