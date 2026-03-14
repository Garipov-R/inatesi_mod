using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.VFX;
using InatesiCharacter.Testing.LeoEcs5.Components;

namespace InatesiCharacter.Testing.Shared
{
    public static class ParticlesManager
    {
        public static void SendParticleEvent(
            EcsWorld ecsWorld,
            RaycastHit raycastHit,  
            float sizeParticle = .4f, 
            float speedParticle = 10f
        )
        {
            var newParticleEventEntity = ecsWorld.NewEntity();
            var particleEventPool = ecsWorld.GetPool<ParticleEvent>();
            particleEventPool.Add(newParticleEventEntity);
            ref var particleEventComponent = ref particleEventPool.Get(newParticleEventEntity);
            particleEventComponent.hit = raycastHit;
            //particleEventComponent.sizeParticle = sizeParticle;
            //particleEventComponent.speedParticle = speedParticle;
            particleEventComponent.ray = new Ray(particleEventComponent.hit.point + particleEventComponent.hit.normal * 0.01f, -particleEventComponent.hit.normal);

        }

        public static void SendParticleEvent(
            EcsWorld ecsWorld,
            Vector3 position,
            Vector3 normal = new Vector3(),
            VisualEffectAsset visualEffectAsset = null
        )
        {
            var newParticleEventEntity = ecsWorld.NewEntity();
            var particleEventPool = ecsWorld.GetPool<ParticleEvent>();
            particleEventPool.Add(newParticleEventEntity);
            ref var particleEventComponent = ref particleEventPool.Get(newParticleEventEntity);
            particleEventComponent.visualEffectAsset = visualEffectAsset;
            particleEventComponent.position = position;
            particleEventComponent.normal = normal;
        }
    }
}
