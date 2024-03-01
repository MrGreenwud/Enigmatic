using System;
using UnityEngine;

namespace KFInputSystem
{
    [Serializable]
    public class Axis
    {
        [SerializeField] private Device m_Device;
        [SerializeField] private InputType m_InputType;

        [Space(10)]

        [SerializeField] private float m_Gravity = 3;
        [SerializeField] private float m_Dead = 0.001f;
        [SerializeField] private float m_Sensitivity = 3;

        [Space(10)]
        [Header("Button")]

        [Header("Keyboard")]
        [SerializeField] private KeyboardKeyCode m_KeyboardPosetiveButton;
        [SerializeField] private KeyboardKeyCode m_KeyboardNegativeButton;

        [Space(5)]

        [Header("Joystick")]
        [SerializeField] private JoystickKeyCode m_JoystickPosetiveButton;
        [SerializeField] private JoystickKeyCode m_JoystickNegativeButton;

        [Space(10)]
        [Header("Axis")]

        [Header("Mouse")]
        [SerializeField] private MouseAxis m_MouseAxis;

        [Space(5)]

        [Header("Joystick")]
        [SerializeField] private JoystickAxis m_JoystickAxis;

        public Device Device => m_Device;
        public InputType InputType => m_InputType;

        public float Gravity => m_Gravity;
        public float Dead => m_Dead;
        public float Sensitivity => m_Sensitivity;

        public KeyboardKeyCode KeyboardPosetiveButton => m_KeyboardPosetiveButton;
        public KeyboardKeyCode KeyboardNegativeButton => m_KeyboardNegativeButton;

        public JoystickKeyCode JoystickPosetiveButton => m_JoystickPosetiveButton;
        public JoystickKeyCode JoystickNegativeButton => m_JoystickNegativeButton;

        public MouseAxis MouseAxis => m_MouseAxis;

        public JoystickAxis JoystickAxis => m_JoystickAxis;
    }
}
