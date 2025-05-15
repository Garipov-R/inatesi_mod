using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Shared.Input
{
    public class UnityInput : PlayerInput
    {
        [SerializeField] private bool _Debug;

        private InputBase m_InputBase;

        public InputBase InputBase { get => m_InputBase; }


        protected override void Awake()
        {
            base.Awake();

            m_InputBase = new NewInputSystemInput();
            m_InputBase.Initialize(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_InputBase is NewInputSystemInput)
                (m_InputBase as NewInputSystemInput).Input.Enable();

            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_InputBase is NewInputSystemInput)
                (m_InputBase as NewInputSystemInput).Input.Disable();
        }

        protected override void Update()
        {
            if (_Debug == true)
            {
                if (m_InputBase != null)
                {
                    m_InputBase.Debug();
                }
            }
        }


        protected override bool GetButtonInternal(string name)
        {
            return m_InputBase.GetButton(name, InputBase.ButtonAction.GetButton);
        }

        protected override bool GetButtonDownInternal(string name)
        {
            return m_InputBase.GetButton(name, InputBase.ButtonAction.GetButtonDown);
        }

        protected override bool GetButtonUpInternal(string name)
        {
            return m_InputBase.GetButton(name, InputBase.ButtonAction.GetButtonUp);
        }

        protected override Vector2 GetVectorInternal(string name)
        {
            return m_InputBase.GetVector(name);
        }

        protected override float GetAxisRawInternal(string name)
        {
            return m_InputBase.GetAxis(name);
        }
    }
}