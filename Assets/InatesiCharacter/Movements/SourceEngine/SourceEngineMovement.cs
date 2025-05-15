using InatesiCharacter.Movements.SourceEngine.TraceUtility;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.SuperCharacter.MovementTypes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InatesiCharacter.Movements.SourceEngine
{
    public class SourceEngineMovement : CharacterMotionBase
    {
        [Header("Settings")]
        [SerializeField] protected Vector3 _colliderSize = new Vector3(1f, 2f, 1f);
        [SerializeField] protected Vector3 _colliderOffset = new Vector3(0, 1, 0);
        [SerializeField] protected bool _AutoBhop = true;
        [SerializeField] protected bool _lerpRotate = false;

        protected Collider _collider;
        protected GameObject _colliderObject;
        protected Vector3 _prevPosition;
        protected bool _jumped;
        protected MovingPlatform _movingPlatform;

        #region Source Movement fields
        // source movement fields
        [Header("Source fields")]
        [SerializeField] private float _rigidbodyPushForce = 0.5f;
        [SerializeField] private float _stopSpeed = 2.7f;
        [SerializeField] private float _Speed = 5f;
        [SerializeField] private float _SpeedRotate = 6f;
        [SerializeField] private float _JumpForce = 20f;
        [SerializeField] private float _StepOffset = .2f;

        // move data
        [SerializeField] private float _Friction = 3f;
        [SerializeField] private float _Acceleration = 5f;
        [SerializeField] private Vector3 _Gravity = new Vector3(0, -9.81f, 0);
        [SerializeField] private float _slopeLimit = 45f;
        [SerializeField] private float _airClampLength = 1f;

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

            //_collider = _CharacterController;

            _Height = _CharacterController.height;

            _movingPlatform = new MovingPlatform(transform);

            if (_CharacterController != null)
            {
                _CharacterController.stepOffset = _StepOffset;
                _CharacterController.slopeLimit = _slopeLimit;
            }
        }



        public override void UpdateCharacter()
        {
            base.UpdateCharacter();

            UpdateInput();


            // ========= stuff ==============


            _origin = transform.position;
            Vector3 positionalMovement = _origin - _prevPosition;
            //Debug.Log(positionalMovement);
            ProcessMovement();

            //_CharacterController.Move(positionalMovement);

            //_origin = transform.position;
            _prevPosition = transform.position;




            // ============= end stuff =================

            Velocity = _velocity;
            _CharacterController.Move(Velocity * Time.deltaTime);
        }

        private void ProcessMovement()
        {
            CheckGrounded();

            if (_OnGrounded == false)
            {
                _velocity -= _Gravity * Time.deltaTime * -1 * _GravityFactor;
                ApplyFriction(ref _velocity, 0, _stopSpeed);
                AirAccelerate(Vector3.ClampMagnitude(_wishVelocity, _airClampLength));
            }
            else
            {
                ApplyFriction(ref _velocity, _Friction, _stopSpeed);
                Accelerate(_wishVelocity);
            }

            ApplyFriction(ref _externalVelocity, _Friction, _stopSpeed);






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
                //SurfPhysics.Reflect(ref _velocity, _collider, transform.position, Time.deltaTime);
            }

            if (_externalVelocity.sqrMagnitude > 0)
            {
                _velocity += _externalVelocity;
                _wishExternalVelocity = false;
                _externalVelocity = Vector3.zero;
            }
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
                _CharacterController.enabled = false;
                transform.position = new Vector3(20, 20, 20);
                _CharacterController.enabled = true;
            }
        }


        public override void RotateUpdate()
        {
            float moveRotationSpeed = 0;
            var targetAngle = Quaternion.Euler(Vector3.Scale(LookSource.Transform.eulerAngles, new Vector3(0f, 1f, 0f)));
            targetAngle = _MovementType.GetRotation(_InputDirection.x, InputDirection.y);
            //targetAngle = Quaternion.Euler(Vector3.Scale(_velocity, Vector3.forward + Vector3.right));

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
                    var newRotation = _lerpRotate ? Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * _SpeedRotate) : targetAngle;
                    var angleDiff = Quaternion.Angle(transform.rotation, newRotation); // Rotation.Distance is unsigned
                    moveRotationSpeed = (angleDiff) / Time.deltaTime;
                    transform.rotation = newRotation;
                }
            }
            else
            {
                var newRotation = _lerpRotate ? Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * _SpeedRotate) : targetAngle;
                var angleDiff = Quaternion.Angle(transform.rotation, newRotation); // Rotation.Distance is unsigned
                moveRotationSpeed = (angleDiff) / Time.deltaTime;
                transform.rotation = newRotation;
            }

            //if (_MoveRotationSpeed > 100)
            //Debug.Log(_MoveRotationSpeed);

            _MoveRotationSpeed = Mathf.Clamp(moveRotationSpeed, 0, 400);
        }

        public void BuildWishVelocity()
        {
            var result = new Vector3();
            if (_AutoBhop)
            {
                Vector3 direction = new Vector3(_InputVector.x, _InputVector3.y, _InputVector.y);
                direction = new Vector3(_InputDirection.x, _InputVector3.y, _InputDirection.y);
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

        private void OnSteepSlope()
        {
            var trace = TraceBounds(transform.position, transform.position + _velocity, RaycastLayer);

            Debug.Log(_CharacterController.isGrounded);


            if (_CharacterController.isGrounded == false) return;
            
            Vector3 slideDirection = transform.up - trace.planeNormal * Vector3.Dot(transform.up, trace.planeNormal);
            float slideSpeed = 10f + Time.deltaTime;

            float slopeAngle = Vector3.Angle(trace.planeNormal, transform.up);
            if (slopeAngle > _slopeLimit)
            {
                var vel = slideDirection * slideSpeed;
                _velocity.y += vel.y - trace.hitPoint.y;
            }

            


            
        }

        #region Physic

        private void ApplyFriction(ref Vector3 velocity, float frictionAmount, float stopSpeed = 1f)
        {
            var speed = velocity.magnitude;
            if (speed < 0.01f) return;

            // Bleed off some speed, but if we have less than the bleed
            //  threshold, bleed the threshold amount.
            float control = (speed < stopSpeed) ? stopSpeed : speed;

            // Add the amount to the drop amount.
            var drop = control * frictionAmount * Time.deltaTime;

            // scale the velocity
            float newspeed = speed - drop;
            if (newspeed < 0) newspeed = 0;
            if (newspeed == speed) return;

            newspeed /= speed;

            velocity *= newspeed;
        }

        public void Accelerate(Vector3 vector)
        {
            _velocity = WithAcceleration(vector, _Acceleration * Time.deltaTime, _velocity);
        }

        public void AirAccelerate(Vector3 vector)
        {
            _velocity = WithAirAcceleration(vector, _velocity);
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

            return velocity + wishDir * accelerationSpeed;
        }

        public Vector3 WithAirAcceleration(Vector3 target, Vector3 velocity)
        {
            if (target.magnitude == 0)
            {
                return velocity;
                //return this; // return vector3
            }

            Vector3 wishDir = target.normalized;
            float length = target.magnitude;
            float currentSpeed = Vector3.Dot(velocity, wishDir);
            float addSpeed = Mathf.Clamp( length - currentSpeed, 0, 3200 * Time.deltaTime);

            return velocity + wishDir * addSpeed;
        }

        public Trace TraceToFloor()
        {
            var origin = transform.position + _colliderOffset;
            var down = origin;
            down.y -= 0.2f;

            return Tracer.TraceCollider(_collider, origin, down, _RaycastLayer, 1);
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

            float groundSteepness = Vector3.Angle(Vector3.up, trace.planeNormal);

            if (
                trace.hitCollider == null /* && _CharacterController.isGrounded == false*/ ||

                (trace.hitCollider != null && groundSteepness > _slopeLimit)
            )
            {
                SetGround(null);
                if (trace.hitCollider != null) _groundNormal = trace.planeNormal;
                _OnGrounded = false;
                return false;
            }
            else
            {
                _groundNormal = trace.planeNormal;
                SetGround(trace.hitCollider ? trace.hitCollider.gameObject : null);
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

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + _colliderOffset, _colliderSize);
            Gizmos.DrawSphere(transform.position + _colliderOffset, .1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.Scale( _velocity.normalized * 5f, Vector3.right + Vector3.forward));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _wishVelocity.normalized * 5f);
        }

        private void OnValidate()
        {
            if (_CharacterController != null)
            {
                _CharacterController.stepOffset = _StepOffset;
                //_CharacterController.slopeLimit = _slopeLimit;
            }
        }
#endif

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (_CharacterController.collisionFlags == CollisionFlags.Sides)
            {
                /*var direction = transform.position - hit.point;
                direction.Normalize();
                var distance = _velocity.magnitude;
                Vector3 penetrationVector = direction * distance;
                Vector3 velocityProjected = Vector3.Project(penetrationVector, -hit.normal);
                velocityProjected.y = 0;
                _velocity -= velocityProjected;*/



                var direction = _velocity.normalized;

                if (_wishVelocity.magnitude > 0)
                {
                    direction = _velocity;
                }
                else
                {
                    direction = transform.forward;
                }

                Debug.DrawLine(transform.position, transform.position + Vector3.ProjectOnPlane(direction, hit.normal), Color.cyan);
                _velocity += Vector3.ProjectOnPlane(direction.normalized, hit.normal) * direction.magnitude * Time.deltaTime;
            }

            if (_CharacterController.collisionFlags == CollisionFlags.Below)
            {
                //Debug.Log($"below {hit.collider.name} ");

               /* var direction = _wishVelocity.normalized;
                Debug.DrawLine(transform.position, transform.position + Vector3.ProjectOnPlane(_velocity.normalized, hit.normal), Color.cyan);
                _velocity += Vector3.ProjectOnPlane(direction, hit.normal) * Time.deltaTime;*/
            }



            /* if (_CharacterController.collisionFlags == CollisionFlags.Sides)
             {
                 if (Vector3.Dot(hit.normal, _velocity) < 0)
                 { 
                    _velocity -= hit.normal * Vector3.Dot(hit.normal, _velocity); 
                 }
             }*/
        }
    }
}