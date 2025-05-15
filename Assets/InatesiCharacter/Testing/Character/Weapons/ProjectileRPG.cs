using InatesiCharacter.Movements.SourceEngine;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs3.Shared;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using Zenject;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public class ProjectileRPG : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private float _damage = 100f;
        [SerializeField] private float _explosionRadius = 10f;
        [SerializeField] private float _explosionForce = 10f;
        [SerializeField] private float _UpwardsModifier = 3f;
        [SerializeField] private bool _isKinematic = false;
        [SerializeField] private ForceMode _ForceMode = ForceMode.Impulse;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Renderer _renderer;
        [SerializeField][Min(0)] private float _ExplosionTimer = 5f;

        private bool _exploded;
        private bool _startTimer;
        private float _SpawnedTimeSince;
        private SetupLeoEcs _SetupLeoEcs;
        public EcsWorld ecsWorld { get => _SetupLeoEcs.World; }


        [Inject]
        protected void Construct(SetupLeoEcs setupLeoEcs)
        {
            _SetupLeoEcs = setupLeoEcs;
        }

        private void Update()
        {
            if (_startTimer == false) return;

            if (_exploded == false)
            {
                _SpawnedTimeSince += Time.deltaTime;
                if (_ExplosionTimer <= _SpawnedTimeSince)
                {
                    Explosion();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_ExplosionTimer > 0 || _exploded) return;

            Explosion();
        }

        protected void OnCollisionEnter(Collision collision)
        {
            _startTimer = true;

            if (collision.gameObject.TryGetComponent(out CharacterMotionBase component))
            {
                Explosion();
            }

            if (_ExplosionTimer > 0 || _exploded) return;

            Explosion();
        }

        private void Explosion()
        {
            _exploded = true;

            if (_AudioSource != null)
            {
                _AudioSource.Play();

                GetComponent<Rigidbody>().isKinematic = _isKinematic;
                if (_isKinematic)
                {
                    GetComponent<Collider>().enabled = false;
                    GetComponent<Renderer>().enabled = false;
                }

                var casts = Physics.SphereCastAll(
                    transform.position,
                    _explosionRadius,
                    Vector3.up,
                    .2f,
                    LayerMask.NameToLayer("Everything"),
                    QueryTriggerInteraction.Ignore
                );

                foreach (var cast in casts)
                {
                    var directionForce = (cast.point - transform.position).normalized * _explosionForce;
                    var damage = _damage; 
                    var distance = Vector3.Distance(cast.transform.position, transform.position);
                    var force = Mathf.Abs(_explosionRadius - distance);
                    damage *= force / _explosionRadius;

                    if (cast.transform.TryGetComponent(out CharacterMotionBase characterMotionBase))
                    {
                        directionForce = (characterMotionBase.Up * _UpwardsModifier) + (cast.point - transform.position).normalized * _explosionForce;
                        directionForce = directionForce * force;
                    }

                    var hit = ecsWorld.NewEntity();

                    var hitPool = ecsWorld.GetPool<DamageComponent>();
                    hitPool.Add(hit);
                    ref var hitComponent = ref hitPool.Get(hit);

                    hitComponent.owner = null;
                    hitComponent.target = cast.transform.gameObject;
                    hitComponent.damage = damage;
                    hitComponent.velocity = directionForce;
                    hitComponent.weaponType = this.GetType();


                    if (cast.rigidbody != null)
                        cast.rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _UpwardsModifier, _ForceMode);
                }


                if (_particleSystem != null)
                {
                    var particle = Instantiate(_particleSystem.gameObject, transform.position, Quaternion.identity);
                    Destroy(particle, 10f);
                }

                Destroy(gameObject, _AudioSource.clip.length);

                if (_renderer)
                {
                    _renderer.enabled = false;
                }
            }
        }

        private void OnDestroy()
        {
            //var particle = Instantiate(_particleSystem.gameObject, transform.position, Quaternion.identity);
            //Destroy(particle, 10f);
        }
    }
}