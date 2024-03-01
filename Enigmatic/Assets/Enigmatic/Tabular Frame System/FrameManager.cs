using System.Collections.Generic;
using UnityEngine;

namespace TabularFrameSystem
{
    public class FrameManager : MonoBehaviour
    {
        [SerializeField] private List<Frame> m_Frames;
        public Frame CurrentViweFrame { get; private set; }

        private void ChangeShowedFrame(Frame newFrame)
        {
            #if DEBUG
            {
                if (newFrame == null)
                    throw new System.ArgumentNullException(nameof(Frame));

                if (m_Frames.Contains(newFrame) == false)
                    throw new System.InvalidOperationException();
            }
            #endif

            if (CurrentViweFrame == newFrame)
            {
                Debug.Log("This frame is showed!");
                return;
            }

            if(CurrentViweFrame != null)
                if(CurrentViweFrame.IsStatic == true)
                    CurrentViweFrame.Hide();

            CurrentViweFrame = newFrame;
            CurrentViweFrame.Show();
        }

        private void ChangeShowedFrame(FrameTag tag)
        {
            if(TryGetFrameWithTag(tag, out Frame frame))
            {
                ChangeShowedFrame(frame);
                return;
            }    

            throw new System.InvalidOperationException();
        }

        public Frame GetFrameWithTag(FrameTag tag)
        {
            foreach (Frame frame in m_Frames) 
                if(frame.Tag == tag)
                    return frame;

            return null;
        }

        public bool TryGetFrameWithTag(FrameTag tag, out Frame frame)
        {
            frame = GetFrameWithTag(tag);
            return frame != null;
        }
    }
}