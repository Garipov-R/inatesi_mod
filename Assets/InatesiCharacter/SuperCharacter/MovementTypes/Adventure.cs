using Inatesi.Utilities;
using System.Collections;
using UnityEngine;


namespace InatesiCharacter.SuperCharacter.MovementTypes
{
    public class Adventure : MovementType
    {
        private bool _AimActive;
        private int _FaceUseTargetCount;

        public override bool FirstPersonPerspective => throw new System.NotImplementedException();

        public override float GetDeltaYawRotation(float characterHorizontalMovement, float characterForwardMovement, float cameraHorizontalMovement, float cameraVerticalMovement)
        {
            if (_LookSource == null)
                return _YawDelta = 0;

            if (characterHorizontalMovement != 0 || characterForwardMovement != 0)
            {
                var lookRotation = Quaternion.LookRotation(
                    _LookSource.Transform.rotation
                    * new Vector3(characterHorizontalMovement, 0, characterForwardMovement).normalized,
                    _CharacterMotion.Up);

                _YawDelta = MathUtility.ClampInnerAngle(
                    MathUtility.InverseTransformQuaternion(_Transform.rotation, lookRotation).eulerAngles.y
                );

                return _YawDelta;
            }
            return _YawDelta = 0;
        }

        public override Quaternion GetRotation(float characterHorizontalMovement = 0, float characterForwardMovement = 0)
        {
            if (_LookSource == null)
                return Quaternion.identity;

            var lookRotation = Quaternion.identity;
            var result = Vector3.zero;

            if (characterHorizontalMovement != 0 || characterForwardMovement != 0)
            {
                lookRotation = Quaternion.LookRotation(
                    _LookSource.Transform.rotation
                    * new Vector3(characterHorizontalMovement, 0, characterForwardMovement).normalized,
                    _CharacterMotion.Up);

                result = Vector3.Scale(lookRotation.eulerAngles, Vector3.up);

                return Quaternion.Euler(result);
            }

            lookRotation = Quaternion.LookRotation(
                _LookSource.LookDirection(true),
                _CharacterMotion.Up
            );

            result = Vector3.Scale(lookRotation.eulerAngles, Vector3.up);

            return _Transform.rotation;
        }

        public override Vector2 GetInputVector(Vector2 inputVector)
        {
            if (_AimActive || _FaceUseTargetCount > 0)
            {
                return inputVector;
            }

            var clampValue = Mathf.Max(Mathf.Abs(inputVector.x), Mathf.Max(Mathf.Abs(inputVector.y), 1));
            inputVector.y = Mathf.Clamp(inputVector.magnitude, -clampValue, clampValue);
            inputVector.x = 0;
            return inputVector;
        }
    }
}