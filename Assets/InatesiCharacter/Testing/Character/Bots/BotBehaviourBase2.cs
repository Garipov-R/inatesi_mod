using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class BotBehaviorBase2 : MonoBehaviour, IBot
    {
        [SerializeField] protected float _stopDistance = 1f;
        [SerializeField] private float _visibleDistance = 10f;
        [SerializeField] private float _sightRange = 30f;
        [SerializeField] private float _walkPointRange = 5f;
        [SerializeField] private float _UpdatePathfindingTime = 2f;
        [SerializeField] protected float _damage = 30f;
        [SerializeField] protected float _DamageVelocity = 3f;

        public float StopDistance { get => _stopDistance; set => _stopDistance = value; }
        public GameObject Target { get ; set; }
        public GameObject GameObject { get; set; }
        public NavMeshPath NavMeshPath { get; set; }
        public CharacterMotionBase CharacterMotion { get; set; }
        public Transform Transform { get; set; }
        public bool IsEnabled { get; set; }
        public EcsWorld EcsWorld { get; set; }

        protected Vector3 _walkPoint;
        protected bool _walkPointSet;
        private bool _lostTarget;
        private bool _playerInSightRange;
        private float _timeSinceUpdatePathfinding;
        private NavMeshQueryFilter _meshQueryFilter = new NavMeshQueryFilter { areaMask = -1 };
        private bool _attacking;
        private bool _damaged;
        private int _damagedIntData = 1;


        public virtual void UpdateTick()
        {
            Debug();

            if (IsEnabled)
            {
                _timeSinceUpdatePathfinding -= Time.deltaTime;
                if (_timeSinceUpdatePathfinding <= 0)
                {
                    if (Target != null)
                    {
                        BuildPath(Transform.position, Target.transform.position, out Vector3 movePosition);
                    }

                    _timeSinceUpdatePathfinding = _UpdatePathfindingTime;
                    _timeSinceUpdatePathfinding = .1f;
                }
            }
        }

        public virtual void Enabled()
        {

        }

        private void Debug()
        {
            if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 1)
            {
                for (int i = 0; i < NavMeshPath.corners.Length; i++)
                {
                    //UnityEngine.Debug.DrawLine(Transform.position, NavMeshPath.corners[i], Color.red);
                }
            }
        }

        private void StartAttack()
        {
            CharacterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Attack);
            //CharacterMotion.AnimatorMonitor.SetAbilityIntDataParameter(1); 
            //CharacterMotion.AnimatorMonitor.SetAbilityID(0);
            //Attack();

            _attacking = true;
            _damaged = false;
        }

        protected virtual void Attack()
        {
            if (EcsWorld == null) return;

            RaycastHit[] raycastHits;
            raycastHits = new RaycastHit[5];
            var casts = Physics.SphereCastNonAlloc(
                Transform.position + (CharacterMotion.Up * (CharacterMotion.Height / 2)), 
                CharacterMotion.Radius * 2,
                CharacterMotion.transform.forward,
                raycastHits,
                CharacterMotion.Radius, 
                CharacterMotion.RaycastLayer, 
                QueryTriggerInteraction.Ignore
            );

            if (casts > 0)
            {
                foreach (var cast in raycastHits)
                {
                    if (cast.transform == null) continue;
                    if (cast.transform == Transform) continue;
                    if (cast.transform != Target.transform) continue;

                    if (cast.transform.TryGetComponent(out CharacterMotionBase component))
                    {
                        //Debug.Log(cast.transform.name);
                        var hit = EcsWorld.NewEntity();

                        var hitPool = EcsWorld.GetPool<DamageComponent>();
                        hitPool.Add(hit);
                        ref var hitComponent = ref hitPool.Get(hit);

                        var velocityAttack = _DamageVelocity * (cast.transform.position - transform.position).normalized;


                        //hitComponent.first = transform.root.gameObject;
                        //hitComponent.other = cast.transform.gameObject;
                        hitComponent.owner = CharacterMotion.gameObject;
                        hitComponent.target = cast.transform.gameObject;
                        hitComponent.damage = _damage;
                        hitComponent.velocity = velocityAttack;
                        hitComponent.position = cast.point;
                        hitComponent.isHit = true;
                        hitComponent.hit = cast;
                        Shared.ParticlesManager.SendParticleEvent(EcsWorld, hitComponent.hit);
                    }
                }
            }
        }

        private void StopAttack()
        {
            CharacterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Idle);
            _damaged = false;
            _attacking = false;
        }


        public virtual void Damage()
        {
            _lostTarget = true;

            if (Target != null)
            {
                SetWalkPoint(Target.transform.position);
            }

            CharacterMotion.AnimatorMonitor.SetAbilityID(0);
            CharacterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Damage);

            var random = Random.Range(1, 2);
            if (_damagedIntData == random)
            {
                _damagedIntData++;
                if (_damagedIntData > 2)
                {
                    _damagedIntData = 1;
                }
            }
            else
            {
                _damagedIntData = random;
            }

            //CharacterMotion.AnimatorMonitor.SetAbilityIntDataParameter(_damagedIntData);

            _attacking = false;
            _damaged = true;
        }

        public void AfterDamage()
        {
            _attacking = false;
            _damaged = false;

            CharacterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Idle);
        }

        public virtual void Died()
        {
            _attacking = false;
            _damaged = false;

            CharacterMotion.AnimatorMonitor.SetAbilityIntDataParameter(_damagedIntData);
            CharacterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Die);
        }

        protected void SetWalkPoint(Vector3 position)
        {
            _walkPointSet = true;
            _walkPoint = position;
        }


        #region PATH

        protected bool BuildPath(Vector3 sourcePosition, Vector3 targetPosition, out Vector3 movePosition)
        {
            movePosition = Vector3.zero;

            var buildedPath = CalculatePath(sourcePosition, targetPosition);

            if (buildedPath == true)
            {
                if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 1)
                {
                    movePosition = NavMeshPath.corners[1];
                    _walkPointSet = true;
                }
            }
            else
            {
                SamplePosition(
                    targetPosition,
                    Vector3.Distance(targetPosition, Transform.position)* 0 + 10f,
                    out bool tartgetSamplePosition,
                    out NavMeshHit playerSamplePositionHit
                );

                //CLoseEdge(playerSamplePositionHit.position, out bool closeEdge, out NavMeshHit closeEdgeHit);

                if (tartgetSamplePosition)
                {
                    buildedPath = CalculatePath(sourcePosition, playerSamplePositionHit.position);

                    if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 1)
                    {
                        movePosition = NavMeshPath.corners[1];
                        _walkPointSet = Vector3.Distance(targetPosition, Transform.position) < _sightRange;
                    }
                }
                else
                {
                    _walkPointSet = false;
                }
            }

            _walkPoint = movePosition;

            return buildedPath;
        }

        private void SamplePosition(Vector3 targetPosition, float maxDistance , out bool samplePosition, out NavMeshHit samplePositionHit)
        {
            samplePosition = NavMesh.SamplePosition(
                            targetPosition,
                            out samplePositionHit,
                            maxDistance,
                            _meshQueryFilter
                        );
        }

        private void FindCloseEdge(Vector3 sourcePosition, out bool closeEdge, out NavMeshHit closeEdgeHit)
        {
            closeEdge = NavMesh.FindClosestEdge(
                            sourcePosition,
                            out closeEdgeHit,
                            _meshQueryFilter
                        );
        }

        private bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition)
        {
            var state = NavMesh.CalculatePath(
                    sourcePosition,
                    targetPosition,
                    _meshQueryFilter,
                    NavMeshPath
                );

            return NavMeshPath.status == NavMeshPathStatus.PathComplete && state;
        }

        #endregion


        protected Vector2 GetMovePath()
        {
            Vector2 move = Vector2.zero;

            if (Vector3.Distance(_walkPoint, Transform.position) > _stopDistance)
            {
                if (_walkPointSet)
                {
                    var pointy = _walkPoint - Transform.position;
                    pointy.Normalize();
                    move = new Vector2(pointy.x, pointy.z);
                }
                else if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 0)
                {
                    var point = NavMeshPath.corners[1] - Transform.position;
                    point.Normalize();
                    move = new Vector2(point.x, point.z);
                }

                if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 0)
                {
                    var point = NavMeshPath.corners[1] - Transform.position;
                    point.Normalize();
                    move = new Vector2(point.x, point.z);
                }
            }

            if (IsEnabled == false) move = Vector2.zero;


            return move;
        }

        private bool TargetIsVisibleSight()
        {
            if (Target == null)
            {
                return false;
            }

            float distance = Vector3.Distance(Transform.position, Target.transform.position);
            float dot = Vector3.Dot(Transform.forward, Target.transform.position - Transform.position);
            var rot = Transform.position + Transform.rotation * Transform.forward * CharacterMotion.Radius / 2;
            var position = (Transform.position) + CharacterMotion.Up * CharacterMotion.Height;
            bool cast = Physics.Raycast(
                position,
                (Target.transform.position - Transform.position).normalized,
                out RaycastHit hitInfo,
                _sightRange,
                CharacterMotion.RaycastLayer,
                QueryTriggerInteraction.Ignore
            );
            bool isTarget = cast && hitInfo.transform.gameObject == Target;


            return (distance < _sightRange && dot > 0 && isTarget) || distance <= _stopDistance;
        }
    }
}