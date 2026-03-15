using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class BotTest : MonoBehaviour
    {

        [Header("References")]
        private NavMeshAgent agent;
        private Animator animator;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private Transform _target;

        [Header("AI Settings")]
        public float detectionRange = 15f;
        public float attackRange = 3f;
        public float attackCooldown = 2f;
        public float attackForce = 5f;
        public float chaseSpeed = 3.5f;
        public float idleSpeed = 0f; 
        public float wanderRadius = 10f;
        public float transitionToPatrolTime = 2f;
        public float damageTime = 1f;

        [Header("Attack Settings")]
        public int attackDamage = 10;
        public float attackDelay = 0.3f; // Time before damage is applied

        [Header("Settings")]
        [SerializeField] private float _health = 2f;
        [Header("Effects")]
        [SerializeField] private VisualEffect _DamageVisualEffect;
        [Header("Audio")]
        [SerializeField] private AudioClip _OnAwakeAudio;
        [SerializeField] private AudioClip _OnAttackAudio;
        [SerializeField] private AudioClip _OnDamageAudio;
        [SerializeField] private AudioClip _OnDeadAudio;

        private EnemyState currentState;
        private float lastAttackTime;
        private bool isAttacking = false;
        private Vector3 _lastTargetPosition;
        private bool _targetIsVisible = false;
        private RaycastHit _targetHitInfo;
        private float patrolTimer;
        private float wanderTimer;
        private float damageTimer;
        private bool died;
        [Zenject.Inject] private StartEcs _StartEcs;

        public bool TargetIsVisible { get => _targetIsVisible; set => _targetIsVisible = value; }
        public Vector3 TargetPosition { get => _targetIsVisible ? _target.position : _lastTargetPosition; }
        public VisualEffect DamageVisualEffect { get => _DamageVisualEffect; set => _DamageVisualEffect = value; }
        public float Health { get => _health; set => _health = value; }

        private enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Patrol,
            Died
        }

        [Zenject.Inject]
        public void Construct(StartEcs startEcs)
        {
            Debug.Log(startEcs.gameObject.name);
        }

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            //_target = GameObject.FindGameObjectWithTag("Player").transform;

            patrolTimer = transitionToPatrolTime;
            currentState = EnemyState.Idle;
            lastAttackTime = -attackCooldown;
            _lastTargetPosition = transform.position;
        }

        void Update()
        {
            if (_target == null) return;

            if (damageTimer > 0)
            {
                //currentState = EnemyState.Idle;
                damageTimer -= Time.deltaTime;
                return;
            }

            if (died == true)
            {
                currentState = EnemyState.Died;
            }

            CheckTargetIsVisible();

            float distanceToPlayer = Vector3.Distance(transform.position, TargetPosition);

            // State machine
            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleState(distanceToPlayer);
                    break;
                case EnemyState.Chase:
                    UpdateChaseState(distanceToPlayer);
                    break;
                case EnemyState.Attack:
                    UpdateAttackState(distanceToPlayer);
                    break;
                case EnemyState.Patrol:
                    UpdatePatrolState();
                    break;
                case EnemyState.Died:
                    
                    break;
            }

            // Update animations
            UpdateAnimations();
        }

        void UpdateIdleState(float distanceToPlayer)
        {
            agent.speed = idleSpeed;

            // Stop moving
            agent.SetDestination(TargetPosition);

            // Check if player is in detection range
            /*if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chase;
            }*/

            if (_targetIsVisible == true)
            {
                PlayAudio(_OnAwakeAudio);
                currentState = EnemyState.Chase;
            }
            else
            {
                patrolTimer -= Time.deltaTime;

                if (patrolTimer <= 0)
                {
                    currentState = EnemyState.Patrol;
                    wanderTimer = transitionToPatrolTime + 2f;
                }
            }
        }

        void UpdateChaseState(float distanceToPlayer)
        {
            agent.speed = chaseSpeed;

            // Chase the player
            agent.SetDestination(TargetPosition);

            if (distanceToPlayer <= attackRange && _targetIsVisible == true)
            {
                currentState = EnemyState.Attack;
                agent.SetDestination(transform.position); // Stop moving
            }
            // Check if player left detection range
            else if (distanceToPlayer > detectionRange && _targetIsVisible == false)
            {
                PlayAudio(_OnAwakeAudio);
                currentState = EnemyState.Idle;
            }

            if (agent.remainingDistance <= 0.1f)
            {
                if (_targetIsVisible == false)
                {
                    currentState = EnemyState.Idle;
                }
            }
        }

        private void PlayAudio(AudioClip audioClip)
        {
            if (_AudioSource == null)
                return;

            if (audioClip == null)
                return;

            _AudioSource.clip = audioClip;
            _AudioSource.PlayDelayed(.5f);
            //_AudioSource.PlayOneShot(audioClip);
        }

        void UpdateAttackState(float distanceToPlayer)
        {
            agent.speed = 0f;

            // Face the player
            Vector3 direction = (_target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 5f
                );
            }

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    StartCoroutine(PerformAttack());
                }
            }

            if (Time.time < lastAttackTime + attackCooldown)
            {
                return;
            }
            
            // If player moves out of attack range
            if (distanceToPlayer > attackRange || _targetIsVisible == false)
            {
                currentState = EnemyState.Chase;
            }
        }

        void UpdatePatrolState()
        {
            agent.speed = chaseSpeed;
            if (_targetIsVisible)
            {
                PlayAudio(_OnAwakeAudio);
                currentState = EnemyState.Chase;
            }

            wanderTimer -= Time.deltaTime;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Reached destination, can do something here
            }

            if (wanderTimer <= 0)
            {
                SetRandomDestination();
                wanderTimer = transitionToPatrolTime + 2f;
            }
        }

        IEnumerator PerformAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            // Trigger attack animation
            if (animator != null)
            {
                animator.SetTrigger("attack");
            }

            // Wait for attack delay
            yield return new WaitForSeconds(attackDelay);

            // Check if player is still in range
            float distanceToPlayer = Vector3.Distance(transform.position, _target.position);
            if (distanceToPlayer <= attackRange)
            {
                // Apply damage to player
                /*PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }*/

                //Debug.Log("Enemy attacked player!");

                Attack();
            }

            isAttacking = false;
        }

        private void Attack()
        {
            PlayAudio(_OnAttackAudio);

            if (_StartEcs == null) return;

            ref var damageComponent = ref SendDamageEvent.Send(_StartEcs.EcsWorld);

            Ray ray = new(transform.position + transform.up * 1, transform.forward);
            var cast = Physics.Raycast(
                ray, 
                out RaycastHit hitInfo, 
                attackRange, 
                Configs.Config.s_DamageCharacterLayerMask, 
                QueryTriggerInteraction.Ignore
            );

            if (cast)
            {
                damageComponent.damage = 2;
                damageComponent.hit = hitInfo;
                damageComponent.isHit = true;
                damageComponent.ray = ray;
                damageComponent.target = hitInfo.transform.gameObject;
                damageComponent.owner = transform.gameObject;
                damageComponent.velocity = transform.forward * attackForce;
            }
        }

        void UpdateAnimations()
        {
            if (animator != null)
            {
                // Set speed parameter for blend tree
                //animator.SetFloat("Speed", agent.velocity.magnitude);
                animator.SetBool("walk", agent.velocity.magnitude > 1);

                // Set state parameters
                //animator.SetBool("IsChasing", currentState == EnemyState.Chase);
                //animator.SetBool("IsAttacking", currentState == EnemyState.Attack);
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _lastTargetPosition = target.position;
        }

        private bool CheckTargetIsVisible()
        {
            Ray ray = new Ray(transform.position + transform.up * agent.height / 2, (_target.position - transform.position).normalized);
            bool cast = Physics.Raycast(
                ray.origin, 
                ray.direction, 
                out _targetHitInfo,
                detectionRange, 
                LayerMask.NameToLayer("Everything"), 
                QueryTriggerInteraction.Ignore
            );

            if (cast)
            {
                if (_targetHitInfo.transform == _target)
                {
                    _lastTargetPosition = _targetHitInfo.transform.position;
                    _targetIsVisible = true;
                }
                else
                {
                    _targetIsVisible = false;
                }
            }
            else
            {
                _targetIsVisible = false;
            }


            return _targetIsVisible;
        }

        void SetRandomDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        public void Damage()
        {
            if (animator == null) return;

            PlayAudio(_OnDamageAudio);
            animator.SetTrigger("damage");
            agent.speed = 0;
            damageTimer = damageTime;
        }

        public void Die()
        {
            if (animator == null) return;

            animator.SetTrigger("die");

            PlayAudio(_OnDeadAudio);
            currentState = EnemyState.Died;
            agent.speed = 0;
            //GetComponent<Collider>();
            died = true;
        }

        // Visualize ranges in editor
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (_target)
            {
                Ray ray = new Ray(transform.position + transform.up * agent.height / 2, (_target.position - transform.position).normalized);
                if (_targetIsVisible)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(ray.origin, _target.position);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(ray.origin, _lastTargetPosition);
                }
            }
            
        }
    }
}