using _Demonstration.bot_test;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        [SerializeField] private float visionRange = 15f;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float patrolSpeed = 3f;

        [Header("Settings")]
        [SerializeField] private float _visionRange = 5;
        [SerializeField] private float _AttackVelocity = 5;

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
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
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

            //Debug.Log(_AnimatorMonitor.Animator == null);
            _AnimatorMonitor.SetForwardMovementParameter(agent.velocity.magnitude, Time.deltaTime);
            _AnimatorMonitor.WithVelocity(agent.velocity);
            _AnimatorMonitor.SetMovingParameter(agent.velocity.magnitude > 0);

            _AnimatorMonitor.IsGrounded = agent.velocity.y < 0 ? false : true;
            _AnimatorMonitor.SetParameters();

            RotateToTarget();

            _AttackSinceTime = _AttackSinceTime > 0 ? _AttackSinceTime - Time.deltaTime : 0;

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

        public void Damage()
        {
            _AnimatorMonitor.SetAbilityID((int)_DamageAnimAbility);

            _stopTimer += 0.5f;
        }

        public void Go()
        {

        }

        public void Die()
        {
            agent.enabled = false;
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

        public void SendDamageTarget()
        {
            if (StartEcs == null)
                return;

            if (StartEcs.EcsWorld == null) return;

            ref var damageComponent = ref SendDamageEvent.Send(StartEcs.EcsWorld);

            var origin = (transform.position + transform.forward * .1f) + transform.up * 1.0f;
            var direction = transform.forward;
            var ray = new Ray(origin, direction);
            var cast = Physics.Raycast(ray, out RaycastHit hitInfo, 2f, Configs.Config.s_DamageLayerMask2, QueryTriggerInteraction.Ignore);
            cast = Physics.SphereCast(ray, .5f, out hitInfo, 2f, Configs.Config.s_DamageLayerMask2, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.cadetBlue, 1f);
            if (cast)
            {
                var target = hitInfo.transform.root.gameObject;

                /*if (_advancedNavMeshBot.Target)
                    target = _advancedNavMeshBot.Target.gameObject;*/

                damageComponent.owner = gameObject;
                damageComponent.target = target;
                damageComponent.ray = ray;
                damageComponent.hit = hitInfo;
                damageComponent.damage = _Damage;
                damageComponent.velocity = ray.direction * _AttackVelocity;
            }   
             
            damageComponent.isHit = cast; 
            
            Shared.ParticlesManager.SendParticleEvent(StartEcs.EcsWorld, damageComponent.hit);

        }

        public bool InRaduisAttack()
        {
            if (_target && Vector3.Distance(_target.position, transform.position) < 2f)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = Vector3.Normalize(_target.position - transform.position);
                var dot = Vector3.Dot(forward, toOther);
                if (Mathf.Abs(dot) > 0.8f)
                {
                    return true;
                }
            }

            return false;
        }

        public void RotateToTarget()
        {
            if (_target == null)
                return;

            var targetPosition = _target.position - transform.position;
            targetPosition.y = 0;
            var targetRotation = Quaternion.LookRotation(targetPosition, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }



        #region Nav Mesh Agent

        void Patrol()
        {
            agent.speed = patrolSpeed;

            if (!agent.pathPending && agent.remainingDistance < 2f)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                agent.SetDestination(waypoints[currentWaypoint]);
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
            agent.speed = chaseSpeed;

            if (_target != null)
            {
                agent.SetDestination(_target.position);

                // Lose target if too far
                if (Vector3.Distance(transform.position, _target.position) > _visionRange * 1.5f)
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
            if (Vector3.Distance(transform.position, target.position) < _visionRange * 1.5f)
            {
                return true;
            }

            return false;
        }





        void SetupWaypoints()
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
                agent.SetDestination(waypoints[0]);
            }
        }

        Vector3 GetRandomPointOnNavMesh()
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

        #endregion
    }
}