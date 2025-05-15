using InatesiCharacter.SuperCharacter.MovementTypes;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class CharacterMotion : CharacterMotionBase
    {
        [Header("Settings")]
        [SerializeField] protected Vector3 _CheckGroundDirection = new Vector3(0,1,0);
        [SerializeField] protected float _SpeedRotate = 30f;
        [SerializeField] protected bool _LerpRotate = true;


        private bool _MovingParameter;
        private Vector3 _ExternalForce;
        private Quaternion _Torque;
        private Vector3 _MoveDirection;

        public Vector3 ExternalForce { get => _ExternalForce; set => _ExternalForce = value; }


        public override void Awake()
        {
            base.Awake();

            _LookSource = _LookSourceInstance ? _LookSourceInstance : _LookSource; 

            _MovementType = new Adventure(); //new Combat();

            _RaycastLayer = ~(0 | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("FPC"));
        }

        protected void Update()
        {
            if (_UpdateViaMono == false)
                return;

            if (_UpdateMethod != UpdateMethod.Update)
                return;

            UpdateCharacter();
        }

        protected void FixedUpdate()
        {
            if (_UpdateViaMono == false)
                return;

            UpdatePhysicsCharacter();

            if (_UpdateMethod != UpdateMethod.FixedUpdate)
                return;

            UpdateCharacter();
        }

        /// <summary>
        /// update physics character
        /// </summary>
        public override void UpdatePhysicsCharacter()
        {
            if (_IsInputDisabled)
                return;

            

            RotateUpdate();
            PhysicsProccess();
            UpdateExternalForce();
        }

        /// <summary>
        /// update
        /// </summary>
        public override void UpdateCharacter()
        {
            if (_IsInputDisabled)
                return;

            _MoveDirection = (_Velocity * Time.deltaTime * 60);
            _CharacterController.Move(_MoveDirection);

            if (_InputVector.sqrMagnitude > .001f)
            {
                _MovingParameter = true;
            }
            else
            {
                _MovingParameter = false;
            }

            UpdateAnimator();
        }

        public override void RotateUpdate()
        {
            float yaw = _MovementType.GetDeltaYawRotation(_InputDirection.x, _InputDirection.y, 0, 0);
            _DeltaRotation.Set(0, yaw, 0);

            if (_LerpRotate == true)
            {
                // rotate
                Quaternion targetRotation;
                targetRotation = Quaternion.Slerp(
                    transform.rotation,
                    transform.rotation * Quaternion.Euler(_DeltaRotation),
                    _SpeedRotate * (Time.fixedDeltaTime)
                );

                targetRotation = Quaternion.Lerp(
                    transform.rotation,
                    transform.rotation * Quaternion.Euler(_DeltaRotation),
                    _SpeedRotate * (Time.fixedDeltaTime)
                );

                transform.rotation = targetRotation;
                //_Torque = targetRotation;
            }
            else
            {
                Quaternion targetRotation;
                targetRotation = transform.rotation * Quaternion.Euler(_DeltaRotation);

                transform.rotation = targetRotation;
            }
        }

        private void PhysicsProccess()
        {
            Vector3 direction = new Vector3(_InputVector.x, _InputVector3.y, _InputVector.y);
            //Vector3 direction = new Vector3(_InputVector.x, 0, _InputVector.y);

            Quaternion rotationT = transform.rotation;
            Vector3 gravity = _GravityDirection * _GravityMagnitude;
            Vector3 gForce = gravity * _Mass * Time.fixedDeltaTime * Time.fixedDeltaTime;
            gForce = CheckGround() ? Vector3.zero : gForce ;
            _Velocity += gForce;
            _Velocity += rotationT * direction * _SpeedMove * Time.fixedDeltaTime; 
            Vector3 externalForce = _ExternalForce * Time.fixedDeltaTime; //Vector3.zero; 
            _Velocity += externalForce;
            _Velocity *= Mathf.Clamp01(1f - _Drag * Time.fixedDeltaTime);
        }

        public override void Move(Vector2 input)
        {
            _InputDirection = input;
            _InputVector = _MovementType.GetInputVector(input);
        }

        public override void SetMovementType()
        {
            if (_MovementType is Adventure)
            {
                _MovementType = new Combat();
            }
            else
            {
                _MovementType = new Adventure();
            }

            _MovementType.Initialize(this);
        }

        public override void UpdateAnimator()
        {
            _AnimatorMonitor.SetHorizontalMovementParameter(_InputVector.x, _TimeScale);
            _AnimatorMonitor.SetForwardMovementParameter(_InputVector.y, _TimeScale);
            _AnimatorMonitor.SetMovingParameter(_MovingParameter);
        }

        public override void AddForce(Vector3 force)
        {
            //_Velocity += force;
            _ExternalForce += force;
        }

        public void UpdateExternalForce()
        {
            _ExternalForce *= Mathf.Clamp01(1f - _Drag * Time.fixedDeltaTime);
        }

        public override bool CheckGround()
        {
            var radius = 0.5f;

            var cast = Physics.SphereCast(
                transform.position + _Up * ((radius * 2 + radius / 2) - 0.02f), // 0.02f -> offset
                radius, 
                -_Up, 
                out RaycastHit raycastHit,
                _CheckGroundDirection.y, 
                _RaycastLayer, 
                QueryTriggerInteraction.Ignore
            );

            if (cast == true)
            {

            }

            return cast;
        }


        #region UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            var radius = 0.5f;

            var cast = Physics.SphereCast(
                transform.position + _Up * ((radius * 2 + radius / 2) - 0.02f), // 0.02f -> offset
                radius,
                _Up * -1,
                out RaycastHit raycastHit,
                1,
                _RaycastLayer,
                QueryTriggerInteraction.Ignore
            );

            Gizmos.color = cast ? Color.red : Color.white;

            Gizmos.DrawWireSphere(transform.position + _Up * ((radius * 2 + radius / 2) - 0.02f) + _Up * -1, radius);
        }

        #endregion
    }
}