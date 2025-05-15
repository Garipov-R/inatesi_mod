using Footsteps;
using InatesiCharacter.Movements.SourceEngine;
using InatesiCharacter.SuperCharacter.MovementTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static InatesiCharacter.SuperCharacter.CharacterFootstep;


namespace InatesiCharacter.SuperCharacter 
{
    //[RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class CharacterMotionBase : MonoBehaviour, ICharacter
    {
        [Header("base Settings")]
        [SerializeField] protected LookSource _LookSourceInstance;
        [SerializeField] protected UpdateMethod _UpdateMethod = UpdateMethod.Update;
        [SerializeField] protected Vector3 _GravityDirection = new Vector3(0, -1, 0);
        [SerializeField] protected float _GravityMagnitude = 9.81f;
        [SerializeField] protected float _Mass = 25;
        [SerializeField] protected float _Drag = 10;
        [SerializeField] protected float _SpeedMove = 1.7f;
        [SerializeField] protected AnimatorMonitor _AnimatorMonitor = new AnimatorMonitor();
        [SerializeField] protected RagdollMonitor _RagdollMonitor = new RagdollMonitor();
        [SerializeField] protected float _TimeScale = 1f;
        [SerializeField] protected LayerMask _RaycastLayer;
        [SerializeField] protected bool _IsInputDisabled = true;
        [SerializeField] protected float _GravityFactor = 1f;
        [SerializeField] protected MoveConfig _MoveConfig = new MoveConfig();
        [SerializeField] protected FirstPersonSettings _FirstPersonSettings = new FirstPersonSettings();
        [SerializeField] protected CharacterFootstep _CharacterFootstep = new CharacterFootstep();
        [SerializeField] protected UnityEvent _OnLanded = new();

        [Header("Ecs")]
        [SerializeField] protected bool _UpdateViaMono = false;

        protected CharacterController _CharacterController;
        protected Vector2 _InputVector;
        protected Vector3 _Velocity;
        protected ILookSource _LookSource;
        protected MovementType _MovementType = new Combat (); //new Adventure();
        protected Vector3 _DeltaRotation;
        protected Vector3 _Up;
        protected Vector3 _InputDirection;
        protected Vector3 _InputVector3;
        protected AudioSource _AudioSource;
        protected bool _MovingParameter;
        protected bool _OnGrounded;
        protected float _MoveRotationSpeed;
        protected float _Height;
        protected float _radius;
        protected bool _isNoclip = false;
        protected bool _isFreeze = false;
        protected bool _underwater = false;
        protected Collider _Collider;
        protected float _waterDepth = 0; // is should be here??
        protected RaycastHit _currentGroundInfo;
        protected Renderer _Renderer;
        protected Renderer[] _Renderers;
        protected float _DefaultHeight;

        protected float _yaw;


        public UpdateMethod UpdateMethod { get => _UpdateMethod; set => _UpdateMethod = value; }
        public ILookSource LookSource
        {
            get => _LookSource;
            set
            {
                _LookSource = value;

                _MovementType?.Initialize(this);
            }
        }
        public AnimatorMonitor AnimatorMonitor { get => _AnimatorMonitor; set => _AnimatorMonitor = value; }
        public CharacterController CharacterController { get => _CharacterController; set => _CharacterController = value; }
        public Vector3 DeltaRotation { get => _DeltaRotation; set => _DeltaRotation = value; }
        public bool IsInputDisabled { get => _IsInputDisabled; set => _IsInputDisabled = value; }
        public Vector3 Up { get => _Up; set => _Up = value; }
        public LayerMask RaycastLayer { get => _RaycastLayer; set => _RaycastLayer = value; }
        public float SpeedMove { get => _SpeedMove; set => _SpeedMove = value; }
        public float GravityMagnitude { get => _GravityMagnitude; set => _GravityMagnitude = value; }
        public Vector2 InputVector { get => _InputVector; set => _InputVector = value; }
        public MovementType ActiveMovementType { get => _MovementType; set => _MovementType = value; }
        public RagdollMonitor RagdollMonitor { get => _RagdollMonitor; set => _RagdollMonitor = value; }
        public virtual Vector3 Velocity { get => _Velocity; set => _Velocity = value; }
        public bool UpdateViaMono { get => _UpdateViaMono; set => _UpdateViaMono = value; }
        public Vector3 InputDirection { get => _InputDirection; set => _InputDirection = value; }
        public Vector3 InputVector3 { get => _InputVector3; set => _InputVector3 = value; }
        public AudioSource AudioSource { get => _AudioSource; set => _AudioSource = value; }
        public bool OnGrounded { get => _OnGrounded; set => _OnGrounded = value; }
        public float MoveRotationSpeed { get => _MoveRotationSpeed; set => _MoveRotationSpeed = value; }
        public float GravityFactor { get => _GravityFactor; set => _GravityFactor = value; }
        public MoveConfig MoveConfig { get => _MoveConfig; set => _MoveConfig = value; }
        public FirstPersonSettings FirstPersonSettings { get => _FirstPersonSettings; set => _FirstPersonSettings = value; }
        public virtual float Height { get => _Height; set => _Height = value; }
        public bool IsNoclip { get => _isNoclip; set => _isNoclip = value; }
        public bool IsFreeze { get => _isFreeze; set => _isFreeze = value; }
        public CharacterFootstep CharacterFootstep { get => _CharacterFootstep;  }
        public float Radius { get => _radius * transform.lossyScale.magnitude; }
        public bool Underwater { get => _underwater; set => _underwater = value; }
        public Collider Collider { get => _Collider; }
        public float WaterDepth { get => _waterDepth; set => _waterDepth = value; }
        public Renderer Renderer { get => _Renderer; set => _Renderer = value; }
        public Renderer[] Renderers { get => _Renderers;  }
        public float DefaultHeight { get => _DefaultHeight;  set => _DefaultHeight = value; }
        public UnityEvent OnLanded { get => _OnLanded; set => _OnLanded = value; }


        public virtual void Awake()
        {
            _Up = transform.up;
            _CharacterController = GetComponent<CharacterController>();
             
            _MovementType.LookSource = _LookSource;
            _MovementType.Initialize(this);

            //_AnimatorMonitor = new AnimatorMonitor();
            _AnimatorMonitor.Initialize(gameObject);

            //_RagdollMonitor = new RagdollMonitor();
            _RagdollMonitor.Initialize(this);
            _RagdollMonitor.DisableRagdoll();

            _RaycastLayer = ~(0 | 1 << LayerMask.NameToLayer("Player") << LayerMask.NameToLayer("FPC"));

            _AudioSource = GetComponent<AudioSource>();

            _LookSource = _LookSourceInstance ? _LookSourceInstance : _LookSource;

            _CharacterFootstep.Init(this);

            _Collider = GetComponent<Collider>();

            if (GetComponent<Renderer>() != null)
            {
                _Renderer = GetComponent<Renderer>();
            }
            else if (GetComponentInChildren<Renderer>() != null)
            {
                _Renderer = GetComponentInChildren<Renderer>();
            }


            if (GetComponentsInChildren<Renderer>() != null)
            {
                _Renderers = GetComponentsInChildren<Renderer>();
            }

            if (_Collider != null)
            {
                if (_Collider is CapsuleCollider) _radius = ((CapsuleCollider)_Collider).radius;
            }
            if (_CharacterController != null) 
            {
                _radius = _CharacterController.radius;
            }

            _GravityMagnitude = _MoveConfig.Gravity;

            _SpeedMove = _MoveConfig.Speed;
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

        public virtual void Move(Vector2 input)
        {
            _InputDirection = input;
            _InputVector = _MovementType.GetInputVector(input);

            /*_InputVector = input;
            float yaw = _MovementType.GetDeltaYawRotation(_InputVector.x, _InputVector.y, 0, 0);
            _DeltaRotation.Set(0, yaw, 0);
            _InputVector = _MovementType.GetInputVector(_InputVector);*/
        }

        public virtual void AddForce(Vector3 force) { }

        public virtual bool CheckGround() { return false; }

        public virtual void SetMovementType()
        {
            SetMovementType(new Combat());

            if (_MovementType is Adventure)
            {
                SetMovementType(new Combat());
            }
            else if (_MovementType is Combat)
            {
                SetMovementType(new Adventure());
            }

            _MovementType.Initialize(this);
        }

        public virtual void SetMovementType(MovementType movementType)
        {
            _MovementType = movementType;

            _MovementType.Initialize(this);
        }

        /// <summary>
        /// Update - unity method
        /// </summary>
        public virtual void UpdateCharacter() 
        {
            if (_isFreeze)
                return;

            if (_InputVector.sqrMagnitude > .001f)
            {
                _MovingParameter = true;
            }
            else
            {
                _MovingParameter = false;
            }
        }

        /// <summary>
        /// Fixed Update - unity method
        /// </summary>
        public virtual void UpdatePhysicsCharacter() 
        {
            if (_isFreeze)
                return;

            //_yaw = _MovementType.GetDeltaYawRotation(_InputDirection.x, _InputDirection.y, 0, 0);
            //_DeltaRotation.Set(0, _yaw, 0); 
            
            //UpdateAnimator();


            //CheckWater();

            //_CharacterFootstep.UnderWater = _waterDepth >= 0.5f;
            //_CharacterFootstep.OnWater = _waterDepth > 0 && _waterDepth < 0.5f;
            if (LookSource != null) LookSource.GameObject.GetComponent<AudioLowPassFilter>().enabled = _waterDepth > 0.9f;

            //_CharacterFootstep.UpdateTick();
        }

        public virtual void RotateUpdate()
        {
            Quaternion targetRotation;
            targetRotation = transform.rotation * Quaternion.Euler(_DeltaRotation);

            transform.rotation = targetRotation;
        }

        public virtual void UpdateAnimator()
        {
            if (_AnimatorMonitor.Animator == null) return;

            _AnimatorMonitor.SetHorizontalMovementParameter(_InputVector.x, _TimeScale);
            _AnimatorMonitor.SetForwardMovementParameter(_InputVector.y, _TimeScale);
            _AnimatorMonitor.SetMovingParameter(_MovingParameter);
        }

        public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation) 
        {
            transform.position = position;
            transform.rotation = rotation;
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public virtual void UpdateFootstep()
        {
            _CharacterFootstep.UnderWater = _waterDepth >= 0.5f;
            _CharacterFootstep.OnWater = _waterDepth > 0 && _waterDepth < 0.5f;


            //if (Mathf.Abs(Velocity.magnitude) <= 0) return;

            //Physics.Raycast(ray2, out currentGroundInfo, groundCheckDistance, _characterMotionBase.RaycastLayer, QueryTriggerInteraction.Ignore);
            
            Physics.SphereCast(
                transform.position + _Up * _radius * 2, // _groundCheckHeight
                _radius / 1.8f, 
                _Up * -1,
                out _currentGroundInfo,
                _radius * 2, // _groundCheckDistance
                _RaycastLayer,
                QueryTriggerInteraction.Ignore
            );

            //_CharacterFootstep.CurrentGroundInfo = _currentGroundInfo;
            _CharacterFootstep.UpdateTick(_currentGroundInfo);
        }



        private Collider[] _waterCollidersCache = new Collider[4];
        public void CheckWater()
        {
            if (_Collider == null) return;
            if (_Collider is not CapsuleCollider) return;

            var capsuleCollider = _Collider as CapsuleCollider;


            var extents = capsuleCollider.bounds.extents;
            extents.x *= 0.9f;
            extents.z *= 0.9f;
            var center = transform.position;
            center.y += extents.y;

            var hitCount = Physics.OverlapBoxNonAlloc(center, extents, _waterCollidersCache, transform.rotation, 1 << LayerMask.NameToLayer("Water"));

            for (int i = 0; i < hitCount; i++)
            {
                if (!_waterCollidersCache[i].enabled)
                {
                    continue;
                }

                var water = _waterCollidersCache[i];
                var headPoint = transform.position + new Vector3(0, capsuleCollider.bounds.size.y, 0);

                if (water.bounds.Contains(headPoint))
                {
                    _waterDepth = 1f;
                }
                else
                {
                    var closetsPoint = water.ClosestPoint(headPoint);
                    var dist = Vector3.Distance(closetsPoint, headPoint);
                    _waterDepth = Mathf.Max(0.1f, (capsuleCollider.bounds.size.y - dist) / capsuleCollider.bounds.size.y);
                }
            }

            if (hitCount == 0)
            {
                _waterDepth = 0;
            }

            if (_waterDepth >= 0.5f)
            {
                //_surfer.MoveType = MoveType.Swim;

                _underwater = true;
            }
            //else if (_surfer.MoveType == MoveType.Swim)
            else if (_underwater == true)
            {
                //_surfer.MoveType = MoveType.Walk;

                _underwater = false;
            }
        }




        public virtual void UpdateCharacterMethod()
        {
            //UpdateCharacter();

            if (_isFreeze)
                return;

            if (_InputVector.sqrMagnitude > .001f)
            {
                _MovingParameter = true;
            }
            else
            {
                _MovingParameter = false;
            }
        }

        public virtual void FixedUpdateCharacterMethod()
        {
            //UpdatePhysicsCharacter();

            if (_isFreeze)
                return;

            //_yaw = _MovementType.GetDeltaYawRotation(_InputDirection.x, _InputDirection.y, 0, 0);
            //_DeltaRotation.Set(0, _yaw, 0); 

            //UpdateAnimator();


            //CheckWater();

            
            if (LookSource != null) LookSource.GameObject.GetComponent<AudioLowPassFilter>().enabled = _waterDepth > 0.9f;

            
        }

        public virtual void SetHeight()
        {
            if (_Renderer != null)
            {
                var skinedMeshRender = _Renderer as SkinnedMeshRenderer;
                if (skinedMeshRender != null)
                {
                    _Height = _DefaultHeight = Mathf.Abs(transform.position.y - skinedMeshRender.bounds.max.y);
                    //_Motor.SetCapsuleDimensions(_Motor.Capsule.radius, _Height, _Height / 2);
                }
            }
        }
    }


    [Serializable]
    public sealed class MoveConfig
    {
        [SerializeField] private float _stopSpeed = .4f;
        [SerializeField] private float _airStopSpeed = .4f;
        [SerializeField] private float _friction = 8;
        [SerializeField] private float _acceleration = 5;
        [SerializeField] private float _airAcceleration = 1;
        [SerializeField] private float _Speed = 6f;
        [SerializeField] private float _RunSpeed = 10f;
        [SerializeField] private float _JumpForce = 5f;
        [SerializeField] private float _CrouchingSpeed = 1f;
        [SerializeField] private float _CrouchHeight = .5f;
        [SerializeField] private bool _AutoBhop = true;
        [SerializeField] private float _noClipSpeed = 20f;
        [SerializeField] private float _underwaterFriction = 1f;
        [SerializeField] private float _airClampLength = 1f;
        [SerializeField] private float _maxSpeed = 6f;
        [SerializeField] private float _FallDamageMultiply = 1f;
        [SerializeField] private float _FallDamage = 3f;
        [SerializeField] private float _FallDamageVelocity = -10f;
        [SerializeField] private float _Gravity = -9.81f;

        public float JumpForce { get => _JumpForce; set => _JumpForce = value; }
        public float CrouchingSpeed { get => _CrouchingSpeed; set => _CrouchingSpeed = value; }
        public float CrouchHeight { get => _CrouchHeight; set => _CrouchHeight = value; }
        public bool AutoBhop { get => _AutoBhop; set => _AutoBhop = value; }
        public float Speed { get => _Speed; set => _Speed = value; }
        public float RunSpeed { get => _RunSpeed; set => _RunSpeed = value; }
        public float NoClipSpeed { get => _noClipSpeed; set => _noClipSpeed = value; }
        public float UnderwaterFriction { get => _underwaterFriction; set => _underwaterFriction = value; }
        public float StopSpeed { get => _stopSpeed; set => _stopSpeed = value; }
        public float Friction { get => _friction; set => _friction = value; }
        public float Acceleration { get => _acceleration; set => _acceleration = value; }
        public float AirClampLength { get => _airClampLength; set => _airClampLength = value; }
        public float AirStopSpeed { get => _airStopSpeed; set => _airStopSpeed = value; }
        public float AirAcceleration { get => _airAcceleration; set => _airAcceleration = value; }
        public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = value; }
        public float FallDamageMultiply { get => _FallDamageMultiply; set => _FallDamageMultiply = value; }
        public float FallDamage { get => _FallDamage; set => _FallDamage = value; }
        public float FallDamageVelocity { get => _FallDamageVelocity; set => _FallDamageVelocity = value; }
        public float Gravity { get => _Gravity; set => _Gravity = value; }
    }

    [Serializable]
    public sealed class FirstPersonSettings
    {
        [SerializeField] private Renderer _BodyRenderer;
        [SerializeField] private List<GameObject> _HideObjects = new List<GameObject>();


        public void HideFoot()
        {
            Switch(true);
        }

        public void ShowFoot()
        {
            Switch(false);
        }

        private void Switch(bool state)
        {
            foreach (GameObject obj in _HideObjects)
            {
                obj.transform.localScale = state ? Vector3.one : Vector3.zero;
            }
        }
    }

    [Serializable]
    public class CharacterFootstep
    {
        public enum TriggeredBy
        {
            COLLISION_DETECTION,    // The footstep sound will be played when the physical foot collides with the ground.
            TRAVELED_DISTANCE       // The footstep sound will be played after the character has traveled a certain distance
        }

        [SerializeField] TriggeredBy triggeredBy = TriggeredBy.TRAVELED_DISTANCE;
        [SerializeField] float distanceBetweenSteps = 1.8f; 
        [SerializeField] float minVolume = 0.3f;
        [SerializeField] float maxVolume = 0.5f;
        [SerializeField] float groundCheckHeight = 3.5f;
        [SerializeField] float groundCheckDistance = 1f;

        private float stepCycleProgress;
        private bool previouslyGrounded;
        private bool _underWater;
        private bool _onWater;
        private RaycastHit _currentGroundInfo;
        private CharacterMotionBase _characterMotionBase;

        public bool UnderWater { get => _underWater; set => _underWater = value; }
        public bool OnWater { get => _onWater; set => _onWater = value; }
        public RaycastHit CurrentGroundInfo { get => _currentGroundInfo; set => _currentGroundInfo = value; }

        public void Init(CharacterMotionBase characterMotionBase)
        {
            _characterMotionBase = characterMotionBase;
        }

        public void UpdateTick(RaycastHit raycastHit)
        {
            _currentGroundInfo = raycastHit;
            if (_characterMotionBase == null) return;
            if (SurfaceManager.singleton == null) return;

            //CheckGround();

            if (!previouslyGrounded && _characterMotionBase.OnGrounded)
            {
                PlayLandSound();
            }


            if (triggeredBy == TriggeredBy.TRAVELED_DISTANCE)
            {
                //float speed = _characterMotionBase.Velocity.magnitude;

                if (_characterMotionBase.Velocity.magnitude == 0)
                {
                    stepCycleProgress = distanceBetweenSteps;
                }

                if (_characterMotionBase.OnGrounded || _characterMotionBase.WaterDepth >= 0.1f)
                {
                    previouslyGrounded = true;
                    AdvanceStepCycle(_characterMotionBase.Velocity.magnitude * Time.deltaTime);
                }
                else
                {
                    previouslyGrounded = false;
                }
            }
        }

        private void AdvanceStepCycle(float increment)
        {
            stepCycleProgress += increment;

            if (stepCycleProgress > distanceBetweenSteps)
            {
                stepCycleProgress = 0f;
                PlayFootstep();
            }
        }

        private void PlayFootstep()
        {
            AudioClip randomFootstep = null;

            try
            {
                if (_onWater) 
                {
                    randomFootstep = SurfaceManager.singleton.GetWaterFootstep();
                }
                else if (_underWater)
                {
                    randomFootstep = SurfaceManager.singleton.GetUnderWaterFootstep();
                }
                else
                {
                    randomFootstep = SurfaceManager.singleton.GetFootstep(_currentGroundInfo.collider, _currentGroundInfo.point);
                }

                if (randomFootstep == null)
                    return;

                float randomVolume = UnityEngine.Random.Range(minVolume, maxVolume);

                if (randomFootstep)
                {
                    _characterMotionBase.AudioSource.PlayOneShot(randomFootstep, randomVolume);
                }
            }
            catch { }
        }

        public void TryPlayFootstep()
        {
            if (_characterMotionBase == null) return;
            if (SurfaceManager.singleton == null) return;
            PlayFootstep();
        }

        private void PlayLandSound()
        {
            PlayFootstep();
        }

        private void CheckGround()
        {
            if (Mathf.Abs(_characterMotionBase.Velocity.magnitude) <= 0) return;

            Ray ray2 = new(
                _characterMotionBase.transform.position + _characterMotionBase.Up * _characterMotionBase.Radius * 2, // _groundCheckHeight
                _characterMotionBase.Up * -1
            );

            //Physics.Raycast(ray2, out currentGroundInfo, groundCheckDistance, _characterMotionBase.RaycastLayer, QueryTriggerInteraction.Ignore);

            Physics.SphereCast(
                ray2,
                _characterMotionBase.Radius / 1.8f,
                out _currentGroundInfo,
                _characterMotionBase.Radius * 2, // _groundCheckDistance
                _characterMotionBase.RaycastLayer,
                QueryTriggerInteraction.Ignore
            );
        }
    }

    public struct CharacterFootstep2
    {
        private void CheckGround(CharacterMotionBase characterMotionBase, out RaycastHit currentGroundInfo)
        {
            if (Mathf.Abs(characterMotionBase.Velocity.magnitude) <= 0) 
            {
                currentGroundInfo = new RaycastHit();
                return;
            }
            

            Ray ray2 = new(
                characterMotionBase.transform.position + characterMotionBase.Up * characterMotionBase.Radius * 2, // _groundCheckHeight
                characterMotionBase.Up * -1
            );

            //Physics.Raycast(ray2, out currentGroundInfo, groundCheckDistance, _characterMotionBase.RaycastLayer, QueryTriggerInteraction.Ignore);

            Physics.SphereCast(
                ray2,
                characterMotionBase.Radius / 1.8f,
                out currentGroundInfo,
                characterMotionBase.Radius * 2, // _groundCheckDistance
                characterMotionBase.RaycastLayer,
                QueryTriggerInteraction.Ignore
            );

            /*if (!previouslyGrounded && characterMotionBase.OnGrounded)
            {
                PlayLandSound();
            }*/
        }
    }
}