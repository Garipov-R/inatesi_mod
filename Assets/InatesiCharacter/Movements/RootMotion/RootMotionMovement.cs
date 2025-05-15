using InatesiCharacter.SuperCharacter;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Movements
{
    [RequireComponent(typeof(CharacterController))]
    public class RootMotionMovement : CharacterMotionBase
    {
        public override void Awake()
        {
            base.Awake();
        }

        /*protected virtual void Update()
        {
            if (_UpdateViaMono == false)
                return;

            if (_UpdateMethod != UpdateMethod.Update)
                return;

            UpdateCharacter();
        }

        protected virtual void FixedUpdate()
        {
            if (_UpdateViaMono == false)
                return;

            UpdatePhysicsCharacter();

            if (_UpdateMethod != UpdateMethod.FixedUpdate)
                return;

            UpdateCharacter();
        }*/

        public override void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (_CharacterController != null) 
            {
                _CharacterController.enabled = false;
                base.SetPositionAndRotation(position, rotation);
                //_AnimatorMonitor.Animator.bodyPosition = position;
                _AnimatorMonitor.Animator.rootRotation = rotation;
                _CharacterController.enabled = true;
            }
        }

        public override void UpdateCharacter()
        {
            base.UpdateCharacter();

            _OnGrounded = true;

            //UpdateAnimator();
            //Move(new Vector2(0, 1f));

            RotateUpdate();
        }

        public override void RotateUpdate()
        {
            /*Vector3 movement = new Vector3(_InputDirection.x, 0, _InputDirection.y);
            Quaternion target_rot = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rot, 500 * Time.deltaTime);


            return;*/

            var targetAngle = _MovementType.GetRotation(_InputDirection.x, _InputDirection.y);
            var newRotation =Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 3f);
            var angleDiff = Quaternion.Angle(transform.rotation, newRotation); // Rotation.Distance is unsigned
            //moveRotationSpeed = (angleDiff) / Time.deltaTime;
            transform.rotation = newRotation;
        }
    }
}