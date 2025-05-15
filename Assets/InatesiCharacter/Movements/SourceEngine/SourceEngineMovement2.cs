using Fragsurf.Movement;
using Inatesi.Utilities;
using InatesiCharacter.Movements.SourceEngine.TraceUtility;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.SuperCharacter.MovementTypes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


namespace InatesiCharacter.Movements.SourceEngine
{
    public class SourceEngineMovement2 : CharacterMotionBase
    {
        [Header("Settings")]
        [SerializeField] protected Vector3 _colliderSize = new Vector3(1f, 2f, 1f);
        [SerializeField] protected Vector3 _colliderOffset = new Vector3(0, 1, 0);
        [SerializeField] protected bool _AutoBhop = true;
        [SerializeField] protected bool _lerpRotate = false;

        protected Collider _collider;
        protected GameObject _colliderObject;
        protected float _defaultHeight;
        protected Vector3 _prevPosition;
        protected bool _jumped;
        protected MovingPlatform _movingPlatform;

        #region Source Movement fields
        // source movement fields
        [Header("Source fields")]
        [SerializeField] private float _rigidbodyPushForce = 0.5f;
        [SerializeField] private float _stopSpeed = 2f;
        [SerializeField] private float _Speed = 10f;
        [SerializeField] private float _SpeedRotate = 10f;
        [SerializeField] private float _JumpForce = 20f;
        [SerializeField] private float _StepOffset = .2f;

        // move data
        [SerializeField] private float _Friction = .3f;
        [SerializeField] private float _Acceleration = 1.4f;
        [SerializeField] private Vector3 _Gravity = new Vector3(0, -9.81f, 0);
        [SerializeField] private float _slopeLimit = 45f;
        [SerializeField] private float _airClampLength = 45f;

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

        public override Vector3 Velocity { get => _velocity; set => _velocity = value; }
        public float JumpForce { get => _JumpForce; set => _JumpForce = value; }

        #endregion


        public override void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            _colliderObject = new GameObject("PlayerCollider");
            _colliderObject.layer = gameObject.layer;
            _colliderObject.transform.SetParent(transform);
            _colliderObject.transform.rotation = Quaternion.identity;
            _colliderObject.transform.localPosition = Vector3.zero;
            _colliderObject.transform.SetSiblingIndex(0);
            _collider = _colliderObject.AddComponent<BoxCollider>();


            var boxc = (BoxCollider)_collider;
            boxc.size = _colliderSize;
            //boxc.center = colliderOffset;
            boxc.transform.position += _colliderOffset;

            _defaultHeight = boxc.size.y;
            //_origin = transform.position;

            _movingPlatform = new MovingPlatform(transform);
        }



        public override void UpdateCharacter()
        {
            base.UpdateCharacter();

            UpdateInput();

            UpdateMovement();


            // ========= stuff ==============
            _colliderObject.transform.rotation = Quaternion.identity;

            Vector3 positionalMovement = transform.position - _prevPosition;
            transform.position = _prevPosition;
            _origin += positionalMovement;

            ProcessMovement();

            transform.position = _origin - _colliderOffset;
            _prevPosition = transform.position;

            _colliderObject.transform.rotation = Quaternion.identity;
            // ============= end stuff =================

            Velocity = _velocity;
        }

        public override void UpdatePhysicsCharacter()
        {
            base.UpdatePhysicsCharacter();

            BuildWishVelocity();
            RotateUpdate();
        }

        private void UpdateInput()
        {
            /*var bhop = _AutoBhop ? Inatesi.Inputs.Input.Down("Jump") : Inatesi.Inputs.Input.Pressed("Jump");
            if (bhop)
            {
                Jump();
            }*/

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                _velocity = Vector3.zero;
                _origin = new Vector3(20,20,20);
            }
        }

        private void UpdateMovement()
        {
            if (_OnGrounded == true && _jumped == true)
                _jumped = false;
        }

        public void Jump()
        {
            if (_OnGrounded == false)
                return;

            if (_jumped == true)
                return;

            SetGround(null);
            _jumped = true;
            Force(Vector3.up * _JumpForce);
        }

        public override void RotateUpdate()
        {
            float moveRotationSpeed = 0;
            var targetAngle = Quaternion.Euler( Vector3.Scale(LookSource.Transform.eulerAngles, new Vector3(0f, 1f, 0f)) );
            targetAngle =  _MovementType.GetRotation(_InputDirection.x, InputDirection.y);
                
            if (_MovementType is Adventure)
            {
                targetAngle = Quaternion.Euler(Vector3.Scale(LookSource.Transform.eulerAngles, new Vector3(0f, 1f, 0f)));
                var v = _velocity;
                v.y = 0;

                if (_wishVelocity.magnitude > 1.0f)
                {
                    //targetAngle = Quaternion.LookRotation(v, transform.up);
                    //targetAngle = Quaternion.Euler(v.normalized);
                    targetAngle = _MovementType.GetRotation(_InputDirection.x, InputDirection.y);
                }

                float rotateDifference = Quaternion.Angle(transform.rotation, targetAngle);
                if (rotateDifference >= 165 && _wishVelocity.magnitude > 1.0f && TurnAnim == false && _OnGrounded == true)
                {
                    Debug.Log(rotateDifference);
                    TurnAnim = true;
                    //transform.localRotation = targetAngle;
                    Vector3 anim_rotation = AnimatorMonitor.Animator.rootRotation.eulerAngles;
                    desired_rotation = Quaternion.Euler(new Vector3(anim_rotation.x, anim_rotation.y + 180, anim_rotation.z));
                    desired_rotation = targetAngle;
                }

                

                if (TurnAnim)
                {
                    _wishVelocity = Vector3.zero;
                    //_velocity = new Vector3(0, _velocity.y, 0);
                    //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetAngle, 500 * Time.deltaTime);
                }

                if ((((rotateDifference > 50.0f) || (_wishVelocity.magnitude > 1.0f) || _MoveRotationSpeed > 0)) && TurnAnim == false)
                {
                    var newRotation = _lerpRotate ? Quaternion.Lerp(transform.localRotation, targetAngle, Time.deltaTime * _SpeedRotate) : targetAngle;
                    var angleDiff = Quaternion.Angle(transform.localRotation, newRotation); // Rotation.Distance is unsigned
                    moveRotationSpeed = (angleDiff) / Time.deltaTime;
                    transform.localRotation = newRotation;
                }
            }
            else
            {
                var newRotation = _lerpRotate ? Quaternion.Slerp(transform.localRotation, targetAngle, Time.deltaTime * _SpeedRotate) : targetAngle;
                var angleDiff = Quaternion.Angle(transform.rotation, newRotation); // Rotation.Distance is unsigned
                moveRotationSpeed = (angleDiff) / Time.deltaTime;
                transform.localRotation = newRotation;
            }

            //if (_MoveRotationSpeed > 100)
                //Debug.Log(_MoveRotationSpeed);

            _MoveRotationSpeed = Mathf.Clamp(moveRotationSpeed, 0, 400);
        }

        public void BuildWishVelocity()
        {
            var result = new Vector3();
            if (_AutoBhop && true) 
            {
                Vector3 direction = new Vector3(_InputVector.x, _InputVector3.y, _InputVector.y);
                //Vector3 direction = new Vector3(_InputVector.x, 0, _InputVector.y);

                Quaternion rotationT = transform.rotation;
                result = rotationT * direction;
            }
            else
            {
                var rot = Quaternion.Euler(Vector3.Scale(LookSource.Transform.eulerAngles, Vector3.up));
                var dir = new Vector3(_InputDirection.x, 0, _InputDirection.y);
                result = rot * dir;
            }

            
            

            _wishVelocity = result;
            _wishVelocity.y = 0f;

            if (_wishVelocity.magnitude != 0) _wishVelocity = _wishVelocity.normalized;
            _wishVelocity *= _Speed;
        }

        public override void AddForce(Vector3 force)
        {
            Force(force);
        }

        public override void UpdateAnimator()
        {
            base.UpdateAnimator();

            _AnimatorMonitor.WithVelocity(_velocity);
            _AnimatorMonitor.WithWishVelocity(_wishVelocity);

            _AnimatorMonitor.RotaionSpeed = _MoveRotationSpeed;
            _AnimatorMonitor.IsGrounded = _OnGrounded;
            _AnimatorMonitor.MoveSpeed = Velocity.magnitude;

            _AnimatorMonitor.Animator.SetBool("turn", TurnAnim);

            _AnimatorMonitor.SetParameters();
        }

        #region Physic

        private void ProcessMovement()
        {
            if (_OnGrounded == false)
            {
                _velocity -= _Gravity * Time.deltaTime * -1 * _GravityFactor;
                Accelerate(Vector3.ClampMagnitude(_wishVelocity, _airClampLength));
                ApplyFriction(ref _velocity, 0, _stopSpeed);
            }
            else
            {
                //_velocity.y = 0;
                Accelerate(_wishVelocity);
                ApplyFriction(ref _velocity, _Friction, _stopSpeed);
            }



            // platform
            _movingPlatform.OnGroundColliderHit(null, _groundObject != null ? _groundObject.transform : null);
            _movingPlatform.Update();
            var vel = _movingPlatform.MoveDirection;
            _origin += vel;
            // end



            ApplyFriction(ref _externalVelocity, _Friction, _stopSpeed);



            CheckGrounded();
            if (_OnGrounded == true)
            {
                Vector3 forward = Vector3.Cross(_groundNormal, -transform.right);
                Vector3 right = Vector3.Cross(_groundNormal, forward);
                Vector3 _wishDir;
                float forwardMove = _InputDirection.y;
                float rightMove = -_InputDirection.x;

                _wishDir = forwardMove * forward + rightMove * right;
                _wishDir.Normalize();

                Vector3 forwardVelocity = Vector3.Cross(
                        _groundNormal,
                        Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(_velocity.x, 0f, _velocity.z)
                    );
                float yVelocityNew = forwardVelocity.normalized.y * new Vector3(_velocity.x, 0f, _velocity.z).magnitude;

                // Apply the Y-movement from slopes
                _velocity.y = yVelocityNew * (_wishDir.y < 0f ? 1f : 1f);
            }
            else
            {
                SurfPhysics.Reflect(ref _velocity, _collider, _origin, Time.deltaTime);
            }

            if (_externalVelocity.sqrMagnitude > 0)
            {
                _velocity += _externalVelocity;
                _wishExternalVelocity = false;
                _externalVelocity = Vector3.zero;
            }


            if (_velocity.sqrMagnitude == 0f)
            {
                // don't penetrate walls
                SurfPhysics.ResolveCollisions(
                        _collider,
                        ref _origin,
                        ref _velocity,
                        _rigidbodyPushForce,
                        1f,
                        _StepOffset, //_surfer.moveData.stepOffset, 
                        null, //_surfer
                        true
                    );
            }
            else
            {
                float maxDistPerFrame = 0.2f;
                Vector3 velocityThisFrame = _velocity * Time.deltaTime;
                float velocityDistLeft = velocityThisFrame.magnitude;
                float initialVel = velocityDistLeft;
                while (velocityDistLeft > 0f)
                {
                    float amountThisLoop = Mathf.Min(maxDistPerFrame, velocityDistLeft);
                    velocityDistLeft -= amountThisLoop;

                    // increment origin
                    Vector3 velThisLoop = velocityThisFrame * (amountThisLoop / initialVel);
                    _origin += velThisLoop;
                    //_surfer.moveData.origin += PlatformProccess();


                    // don't penetrate walls
                    SurfPhysics.ResolveCollisions(
                        _collider,
                        ref _origin,
                        ref _velocity,
                        _rigidbodyPushForce,
                        amountThisLoop / initialVel,
                        _StepOffset, //_surfer.moveData.stepOffset, 
                        null, //_surfer
                        true
                    );

                }
            }

            //PlatformProcess();
        }

        private void ApplyFriction(ref Vector3 velocity, float frictionAmount, float stopSpeed = 1f)
        {
            var speed = velocity.magnitude;
            if (speed < 0.01f) 
            {
                velocity = Vector3.zero;
                return;
            } 

            // Bleed off some speed, but if we have less than the bleed
            //  threshold, bleed the threshold amount.
            float control = (speed < stopSpeed) ? stopSpeed : speed;

            // Add the amount to the drop amount.
            var drop = control  * frictionAmount * Time.deltaTime;

            // scale the velocity
            float newspeed = speed - drop;
            if (newspeed < 0) newspeed = 0;
            if (newspeed == speed) 
            {
                //velocity = Vector3.zero;
                return;
            }

            newspeed /= speed;

            velocity *= newspeed;
        }

        public void Accelerate(Vector3 vector)
        {
            _velocity = WithAcceleration(vector, _Acceleration * Time.deltaTime, _velocity);
        }

        public Vector3 WithAcceleration(Vector3 target, float acceleration, Vector3 velocity)
        {
            if (target.magnitude == 0)
            {
                return velocity;
                //return this; // return vector3
            }

            Vector3 wishDir = target.normalized;
            float length = target.magnitude;
            float currentSpeed = Vector3.Dot(velocity, wishDir);
            float addSpeed = length - currentSpeed;
            if (addSpeed <= 0f)
            {
                return velocity;
                //return this; // return vector3
            }

            float accelerationSpeed = acceleration * length;
            if (accelerationSpeed > addSpeed)
            {
                accelerationSpeed = addSpeed;
            }


            // Add the velocity.
            velocity.x += accelerationSpeed * wishDir.x;
            velocity.y += accelerationSpeed * wishDir.y;
            velocity.z += accelerationSpeed * wishDir.z;

            return velocity;
            //return velocity + wishDir * accelerationSpeed;
            //return this + b * num3;
        }

        public Trace TraceToFloor()
        {
            var origin = _origin;
            var down = origin;
            down.y -= 0.15f; //0.15f;

            return Tracer.TraceCollider(_collider, origin, down, _RaycastLayer, .9f);
        }

        private Trace TraceBounds(Vector3 start, Vector3 end, int layerMask)
        {
            return Tracer.TraceCollider(_collider, start, end, layerMask);
        }

        private void SetGround(GameObject obj)
        {
            if (obj != null)
            {
                _groundObject = obj;
                _velocity.y = 0;
            }
            else 
            {
                ClearGround();
            }
        }

        private bool CheckGrounded()
        {
            //_surfer.moveData.surfaceFriction = 1f;
            var trace = TraceToFloor();

            Debug.Log($"{trace.fraction} {trace.distance}");

            float groundSteepness = Vector3.Angle(Vector3.up, trace.planeNormal);

            if (
                trace.hitCollider == null || 
                groundSteepness > _slopeLimit //||
                //(_jumped && _velocity.y > 0f)
                //(_velocity.y > 0f)
            )
            {
                SetGround(null);

                /*if (movingUp && _surfer.moveType != MoveType.Noclip)
                    _surfer.moveData.surfaceFriction = _config.airFriction;*/
                _OnGrounded = false;
                return false;
            }
            else
            {
                _groundNormal = trace.planeNormal;
                SetGround(trace.hitCollider.gameObject);
                _OnGrounded = true;
                return true;
            }
        }

        public void Force(Vector3 amount)
        {
            _wishExternalVelocity = true;
            ClearGround();
            //_velocity += amount; 
            _externalVelocity += amount;
        }

        private void ClearGround()
        {
            _groundObject = null;
            _OnGrounded = false;
            //GroundCollider = default;
        }

        private void PlatformProcess()
        {
            if (_groundObject == null)
            {
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }

                return;
            }

            if (_groundObject.layer == LayerMask.NameToLayer("Platform"))
            {
                if (transform.parent != _groundObject.transform)
                {
                    transform.SetParent(_groundObject.transform);
                }
            }
            else
            {
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
            }
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + _colliderOffset, _colliderSize);


            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.Scale(_velocity, Vector3.right + Vector3.forward));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _wishVelocity);
        }
        #endif
    }

    
}