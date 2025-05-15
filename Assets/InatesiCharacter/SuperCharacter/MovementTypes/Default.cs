using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter.MovementTypes
{
    public class Default : MovementType
    {
        public override bool FirstPersonPerspective => throw new NotImplementedException();

        public override float GetDeltaYawRotation(float characterHorizontalMovement, float characterForwardMovement, float cameraHorizontalMovement, float cameraVerticalMovement)
        {
            return 0;
            //throw new NotImplementedException();
        }

        public override Quaternion GetRotation(float characterHorizontalMovement = 0, float characterForwardMovement = 0)
        {
            var lookRotation = Quaternion.identity;
            var result = Vector3.zero;

            if (characterHorizontalMovement != 0 || characterForwardMovement != 0)
            {
                lookRotation = Quaternion.LookRotation(
                    new Vector3(characterHorizontalMovement, 0, characterForwardMovement).normalized,
                    _CharacterMotion.Up);

                result = Vector3.Scale(lookRotation.eulerAngles, Vector3.up);

                return Quaternion.Euler(result);
            }

            return _Transform.rotation;
        }

        public override Vector2 GetInputVector(Vector2 inputVector)
        {
            var clampValue = Mathf.Max(Mathf.Abs(inputVector.x), Mathf.Max(Mathf.Abs(inputVector.y), 1));
            inputVector.y = Mathf.Clamp(inputVector.magnitude, -clampValue, clampValue);
            inputVector.x = 0;
            return inputVector;
        }
    }
}
