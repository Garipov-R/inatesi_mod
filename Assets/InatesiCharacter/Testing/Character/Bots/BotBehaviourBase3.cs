using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs4.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class BotBehaviourBase3 : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Hurt
        }

        public enum Category
        {
            Enemy,
            Friend
        }

        [Header("General Settings")]
        [SerializeField] private Category _category;
        [SerializeField] private State _state;

        [Header("Movement Settings")]
        public float _moveSpeed = 3.5f;
        public float _rotationSpeed = 120f;
        public float _stoppingDistance = 1.5f;
        public float _attackRange = 2f; 

        [Header("Random Settings")]
        [SerializeField] private bool _isRandomPosition;
        public float _wanderRadius = 10f;
        public float _wanderTimer = 5f;

        [Header("Target Settings")]
        [SerializeField] private bool _seekTarget;
        [SerializeField] private float _stopTargetRadius = 2;
        [SerializeField] private Transform _Target;
        [SerializeField] private NavMeshAgent _navMeshAgent; 



        private Vector3 _lastPosition;
        private float _randomPositionTimer;
        private EcsWorld _EcsWorld;
        
        public EcsWorld EcsWorld { get => _EcsWorld; set => _EcsWorld = value; }



        private void Start()
        {
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
            _navMeshAgent.speed = _moveSpeed;
            _lastPosition = transform.position;

            _randomPositionTimer = _wanderTimer;
        }

        private void Update()
        {
            //RandomPositionProccess();
            //SeekTarget();

            if (_category == Category.Friend)
            {
                SeekTarget();
            }

            MoveProcess();

            HandleRootMotion();
        }

        private void MoveProcess()
        {
            transform.position += _navMeshAgent.nextPosition - _lastPosition;
            //Debug.DrawRay(transform.position, (_navMeshAgent.nextPosition - _lastPosition).normalized, Color.red, _timer);

            if ((_navMeshAgent.nextPosition - _lastPosition).magnitude > 0)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, 
                    Quaternion.LookRotation((_navMeshAgent.nextPosition - _lastPosition).normalized), 
                    Time.deltaTime * _rotationSpeed
                );
            }
            _lastPosition = transform.position;
        }

        private void RandomPositionProccess()
        {
            if (_isRandomPosition == false) return;

            _randomPositionTimer += Time.deltaTime;

            if (_randomPositionTimer >= _wanderTimer || Vector3.Distance(_navMeshAgent.steeringTarget, transform.position) <= _navMeshAgent.radius)
            {
                Vector3 newPos = RandomNavSphere(transform.position, _wanderRadius, Configs.Config.s_DefaultLayerMask);
                _navMeshAgent.SetDestination(newPos);
                _randomPositionTimer = 0;
            }
        }

        private void SeekTarget()
        {
            if (_seekTarget == false) return;   
            if (_Target == null) return;

            if (Vector3.Distance(_Target.position, transform.position) <= _stopTargetRadius)
            {
                _navMeshAgent.SetDestination(transform.position);
                return;
            }
            else
            {
                _navMeshAgent.SetDestination(_Target.position);
            }
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }

        void HandleRootMotion()
        {
            if (_navMeshAgent.isOnNavMesh)
            {
                // Sync NavMeshAgent with root motion position
                _navMeshAgent.nextPosition = transform.position;
            }
        }

        private void GetPlayer()
        {
            if (_EcsWorld == null)
                return;

            foreach (var entity in _EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End())
            {
                _Target = _EcsWorld.GetPool<CharacterComponent>().Get(entity).GameObject.transform;
            }
        }
    }
}