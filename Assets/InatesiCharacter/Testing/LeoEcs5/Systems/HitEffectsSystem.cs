using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class HitEffectsSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsPool<ParticleEvent> _ParticleEventPool;
        private EcsFilter _ParticleEventFilter;

        public void Init(IEcsSystems systems)
        {
            _ParticleEventPool = systems.GetWorld().GetPool<ParticleEvent>();
            Debug.Log(12312);
            _ParticleEventFilter = systems.GetWorld().Filter<ParticleEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _ParticleEventFilter)
            {
                ref var particleEventComponent = ref _ParticleEventPool.Get(entity);

                if (particleEventComponent.hit.transform == null) continue;

                var ray = particleEventComponent.hit.transform != null ? new Ray(particleEventComponent.hit.point + particleEventComponent.hit.normal * 0.01f, -particleEventComponent.hit.normal) : new Ray();
                var hitVisualEffectParticle = SurfaceManager.singleton.GetParticle(
                    ray,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                );
                AudioClip hitAudio = null;
                Material hitDecalMaterial = null;

                var characterLayer = LayerMask.LayerToName(particleEventComponent.hit.transform.gameObject.layer);
                if ("Character" == characterLayer || "Player" == characterLayer || "CharacterHitCollider" == characterLayer)
                {
                    hitVisualEffectParticle = null;
                }
                else
                {
                    hitAudio = SurfaceManager.singleton.GetHitAudio(
                        ray,
                        particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                        particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                    );
                    hitDecalMaterial = SurfaceManager.singleton.GetMaterial(particleEventComponent.ray, particleEventComponent.hit.collider, particleEventComponent.hit.point);
                }

                if (hitVisualEffectParticle != null)
                {
                    SendEventObjectPool.Send(
                        systems.GetWorld(),
                        hitVisualEffectParticle.gameObject,
                        ray.origin,
                        Quaternion.LookRotation(-ray.direction),
                        Pooling.PoolType.Particle
                    );
                }

                if (hitAudio != null)
                {
                    SendEventObjectPool.Send(
                        systems.GetWorld(),
                        systems.GetShared<SharedData>().ParticleSettingsSO.AudioSource.gameObject,
                        ray.origin,
                        Quaternion.LookRotation(-ray.direction),
                        hitAudio,
                        PoolType.Particle
                    );
                }

                if (hitDecalMaterial != null)
                {
                    SendEventObjectPool.Send(
                        systems.GetWorld(),
                        systems.GetShared<SharedData>().ParticleSettingsSO.MeshDecalPrefab.gameObject,
                        particleEventComponent.hit.point - ray.direction * (systems.GetShared<SharedData>().ParticleSettingsSO.MeshDecalPrefab.gameObject.transform.localScale.magnitude / 3),
                        Quaternion.LookRotation(-ray.direction),
                        hitDecalMaterial,
                        particleEventComponent.hit.transform,
                        PoolType.Particle
                    );
                }
            }
        }
    }
}