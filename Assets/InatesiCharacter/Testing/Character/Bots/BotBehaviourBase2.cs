using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.Character.Abilities;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Zenject;

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

        private Vector3 _walkPoint;
        private bool _walkPointSet;
        private bool _lostTarget;
        private bool _playerInSightRange;
        private bool _playerInAttackRange;
        private float _timeSinceUpdatePathfinding;
        private float _timeSinceJump = 3;
        private NavMeshQueryFilter meshQueryFilter = new NavMeshQueryFilter { areaMask = -1 };
        private bool _attacking;
        private bool _damaged;
        private int _damagedIntData = 1;


        public virtual void UpdateTick()
        {


            if (IsEnabled)
            {
                _timeSinceUpdatePathfinding -= Time.deltaTime;
                if (_timeSinceUpdatePathfinding <= 0 )
                {
                    Patroling();

                    _timeSinceUpdatePathfinding = _UpdatePathfindingTime;
                }
            }

            if (Target != null)
            {
                SetWalkPoint(Target.transform.position);
            }

            Vector2 move = GetMovePath();


            if (_attacking == false && _damaged == false)
            {
                CharacterMotion.Move(move);
            }
            else
            {
                CharacterMotion.Move(Vector2.zero);
            }

            if (Vector3.Distance(_walkPoint, transform.position) < _stopDistance)
            {
                StartAttack();
            }
            else
            {
                if (_attacking == false)
                {
                    //StopAttack();
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

            Ray ray = new(
                transform.position + (2 * CharacterMotion.Radius * transform.forward) + (CharacterMotion.Up * (CharacterMotion.Height /2 )), 
                transform.forward
            );
            var casts = Physics.SphereCastAll(
                ray, 
                CharacterMotion.Radius * 1, 
                CharacterMotion.Radius, 
                CharacterMotion.RaycastLayer, 
                QueryTriggerInteraction.Ignore
            );

            if (casts != null && casts.Length > 0)
            {
                foreach (var cast in casts)
                {
                    if (cast.transform == transform) continue;
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

        private void LifeOfBot()
        {
            NavMesh.pathfindingIterationsPerFrame = 50;

            _playerInSightRange = TargetIsVisibleSight();

            if (!_playerInSightRange && !_playerInAttackRange && !_lostTarget)
            {
                _lostTarget = true;

                if (Target != null)
                {
                    //SetWalkPoint(Target.transform.position);
                }
            }

            //if (!_playerInSightRange && !_playerInAttackRange) Patroling();
            if (_playerInSightRange && !_playerInAttackRange)
            {
                _lostTarget = false;

                if (Target != null)
                {
                    SetWalkPoint(Target.transform.position);
                }
            }

            _timeSinceUpdatePathfinding -= Time.deltaTime;
            if (_timeSinceUpdatePathfinding <= 0 && _playerInSightRange == false && _lostTarget)
            {
                _timeSinceUpdatePathfinding = _UpdatePathfindingTime;
                if (Target != null)
                {
                    if (BuildPath(Target.transform.position, Transform.position, out Vector3 movePosition))
                    {
                        SetWalkPoint(movePosition);
                    }
                } 
            }
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

            CharacterMotion.AnimatorMonitor.SetAbilityIntDataParameter(_damagedIntData);

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

        protected void Patroling()
        {
            if (!_walkPointSet) RandomWalkPoint();


            Vector3 distanceToWalkPoint = Transform.position - _walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                _walkPointSet = false;
        }

        private void RandomWalkPoint()
        {
            //Calculate random point in range
            float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
            float randomX = Random.Range(-_walkPointRange, _walkPointRange);

            var point = new Vector3(Transform.position.x + randomX, Transform.position.y, Transform.position.z + randomZ);

            //if (CharacterMotion.OnGrounded == true && BuildPath(point, Transform.position, out Vector3 movePosition))
            if (CharacterMotion.OnGrounded == true)
            {
                SetWalkPoint(point);
            }
        }


        protected void SetWalkPoint(Vector3 position)
        {
            _walkPointSet = true;
            _walkPoint = position;
        }

        public Vector2 PathInput(Vector3 targetPosition, Vector3 botPosition, NavMeshPath navMeshPath, float distance = 2)
        {
            return Vector2.zero;
        }

        protected bool BuildPath(Vector3 targetPosition, Vector3 botPosition, out Vector3 movePosition)
        {
            movePosition = botPosition;

            var meshQueryFilter = new NavMeshQueryFilter
            {
                areaMask = -1
            };

            var buildedPath = IsPathComplete(botPosition, targetPosition);




            if (buildedPath == true)
            {
                if (NavMeshPath.corners != null && NavMeshPath.corners.Length > 1)
                {
                    movePosition = NavMeshPath.corners[1];
                }
            }
            else
            {
                SamplePosition(
                    targetPosition,
                    Vector3.Distance(targetPosition, Transform.position),
                    out bool playerSamplePosition,
                    out NavMeshHit playerSamplePositionHit
                );

                if (playerSamplePosition)
                {
                    buildedPath = IsPathComplete(botPosition, playerSamplePositionHit.position);

                    movePosition = playerSamplePositionHit.position;
                }
            }

            return buildedPath;
        }

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

            if (IsEnabled == false ) move = Vector2.zero;
           

            return move;    
        }



        private void SamplePosition(Vector3 botPosition, float maxDistance , out bool samplePosition, out NavMeshHit samplePositionHit)
        {
             
            samplePosition = NavMesh.SamplePosition(
                            botPosition,
                            out samplePositionHit,
                            maxDistance,
                            meshQueryFilter
                        );
        }

        private void CLoseEdge(Vector3 position, out bool closeEdge, out NavMeshHit closeEdgeHit)
        {
            closeEdge = NavMesh.FindClosestEdge(
                            position,
                            out closeEdgeHit,
                            meshQueryFilter
                        );
        }

        private bool IsPathComplete(Vector3 sourcePosition, Vector3 targetPosition)
        {
            var meshQueryFilter = new NavMeshQueryFilter
            {
                areaMask = -1
            };

            var state = NavMesh.CalculatePath(
                    sourcePosition,
                    targetPosition,
                    meshQueryFilter,
                    NavMeshPath
                );

            return NavMeshPath.status == NavMeshPathStatus.PathComplete && state;
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
            var position = (Transform.position) + CharacterMotion.Up * CharacterMotion.Height ;
            bool cast = Physics.Raycast(
                position,
                (Target.transform.position - Transform.position).normalized,
                out RaycastHit hitInfo,
                _sightRange,
                CharacterMotion.RaycastLayer,
                QueryTriggerInteraction.Ignore
            );
            bool isPlayer = cast && hitInfo.transform.gameObject == Target;


            return (distance < _sightRange && dot > 0 && isPlayer) || distance <= _stopDistance;
        }

        public virtual void Enabled()
        {
        }
    }
}