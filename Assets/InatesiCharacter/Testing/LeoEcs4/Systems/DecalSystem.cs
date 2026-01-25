using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Leopotam.EcsLite;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Decals;
using InatesiCharacter.Testing.LeoEcs4.Components;
using UnityEngine.ProBuilder;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class DecalSystem : IEcsInitSystem, IEcsRunSystem
    {
        //private EcsFilter 
        private EcsFilter _ParticleEventFilter;
        private EcsPool<ParticleEvent> _ParticleEventPool;
        private SharedData _SharedData;
        private IObjectPool<MeshDecal> _DecalPool;


        public void Init(IEcsSystems systems)
        {
            _SharedData = systems.GetShared<SharedData>();

            _ParticleEventFilter = systems.GetWorld().Filter<ParticleEvent>().End();
            _ParticleEventPool = systems.GetWorld().GetPool<ParticleEvent>();

            _DecalPool = new ObjectPool<MeshDecal>(
                CreateDecal,
                OnGetDecal,
                OnReleaseDecal,
                OnDestroyDecal,
                _SharedData.ParticleSettingsSO.CollectionCheck,
                _SharedData.ParticleSettingsSO.DefaultCapacity,
                _SharedData.ParticleSettingsSO.MaxSize
            );
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _ParticleEventFilter)
            {
                ref var particleEventComponent = ref _ParticleEventPool.Get(entity);

                _DecalPool.Get(out MeshDecal decal);

                var material = SurfaceManager.singleton.GetMaterial(particleEventComponent.ray, particleEventComponent.hit.collider, particleEventComponent.hit.point);
                decal.transform.rotation = Quaternion.LookRotation(-particleEventComponent.hit.normal);
                decal.transform.position = particleEventComponent.hit.point - decal.transform.forward * (decal.transform.localScale.magnitude / 2);
                decal.targetMesh = particleEventComponent.hit.transform;
                decal.material = material;
                decal.Recalculate();
            }
        }

        private MeshDecal CreateDecal()
        {
            var decal = GameObject.Instantiate(_SharedData.ParticleSettingsSO.MeshDecalPrefab);

            return decal;
        }

        private void OnGetDecal(MeshDecal meshDecal)
        {
            meshDecal.gameObject.SetActive(true);
        }

        private void OnReleaseDecal(MeshDecal meshDecal)
        {
            meshDecal.gameObject.SetActive(false);
        }

        private void OnDestroyDecal(MeshDecal meshDecal)
        {
            GameObject.Destroy(meshDecal.gameObject);
        }
    }
}