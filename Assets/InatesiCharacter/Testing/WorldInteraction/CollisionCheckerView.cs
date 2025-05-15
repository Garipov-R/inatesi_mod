using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace InatesiCharacter.Testing.LeoEcs3.Shared
{
    public class CollisionCheckerView : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem _OnTriggerParticleSystem;
        [SerializeField] protected AudioClip _OnTriggerAudioClip;

        [Header("Settings")]
        [SerializeField] private float _DestroyTime = 0f;
        [SerializeField] private UnityEvent _OnDestroy = null;
        [SerializeField] private UnityEvent _OnTriggerEnter = null;


        protected SetupLeoEcs _SetupLeoEcs;
        protected Collider _collider;
        protected AudioSource _OnTriggerAudioSource;

        public EcsWorld ecsWorld { get => _SetupLeoEcs.World;  }


        [Inject]
        protected virtual void Construct(SetupLeoEcs setupLeoEcs)
        {
            _SetupLeoEcs = setupLeoEcs;
        }


        protected void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (_SetupLeoEcs == null) 
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<HitComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.first = transform.root.gameObject;
            hitComponent.other = collision.gameObject;
            hitComponent.isEnter = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_SetupLeoEcs == null)
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<HitComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.first = transform.root.gameObject;
            hitComponent.other = other.gameObject;
            hitComponent.isEnter = true;

            OnTriggerEffect(other.transform.position);

            _OnTriggerEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (_SetupLeoEcs == null)
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<HitComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.first = transform.root.gameObject;
            hitComponent.other = other.gameObject;
            hitComponent.isEnter = false;

            OnTriggerEffect(other.transform.position);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_SetupLeoEcs == null)
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<HitComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.first = transform.root.gameObject;
            hitComponent.other = collision.gameObject;
            hitComponent.isEnter = false;
        }

        protected void Hit(GameObject hitGameObject, bool enter = false, object[] data = null)
        {
            if (_SetupLeoEcs == null)
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<HitComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.first = transform.root.gameObject;
            hitComponent.other = hitGameObject;
            hitComponent.isEnter = enter;
            hitComponent.data = data;
        }


        protected void OnTriggerEffect(Vector3 position)
        {
               
            if (_collider == null) return;

            if (_OnTriggerParticleSystem != null) 
            {
                var newParticle = Instantiate(_OnTriggerParticleSystem, _collider.ClosestPoint(position), Quaternion.identity);
                Destroy(newParticle.gameObject, 5f);
            }
            

            if (_OnTriggerAudioSource != null && _OnTriggerAudioClip != null)
            {
                _OnTriggerAudioSource.PlayOneShot(_OnTriggerAudioClip);
            }
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);

            _OnDestroy?.Invoke();
        }

        public virtual void OnHit(float damageAmount = 0)
        {

        }

        private void OnDestroy()
        {
            _OnDestroy = null;  
        }
    }
}