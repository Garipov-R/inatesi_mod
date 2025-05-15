using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs4.Events;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class SpawnGameObjectSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _SpawnFilter;
        private EcsPool<SpawnComponentEvent> _SpawnPool;

        public void Init(IEcsSystems systems)
        {
            _SpawnFilter = systems.GetWorld().Filter<SpawnComponentEvent>().End ();
            _SpawnPool = systems.GetWorld().GetPool<SpawnComponentEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _SpawnFilter)
            {
                ref var spawnGameObjectComponent = ref _SpawnPool.Get(entity);

                if (spawnGameObjectComponent.gameObject == null)
                    continue;

                if (spawnGameObjectComponent.data is CharacterSO)
                    continue;

                GameObject.Instantiate(spawnGameObjectComponent.gameObject, spawnGameObjectComponent.position, spawnGameObjectComponent.rotation);
            }
        }
    }
}