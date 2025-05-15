using System.Collections;
using UnityEngine;
using KinematicCharacterController;
using InatesiCharacter.Testing.InatesiArch.Character;
using InatesiCharacter.SuperCharacter;
using System;
using Input = Inatesi.Inputs.Input;
using Fragsurf.Movement;
using InatesiCharacter.Testing.Character.InteractionSystem;


namespace InatesiCharacter.Movements.SourceEngine
{
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [RequireComponent(typeof(CharacterWorldInteractionSystem))]
    public class SourceEngineMovement3 : CharacterMotionBase, ICharacterController
    {
        [Header("stuff")]
        [SerializeField] private KinematicCharacterMotor _Motor;
        [SerializeField] private bool _AutoBhop = true;
        [SerializeField] private bool _lerpRotate = true;

        #region Source Movement fields

        // source movement fields
        [Header("Source fields")]
        [SerializeField] private float _rigidbodyPushForce = 0.5f;
        [SerializeField] private float _SpeedRotate = 10f;
        [SerializeField] private float _StepOffset = .2f;

        // move data
        [SerializeField] private Vector3 _Gravity = new Vector3(0, -11f, 0);
        [SerializeField] private float _slopeLimit = 45f;

        private Vector3 _wishVelocity;
        private GameObject _groundObject;
        private Vector3 _groundNormal;
        public bool TurnAnim { get; set; }
        private Quaternion _turnRotation;
        public Quaternion desired_rotation;

        // move data
        private Vector3 _origin;
        private Vector3 _velocity;
        private Vector3 _externalVelocity;
        private bool _wishExternalVelocity;

        public override Vector3 Velocity 
        { 
            get => _velocity; 
            set 
            {
                _Motor.BaseVelocity = value; 
                _velocity = value;
                _Velocity = value;

                if (value == Vector3.zero)
                {
                    _wishExternalVelocity = false;
                    _externalVelocity = value;
                }
            } 
        }
        public override float Height 
        { 
            get 
            {
                return base.Height; 
            } 
            set 
            {
                _Motor.SetCapsuleDimensions(_Motor.Capsule.radius, value, value / 2);
                base.Height = value;
            }
        }

        public KinematicCharacterMotor Motor { get => _Motor; set => _Motor = value; }


        #endregion




        public override void Awake()
        {
            base.Awake();

            _Motor.CharacterController = this;
            _Height = _Motor.Capsule.height;
            _DefaultHeight = _Motor.Capsule.height;
            _radius = _Motor.Capsule.radius;
            _Collider = _Motor.Capsule;

            _Motor = GetComponent<KinematicCharacterMotor>();
            SetHeight();

            _Gravity.y = Mathf.Abs(_MoveConfig.Gravity) * -1;
        }

        public override void SetHeight()
        {
            if (_Motor == null) return;

            if (_Renderer != null)
            {
                var skinnedMeshRender = _Renderer as SkinnedMeshRenderer;
                if (skinnedMeshRender != null)
                {
                    _Height = _DefaultHeight = Mathf.Abs(transform.position.y - skinnedMeshRender.bounds.max.y);
                    _Motor.SetCapsuleDimensions(_Motor.Capsule.radius, _Height, _Height / 2);
                }
            }
        }

        public override void Move(Vector2 input)
        {
            base.Move(input);
        }

        public override void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            _Motor.SetPositionAndRotation(position, rotation);
            _Motor.ForceUnground();
        }

        public override void SetRotation(Quaternion rotation)
        {
            _Motor.SetRotation(rotation);
        }

        public override void AddForce(Vector3 force)
        {
            if (Mathf.Abs( force.magnitude )<= 0) return;

            _Motor.ForceUnground();
            Force(force);
        }

        public override void UpdateAnimator()
        {
            base.UpdateAnimator();

            if (_AnimatorMonitor.Animator == null) return;


            _AnimatorMonitor.WithVelocity(_velocity);
            _AnimatorMonitor.WithWishVelocity(_wishVelocity);

            _AnimatorMonitor.RotaionSpeed = _MoveRotationSpeed;
            _AnimatorMonitor.IsGrounded = _velocity.y > _Gravity.y  ? _OnGrounded : false;
            _AnimatorMonitor.MoveSpeed = Velocity.magnitude;

            _AnimatorMonitor.Animator.SetBool("turn", TurnAnim);

            _AnimatorMonitor.SetParameters();
        }

        public void BuildWishVelocity()
        {
            var result = new Vector3();
            Vector3 direction = new Vector3(_InputVector.x, _InputVector3.y, _InputVector.y);
            //direction = new Vector3(_InputDirection.x, _InputVector3.y, _InputDirection.y);

            Quaternion rotationT = transform.rotation;
            result = rotationT * direction;




            _wishVelocity = result;
            _wishVelocity.y = _isNoclip == true || _underwater == true ? _wishVelocity.y : 0f;

            if (_wishVelocity.magnitude != 0) _wishVelocity = _wishVelocity.normalized;
            _wishVelocity *= _isNoclip == false ? _SpeedMove : _MoveConfig.NoClipSpeed;
        }



        public void Force(Vector3 amount)
        {
            _wishExternalVelocity = true;
            //ClearGround();
            //_velocity += amount; 
            _externalVelocity += amount;
        }



        



        #region controller
        public void AfterCharacterUpdate(float deltaTime)
        {
            //CheckWater();



            if (Vector3.Angle(_Motor.GroundingStatus.OuterGroundNormal, _Motor.CharacterUp) < _Motor.MaxStableSlopeAngle)
            {
                _OnGrounded = _Motor.GroundingStatus.IsStableOnGround;
            }
            else
            {
                _OnGrounded = false;
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            //CheckWater();

        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return !_isNoclip;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {

        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                //OnLanded();
                _OnLanded?.Invoke();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                //OnLeaveStableGround();
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_IsInputDisabled ) return;

            float moveRotationSpeed = 0;
            var targetAngle = _MovementType.GetRotation(_InputDirection.x, InputDirection.y);

            var newRotation = _lerpRotate ? Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * _SpeedRotate) : targetAngle;
            var angleDiff = Quaternion.Angle(transform.rotation, newRotation); // Rotation.Distance is unsigned
            moveRotationSpeed = (angleDiff) / Time.deltaTime;
            currentRotation = newRotation;

            _MoveRotationSpeed = moveRotationSpeed;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            BuildWishVelocity();

            if (_isNoclip)
            {
                currentVelocity = currentVelocity.ApplyFriction(3, 8);
                currentVelocity = currentVelocity.WithAcceleration(_wishVelocity, _MoveConfig.Acceleration * deltaTime);
            }
            else
            {
                if (_Motor.GroundingStatus.IsStableOnGround)
                {
                    float currentVelocityMagnitude = currentVelocity.magnitude;
                    Vector3 directionGround = _Motor.GetDirectionTangentToSurface(
                        currentVelocity,
                        _Motor.GroundingStatus.GroundNormal
                    );
                    currentVelocity = directionGround * (currentVelocityMagnitude);

                    currentVelocity = currentVelocity.ApplyFriction(_MoveConfig.Friction, _MoveConfig.StopSpeed);
                    currentVelocity = currentVelocity.WithAcceleration(_wishVelocity, _MoveConfig.Acceleration * deltaTime);
                }
                else
                {
                    if (_wishVelocity.magnitude > 0)
                    {
                        if (_wishVelocity.magnitude > _MoveConfig.MaxSpeed)
                        {
                            _wishVelocity = _wishVelocity * (_MoveConfig.MaxSpeed / _wishVelocity.magnitude);
                        }

                        Vector3 addedVelocity = _wishVelocity * deltaTime;
                        // Prevent air-climbing sloped walls
                        if (_Motor.GroundingStatus.FoundAnyGround)
                        {
                            if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(
                                    Vector3.Cross(
                                        _Motor.CharacterUp,
                                        _Motor.GroundingStatus.GroundNormal
                                    ), _Motor.CharacterUp).normalized;
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                            }
                        }

                        // Apply added velocity
                        //currentVelocity += addedVelocity;
                    }

                    currentVelocity -= _Gravity * deltaTime * -1 * (_underwater ? .11f : _GravityFactor);

                    if (_underwater)
                        currentVelocity = currentVelocity.ApplyFriction(0, _MoveConfig.AirStopSpeed);
                    else
                    {
                        currentVelocity = currentVelocity.ApplyFriction(0, _MoveConfig.AirStopSpeed);
                        //currentVelocity = currentVelocity.ApplyFriction(_MoveConfig.Friction, _MoveConfig.AirStopSpeed);
                    }

                    //currentVelocity = SurfPhysics.WithAirAcceleration(Vector3.ClampMagnitude(_wishVelocity, _airClampLength), currentVelocity);
                    currentVelocity = currentVelocity.WithAcceleration(Vector3.ClampMagnitude(_wishVelocity, _MoveConfig.AirClampLength), _MoveConfig.AirAcceleration * deltaTime);
                    //currentVelocity = currentVelocity.WithAirAcceleration(Vector3.ClampMagnitude(_wishVelocity, _MoveConfig.AirClampLength));

                    //currentVelocity = currentVelocity.WithAirAcceleration(Vector3.ClampMagnitude(_wishVelocity, _MoveConfig.AirClampLength));

                }

                //Debug.Log(currentVelocity);
            }
            
            



            //SurfPhysics.ApplyFriction(ref _externalVelocity, _Friction, _stopSpeed);

            if (_wishExternalVelocity == true)
            {
                _Motor.ForceUnground();
                currentVelocity += _externalVelocity;
                _wishExternalVelocity = false;
                _externalVelocity = Vector3.zero;
            }


            _velocity = currentVelocity;

            if (Vector3.Angle(_Motor.GroundingStatus.OuterGroundNormal, _Motor.CharacterUp) < _Motor.MaxStableSlopeAngle)
            {
                _OnGrounded = _Motor.GroundingStatus.IsStableOnGround || _Motor.LastGroundingStatus.IsStableOnGround;
            }
            else
            {
                _OnGrounded = false;                    
            }
        }

        #endregion
    }
}