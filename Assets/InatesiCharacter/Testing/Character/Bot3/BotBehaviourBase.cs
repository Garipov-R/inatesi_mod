using _Demonstration.bot_test;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Bot3
{
    public class BotBehaviourBase : MonoBehaviour
    {
        [SerializeField] private int _Health = 1;
        [SerializeField] private Animator _Animator;
        [SerializeField] private EntityFlag[] _TargetEntityFlag;
        [SerializeField] private _Demonstration.bot_test.AdvancedNavMeshBot _advancedNavMeshBot;
        [SerializeField] private float _Damage = 1;
        [SerializeField] private GameObject _hitCollidersContainer;
        [SerializeField] private GameObject _collidersContainer;

        [Header("Effects")]
        [SerializeField] private VisualEffect _DamageVisualEffect;
        [SerializeField] private Material _DamageMaterial;

        private float _stopTimer;
        private bool _isDead;

        public int Health { get => _Health; set => _Health = value; }
        public VisualEffect DamageVisualEffect { get => _DamageVisualEffect; set => _DamageVisualEffect = value; }
        public Animator Animator { get => _Animator; set => _Animator = value; }
        public EntityFlag[] TargetsEntityFlag { get => _TargetEntityFlag; set => _TargetEntityFlag = value; }
        public AdvancedNavMeshBot AdvancedNavMeshBot { get => _advancedNavMeshBot; set => _advancedNavMeshBot = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }
        [Zenject.Inject] public StartEcs StartEcs { get; set; }
        public Material DamageMaterial { get => _DamageMaterial; set => _DamageMaterial = value; }

        private void Update()
        {
            if (G.IsPause)
                return;

            if (_isDead)
                return;

            if (_stopTimer <= 0)
            {
                Go();
            }
            else
            {
                _stopTimer -= Time.deltaTime;
            }

            RotateToTarget();
        }

        public void Damage()
        {
            _Animator.SetTrigger("damage");

            _advancedNavMeshBot.Stop = true;

            _stopTimer += 0.5f;
        }

        public void Go()
        {
            _advancedNavMeshBot.Stop = false;
        }

        public void Die()
        {
            GetComponent<NavMeshAgent>().enabled = false;
            _advancedNavMeshBot.Stop = true;
            _isDead = true;
            _Animator.ResetTrigger("attack");
            _Animator.SetTrigger("death");

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
            _Animator.SetTrigger("attack");
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
                damageComponent.velocity = ray.direction * 8f;
            }   

            damageComponent.isHit = cast; 
            
            Shared.ParticlesManager.SendParticleEvent(StartEcs.EcsWorld, damageComponent.hit);

        }

        public bool InRaduisAttack()
        {
            if (_advancedNavMeshBot.Target && Vector3.Distance(_advancedNavMeshBot.Target.position, transform.position) < 2f)
            {
                var dot = Vector3.Dot(transform.position, _advancedNavMeshBot.Target.position);
                
                if (Mathf.Abs(dot) > 0.1f)
                {
                    return true;
                }
            }

            return false;
        }

        public void RotateToTarget()
        {
            if (_advancedNavMeshBot.Target == null)
                return;

            var targetPosition = _advancedNavMeshBot.Target.position - transform.position;
            targetPosition.y = 0;
            var targetRotation = Quaternion.LookRotation(targetPosition, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}