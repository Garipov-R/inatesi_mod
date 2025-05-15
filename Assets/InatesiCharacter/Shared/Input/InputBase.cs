using System.Collections;
using UnityEngine;


namespace InatesiCharacter.Shared.Input
{
    public abstract class InputBase
    {
        public enum ButtonAction { GetButton, GetButtonDown, GetButtonUp }

        protected PlayerInput m_PlayerInput;


        public virtual void Initialize(PlayerInput playerInput)
        {
            m_PlayerInput = playerInput;
        }

        public abstract bool GetButton(string name, ButtonAction buttonAction);

        public abstract float GetAxis(string name);

        public abstract float GetRawAxis(string name);

        public abstract Vector2 GetVector(string name);

        public virtual void Debug() { }
    }
}