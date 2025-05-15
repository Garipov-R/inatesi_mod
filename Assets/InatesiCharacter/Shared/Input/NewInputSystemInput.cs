using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Shared.Input
{
    public class NewInputSystemInput : InputBase
    {
        protected InatesiInputSystem.Input m_Input;

        public InatesiInputSystem.Input Input { get => m_Input; private set => m_Input = value; }


        public override void Initialize(PlayerInput playerInput)
        {
            base.Initialize(playerInput);

            Input = new InatesiInputSystem.Input();
        }

        public override float GetAxis(string name)
        {
            UnityEngine.InputSystem.InputAction inputAction = Input.FindAction(name);

            if (inputAction == null)
                return 0;

            return inputAction.ReadValue<float>();
        }

        public override Vector2 GetVector(string name)
        {
            UnityEngine.InputSystem.InputAction inputAction = Input.FindAction(name);

            if (inputAction == null)
                return Vector2.zero;

            return inputAction.ReadValue<Vector2>();
        }

        public override bool GetButton(string name, ButtonAction buttonAction)
        {
            UnityEngine.InputSystem.InputAction inputAction = Input.FindAction(name);

            if (inputAction == null)
                return false;

            /*UnityEngine.Debug.Log(
                $"WasPerformedThisFrame {inputAction.WasPerformedThisFrame()} \n" +
                $"WasPressedThisFrame {inputAction.WasPressedThisFrame()} \n" +
                $"WasReleasedThisFrame {inputAction.WasReleasedThisFrame()} \n" +
                $"IsPressed {inputAction.IsPressed()}"
            );*/

            /*_Game.TextDebug.StaticLog(
                $"WasPerformedThisFrame {inputAction.WasPerformedThisFrame()} \n" +
                $"WasPressedThisFrame {inputAction.WasPressedThisFrame()} \n" +
                $"WasReleasedThisFrame {inputAction.WasReleasedThisFrame()} \n" +
                $"IsPressed {inputAction.IsPressed()} \n" +
                $"Phase {inputAction.phase}");*/

            try
            {
                switch (buttonAction)
                {
                    case ButtonAction.GetButton:
                        //_Game.TextDebug.StaticLog($"Button");
                        return inputAction.IsPressed();
                    case ButtonAction.GetButtonDown:
                        //_Game.TextDebug.StaticLog($"Button down");
                        return inputAction.WasPressedThisFrame();
                    case ButtonAction.GetButtonUp:
                        //_Game.TextDebug.StaticLog($"Button up");
                        return inputAction.WasReleasedThisFrame();
                }
            }
            catch(System.Exception /**/)
            {
                UnityEngine.Debug.LogError("Input sucks");
            }
/*            switch (buttonAction)
            {
                case ButtonAction.GetButton:
                    return inputAction.phase is UnityEngine.InputSystem.InputActionPhase.Started;
                case ButtonAction.GetButtonDown:
                    return inputAction.phase is UnityEngine.InputSystem.InputActionPhase.Performed;
                case ButtonAction.GetButtonUp:
                    return inputAction.phase is UnityEngine.InputSystem.InputActionPhase.Canceled;
            }*/

            return false;
        }

        public override float GetRawAxis(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Debug()
        {
            //_Game.TextDebug.StaticLog();
        }
    }
}