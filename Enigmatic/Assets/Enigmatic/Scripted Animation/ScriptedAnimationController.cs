using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class ScriptedAnimationController : MonoBehaviour
{
    private List<ScriptedAnimation> m_Animations = new List<ScriptedAnimation>();
    private Queue<Animation> m_Destryeds = new Queue<Animation>();

    private static ProfilerMarker s_ScriptedAnimationUpdate = new ProfilerMarker(ProfilerCategory.Scripts, "Scripted Animation Update");
    private static ProfilerMarker s_AnimationDestroy = new ProfilerMarker(ProfilerCategory.Scripts, "Scripted Animation Destroy");

    public Animation FlowMove(Transform movbleObject, Vector3 targetPosition, float speed, float accuracy = 0.1f)
    {
        FlowMover flowMover = new FlowMover(movbleObject, targetPosition, speed, accuracy);
        m_Animations.Add(flowMover);
        flowMover.AddDestredLisener(DestroyAnimation);
        return flowMover;
    }

    public void Update()
    {
        s_ScriptedAnimationUpdate.Begin();

        foreach (ScriptedAnimation animation in m_Animations)
            animation.Tick();

        s_ScriptedAnimationUpdate.End();
    }

    private void LateUpdate()
    {
        s_AnimationDestroy.Begin();

        while (m_Destryeds.Count > 0)
        {
            ScriptedAnimation scriptedAnimation = m_Destryeds.Dequeue() as ScriptedAnimation;
            m_Animations.Remove(scriptedAnimation);
        }

        s_AnimationDestroy.End();
    }

    public Animation GetAnimationBySender(object sender)
    {
        foreach (ScriptedAnimation animation in m_Animations)
            if (animation.Sender == sender)
                return animation;

        return null;
    }

    public Animation[] GetAnimationsBySender(object sender)
    {
        List<Animation> result = new List<Animation>();

        foreach (ScriptedAnimation animation in m_Animations)
            if (animation.Sender == sender)
                result.Add(animation);

        return result.ToArray();
    }

    private void DestroyAnimation(Animation animation)
    {
        m_Destryeds.Enqueue(animation);
        animation.RemoveDestroyedLisener(DestroyAnimation);
    }
}

public class Animation
{
    public event Action OnStarted;
    public event Action OnFinished;
    public event Action<Animation> OnDestroyed;

    public object Sender { get; private set; }

    protected bool m_isFinish { get; private set; }
    protected bool m_isWaitDestroy { get; private set; }

    protected bool m_isSafely { get; private set; }

    public Animation Safely(object sender)
    {
        Sender = sender;
        m_isSafely = true;

        return this;
    }

    #region Follow event methods

    public Animation AddStartedLisener(Action action)
    {
        OnStarted += action;
        return this;
    }
    
    public Animation AddFinishedLisener(Action action) 
    { 
        OnFinished += action; 
        return this; 
    }

    public Animation AddDestredLisener(Action<Animation> action)
    {
        OnDestroyed += action;
        return this;
    }

    #endregion

    #region Unfollow event methods

    public Animation RemoveStartedLisener(Action action)
    {
        OnStarted -= action;
        return this;
    }

    public Animation RemoveFinishedLisener(Action action)
    {
        OnFinished -= action;
        return this;
    }

    public Animation RemoveDestroyedLisener(Action<Animation> action)
    {
        OnDestroyed -= action;
        return this;
    }

    #endregion

    public void Kill() => Destroyed();

    protected void Started() => OnStarted?.Invoke();

    protected void Finished()
    {
        OnFinished?.Invoke();
        m_isFinish = true;
    }

    protected void Destroyed()
    {
        OnDestroyed?.Invoke(this);
        m_isWaitDestroy = true;
    }

    protected bool CheckSelfly()
    {
        if (m_isSafely == true)
        {
            if (Sender.ToString() == "null")
            {
                Kill();
                return false;
            }
        }

        return true;
    }
}

public class ScriptedAnimation : Animation
{
    public virtual void Tick() 
    {
        if (m_isFinish == true || m_isWaitDestroy == true)
            return;

        if (CheckSelfly() == false)
            return;
    }
}

public class FlowMover : ScriptedAnimation
{
    private Transform m_MovbleObject;
    private Vector3 m_TargetPosition;
    private float m_Speed;
    private float m_Accuracy;

    public FlowMover(Transform movbleObject, Vector3 targetPosition, float speed, float accuracy)
    {
        m_MovbleObject = movbleObject;
        m_TargetPosition = targetPosition;
        m_Speed = speed;
        m_Accuracy = accuracy;

        Started();
    }

    public override void Tick()
    {
        base.Tick();

        float distence = Vector3.Distance(m_MovbleObject.position, m_TargetPosition);
        Vector3 moveDiraction = (m_TargetPosition - m_MovbleObject.position).normalized;

        m_MovbleObject.position += moveDiraction * m_Speed * Time.deltaTime;

        if(distence <= m_Accuracy)
        {
            m_MovbleObject.position = m_TargetPosition;

            Finished();
            Destroyed();
        }
    }
}
