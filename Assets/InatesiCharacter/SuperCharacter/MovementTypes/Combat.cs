using System.Collections;
using UnityEngine;
using Inatesi.Utilities;


namespace InatesiCharacter.SuperCharacter.MovementTypes
{
    public class Combat : MovementType
    {
        public override bool FirstPersonPerspective => throw new System.NotImplementedException();

        public override float GetDeltaYawRotation(
            float characterHorizontalMovement,
            float characterForwardMovement,
            float cameraHorizontalMovement,
            float cameraVerticalMovement
            )
        {
            if (_LookSource == null)
                return 0;

            var lookRotation = Quaternion.LookRotation(
                _LookSource.LookDirection(true),
                _CharacterMotion.Up
            );

            _YawDelta = MathUtility.ClampInnerAngle(
                MathUtility.InverseTransformQuaternion(
                    _Transform.rotation,
                    lookRotation).eulerAngles.y
                );

            return _YawDelta;
        }

        public override Quaternion GetRotation(float characterHorizontalMovement = 0, float characterForwardMovement = 0)
        {
            if (_LookSource == null)
                return Quaternion.identity;

            var lookRotation = Quaternion.LookRotation(
                _LookSource.LookDirection(true),
                _CharacterMotion.Up
            );

            var result = Vector3.Scale(lookRotation.eulerAngles, Vector3.up );

            return Quaternion.Euler(result);
        }

        public override Vector2 GetInputVector(Vector2 inputVector)
        {
            return inputVector;
        }
    }
}