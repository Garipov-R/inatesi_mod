using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.Stuff;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Bot3
{
    public partial class BotBehaviourBase : MonoBehaviour
    {
        [SerializeField] private int _Health = 1;
        [SerializeField] private Animator _Animator;
        [SerializeField] private EntityFlag[] _TargetEntityFlag;
        [SerializeField] private float _Damage = 1;
        [SerializeField] private GameObject _hitCollidersContainer;
        [SerializeField] private GameObject _collidersContainer;
        [SerializeField] private CharacterController _characterController;

        [Header("Effects")]
        [SerializeField] private VisualEffect _DamageVisualEffect;
        [SerializeField] private Material _DamageMaterial;

        [Header("Dynamic Waypoint System")]
        [SerializeField] private Transform[] fixedWaypoints;
        [SerializeField] private LayerMask waypointSearchMask;
        [SerializeField] private float searchRadius = 10f;
        [SerializeField] private bool generateRandomWaypoints = false;
        [SerializeField] private int numberOfRandomWaypoints = 5; 
        
        [Header("AI Behavior")]
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float patrolSpeed = 3f;
        [SerializeField] private float _turnSpeed = 5f;
        [SerializeField] private float _stopDistance = 1f;
        [SerializeField] private int _AgentID = 1;
        [SerializeField] private LayerMask _AreaMask = -1;

        [Header("Settings")]
        [SerializeField] private float _visionRange = 5;
        [SerializeField] private float _AttackVelocity = 5;
        [SerializeField] private bool _ZigZagWalk = false;
        [SerializeField] private bool _Gravity = true;
        [SerializeField] private bool _TrainRotation = false;

        [Header("Animation setings")]
        [SerializeField] private AnimationAbility _AttackAnimAbility = AnimationAbility.Attack;
        [SerializeField] private SlotData _AttackSlot = SlotData.AttackMontirovka;
        [SerializeField] private AnimationAbility _DamageAnimAbility = AnimationAbility.Damage;
        [SerializeField] private AnimationAbility _DeathAnimAbility = AnimationAbility.Die;
        [SerializeField] private WeaponTypeAnimation _CurrentWeaponTypeAnimation = WeaponTypeAnimation.montirovka;
        [SerializeField] private AnimatorMonitor _AnimatorMonitor;
        [SerializeField] private bool _IsAttackAnimationEvent = false;
        [SerializeField] private float _AttackDelay = 1f;

        private float _stopTimer;
        private bool _isDead;
        private float _AttackSinceTime;
        private bool _attacked;
        private BotState _currentBotState;
        private Transform _target;
        private NavMeshAgent agent;
        private List<Vector3> waypoints = new List<Vector3>();
        private int currentWaypoint = 0;
        private float _currentSpeed = 6;
        private Vector3 _velocity;

        private NavMeshPath _path;
        private Vector3[] _pathCorners;
        private int _currentCornerIndex;
        private bool _isMoving = false;
        private NavMeshQueryFilter _queryFilter;
        private float _remainingDistance;

        public int Health { get => _Health; set => _Health = value; }
        public VisualEffect DamageVisualEffect { get => _DamageVisualEffect; set => _DamageVisualEffect = value; }
        public Animator Animator { get => _Animator; set => _Animator = value; }
        public EntityFlag[] TargetsEntityFlag { get => _TargetEntityFlag; set => _TargetEntityFlag = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }
        [Zenject.Inject] public StartEcs StartEcs { get; set; }
        public Material DamageMaterial { get => _DamageMaterial; set => _DamageMaterial = value; }
        public BotState CurrentBotState { get => _currentBotState; set => _currentBotState = value; }
        public Transform Target { get => _target; set => _target = value; }


        private void Awake()
        {
            _AnimatorMonitor = new();
            _AnimatorMonitor.Initialize(gameObject);
            _path = new NavMeshPath();

            int agentTypeCount = NavMesh.GetSettingsCount();
            int agentTypeId = 0;
            for (int i = 0; i < agentTypeCount; i++)
            {
                NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);

                agentTypeId = settings.agentTypeID;

                if (_AgentID == i)
                {
                    break;
                }

                string agentTypeName = NavMesh.GetSettingsNameFromID(agentTypeId);
            }

            _queryFilter = new NavMeshQueryFilter 
            { 
                agentTypeID = agentTypeId, 
                areaMask = _AreaMask
            };

            if (GetComponent <CharacterController > ())
            {
                _characterController = GetComponent <CharacterController> ();
            }
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                agent.updatePosition = false;
                agent.updateRotation = false;
            }
            
            SetupWaypoints();
        }

        private void Update()
        {
            if (G.IsPause)
                return;

            if (_isDead)
                return;


            if (_stopTimer <= 0)
            {
                switch (_currentBotState)
                {
                    case BotState.Patrolling:
                        Patrol();
                        break;
                    case BotState.Chasing:
                        Chase();
                        break;
                    case BotState.Investigating:
                        Investigate();
                        break;
                }


                Go();
            }
            else
            {
                _stopTimer -= Time.deltaTime;
            }

            UpdateAnimators();

            _AttackSinceTime = _AttackSinceTime > 0 ? _AttackSinceTime - Time.deltaTime : 0;

            UpdateRotation();
            UpdateMovement();

            RotateToTarget();

            if (agent != null && agent.updatePosition == false && agent.isOnNavMesh)
            {
                agent.nextPosition = transform.position;
            }
        }

        private void LateUpdate()
        {

        }


        private void UpdateMovement()
        {
            var movementDirection = GetMovementDirection();
             
            
            if (agent != null && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                if (_characterController.isGrounded)
                {
                    _currentSpeed = 0;
                }
            }

            if (_TrainRotation)
            {
                movementDirection = transform.forward;
            }
            
            Vector3 movement = _currentSpeed * Time.deltaTime * movementDirection;


            if (!_characterController.isGrounded && _Gravity == true)
            {
                movement.y -= 9.81f * Time.deltaTime;
            }

            if (movementDirection != Vector3.zero)
            {
                if (_ZigZagWalk)
                {
                    movement += .1f * Mathf.Sin(Time.time * 2) * transform.right;
                }
                _characterController.Move(movement);
                _velocity = _characterController.velocity;
            }
            else
            {
                _velocity = Vector3.zero;
            }
                
        }

        private void UpdateRotation()
        {
            if (InRaduisAttack() == true)
            {
                return;
            }

            var movementDirection = GetMovementDirection();


            if (movementDirection != Vector3.zero)
            {
                if (_ZigZagWalk)
                {
                    movementDirection += transform.right * Mathf.Sin(Time.time * 20) * 1;
                }

                var targetRotation = Quaternion.LookRotation(movementDirection, transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
            }
        }

        public void RotateToTarget()
        {
            if (_target == null)
                return;

            if (InRaduisAttack() == false)
                return;

            var targetPosition = _target.position - transform.position;
            targetPosition.y = 0;
            var targetRotation = Quaternion.LookRotation(targetPosition, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
        }

        private Vector3 GetMovementDirection()
        {
            if (agent != null)
            {
                Vector3 velocity = agent.nextPosition - transform.position;
                Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
                Vector3 moveDirection = horizontalVelocity.normalized;
                if (moveDirection != Vector3.zero)
                {
                    return moveDirection;
                }
            }
            else
            {
                if (!_isMoving || _pathCorners == null || _pathCorners.Length == 0)
                    return Vector3.zero;

                Vector3 targetCorner = _pathCorners[_currentCornerIndex];

                float distanceToCorner = Vector3.Distance(transform.position, targetCorner);
                _remainingDistance = distanceToCorner;
                if (distanceToCorner <= _stopDistance)
                {
                    _currentCornerIndex++;

                    if (_currentCornerIndex >= _pathCorners.Length)
                    {
                        _isMoving = false;
                        return Vector3.zero;
                    }

                    targetCorner = _pathCorners[_currentCornerIndex];
                }

                Vector3 direction = (targetCorner - transform.position);
                direction.y = 0; // Keep the bot on the ground
                direction.Normalize();

                Vector3 moveAmount = direction * _currentSpeed * Time.deltaTime;

                return direction;
            }
                

            

            return Vector3.zero;
        }

        private Vector3 GetNormalGround()
        {
            Physics.Raycast(
                transform.position,
                -transform.up,
                out RaycastHit hit,
                2f,
                Configs.Config.s_DefaultLayerMask,
                QueryTriggerInteraction.Ignore
            );
            return hit.transform != null ? hit.normal : Vector3.zero;
        }

        private void UpdateAnimators()
        {
            if (agent != null)
            {
                _velocity = agent.velocity;
            }

            _AnimatorMonitor.SetForwardMovementParameter(_velocity.magnitude, Time.deltaTime);
            _AnimatorMonitor.WithVelocity(_velocity);
            _AnimatorMonitor.SetMovingParameter(_velocity.magnitude > 0);

            _AnimatorMonitor.IsGrounded = _velocity.y <= -1.45f ? false : true;
            _AnimatorMonitor.SetParameters();



            switch (_currentBotState)
            {
                case BotState.Patrolling:
                    _AnimatorMonitor.Animator.SetLayerWeight(1, 0);
                    break;
                case BotState.Chasing:
                    _AnimatorMonitor.Animator.SetLayerWeight(1, 1);
                    _AnimatorMonitor.SetSlot0((int)_CurrentWeaponTypeAnimation);
                    break;
            }
        }

        #region EVENTS

        public void Damage()
        {
            //_AnimatorMonitor.SetAbilityID((int)_DamageAnimAbility);

            _stopTimer += 0.1f;
        }

        public void Go()
        {

        }

        public void Die()
        {
            if (agent) agent.enabled = false;
            _isDead = true;

            //_Animator?.ResetTrigger("attack");
            _AnimatorMonitor.SetAbilityID((int)_DeathAnimAbility);

            if (_collidersContainer)
            {
                _collidersContainer.SetActive(false);
            }

            if (_hitCollidersContainer)
            {
                _hitCollidersContainer.SetActive(false);
            }
        }

        public void Attack()
        {
            if (_AttackSinceTime > 0)
                return;

            _AnimatorMonitor.Animator.SetLayerWeight(1, 1);
            _AnimatorMonitor.SetSlot0((int)_CurrentWeaponTypeAnimation);
            _AnimatorMonitor.SetSlotData((int)_AttackSlot);

            if (_IsAttackAnimationEvent == false)
            {
                SendDamageTarget();
            }

            _AttackSinceTime = _AttackDelay;
        }

        public void StopAttack()
        {
            //_Animator.ResetTrigger("attack");
        }

        #endregion

        #region Nav Path

        public void SetDestination(Vector3 destination)
        {
            NavMesh.CalculatePath(transform.position, destination, _queryFilter, _path);

            if (_path.status == NavMeshPathStatus.PathComplete)
            {
                _pathCorners = _path.corners; // Get the list of waypoints
                _currentCornerIndex = 0;     // Start from the first waypoint
                _isMoving = true;
            }
            else
            {
                NavMesh.FindClosestEdge(transform.position, out NavMeshHit hit, _queryFilter);
                Debug.Log(_path.status);

                if (_path.status != NavMeshPathStatus.PathComplete)
                {
                    if (_path.status == NavMeshPathStatus.PathPartial)
                    {
                        _pathCorners = _path.corners; 
                        _currentCornerIndex = 0;
                    }

                    //_isMoving = false;
                    Debug.Log("No valid path found!");
                }
                else
                {
                    _pathCorners = new Vector3[0];
                    _pathCorners[0] = hit.position;
                    _currentCornerIndex = 0;
                }
            }
        }

        #endregion

        #region STATES

        void Patrol()
        {
            if (agent) agent.speed = patrolSpeed;
            _currentSpeed = patrolSpeed;

            if (agent != null && !agent.pathPending && agent.remainingDistance < 2f && waypoints.Count > 0)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                //agent.SetDestination(waypoints[currentWaypoint]);
                SetDestination(waypoints[currentWaypoint]);
            }

            if (agent == null && _remainingDistance < _stopDistance && waypoints.Count > 0)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                SetDestination(waypoints[currentWaypoint]);
            }

            // Check for player/enemy in vision
            /*Collider[] hits = Physics.OverlapSphere(transform.position, visionRange);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    _target = hit.transform;
                    currentState = AIState.Chasing;
                    break;
                }
            }*/
        }

        void Chase()
        {
            if (agent) agent.speed = chaseSpeed;
            _currentSpeed = chaseSpeed;

            if (_target != null)
            {
                //agent.SetDestination(_target.position);
                SetDestination(_target.position);

                // Lose target if too far
                if (Vector3.Distance(transform.position, _target.position) > _visionRange)
                {
                    _target = null;
                    _currentBotState = BotState.Patrolling;
                }
            }
            else
            {
                _currentBotState = BotState.Patrolling;
            }
        }

        void Investigate()
        {
            // Implement investigation behavior
        }

        #endregion




        public void SendDamageTarget()
        {
            if (StartEcs == null)
                return;

            if (StartEcs.EcsWorld == null) return;


            var origin = (transform.position + transform.forward * 0) + transform.up * 0;
            var direction = transform.forward;

            direction = InRaduisAttack() ? ((_target.position + transform.up) - transform.position).normalized : transform.forward;

            var ray = new Ray(origin, direction);

            var cast = Physics.Raycast(ray, out RaycastHit hitInfo, 2f, Configs.Config.s_DamageLayerMask2, QueryTriggerInteraction.Ignore);
            cast = Physics.SphereCast(ray, .1f, out hitInfo, 2f, Configs.Config.s_DamageLayerMask2, QueryTriggerInteraction.Ignore);
            //Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.cadetBlue, 1f);
            
            ref var damageComponent = ref SendDamageEvent.Send(StartEcs.EcsWorld);

            if (cast)
            {
                var target = hitInfo.transform.root.gameObject;

                if (target.TryGetComponent(out EntityFlagMono targetEntityFlagMono))
                {
                    if (TryGetComponent(out EntityFlagMono entityFlagMono))
                    {
                        if (targetEntityFlagMono.EntityFlag != entityFlagMono.EntityFlag)
                        {
                            damageComponent.owner = gameObject;
                            damageComponent.target = target;
                            damageComponent.ray = ray;
                            damageComponent.hit = hitInfo;
                            damageComponent.damage = _Damage;
                            damageComponent.velocity = ray.direction * _AttackVelocity;
                        }
                    }
                    
                }
            }

            damageComponent.isHit = cast;

            Shared.ParticlesManager.SendParticleEvent(StartEcs.EcsWorld, damageComponent.hit);

        }

        public void SetTarget(Transform target)
        {
            _currentBotState = BotState.Chasing;

            if (target == null)
            {
                _currentBotState = BotState.Patrolling;
            }

            _target = target;
        }

        public bool IsVisibleTarget(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < _visionRange)
            {
                return true;
            }

            return false;
        }

        public bool InRaduisAttack()
        {
            if (_target && Vector3.Distance(_target.position, transform.position) < 3f)
            {
                return TargetOnFront();
            }

            return false;
        }

        private bool TargetOnFront()
        {
            if (_target == null)
            {
                return false;
            }

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = Vector3.Normalize(Vector3.Scale(_target.position, new Vector3(1, 0, 1)) - Vector3.Scale(transform.position, new Vector3(1, 0, 1)));
            var dot = Vector3.Dot(forward, toOther);
            if (Mathf.Abs(dot) > 0.8f)
            {
                return true;
            }

            return false;
        }

        private void SetupWaypoints()
        {
            // Add fixed waypoints
            if (fixedWaypoints != null)
            {
                foreach (Transform wp in fixedWaypoints)
                {
                    if (wp)
                        waypoints.Add(wp.position);
                }
            }

            // Generate random waypoints on NavMesh
            if (generateRandomWaypoints)
            {
                for (int i = 0; i < numberOfRandomWaypoints; i++)
                {
                    Vector3 randomPoint = GetRandomPointOnNavMesh();
                    if (randomPoint != Vector3.zero)
                    {
                        waypoints.Add(randomPoint);
                    }
                }
            }

            if (waypoints.Count > 0)
            {
                //agent.SetDestination(waypoints[0]);
                SetDestination(waypoints[0]);
            }
        }

        private Vector3 GetRandomPointOnNavMesh()
        {
            Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return Vector3.zero;
        }
    }
}