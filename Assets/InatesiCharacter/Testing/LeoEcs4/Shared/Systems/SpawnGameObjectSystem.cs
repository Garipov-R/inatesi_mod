using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs3.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3.Shared.Systems
{
    public class SpawnGameObjectSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsWorld _world;
        SharedData _sharedData;
        EcsFilter spawnFilter;
        EcsPool<SpawnGameObjectEvent> spawnPool;


        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>();

            spawnFilter = _world.Filter<SpawnGameObjectEvent>().End();
            spawnPool = _world.GetPool<SpawnGameObjectEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in spawnFilter)
            {
                ref var spawnGameObjectComponent = ref spawnPool.Get(entity);

                if (spawnGameObjectComponent.Target == null)
                    continue;

                if (spawnGameObjectComponent.data is CharacterSO)
                    continue;

                GameObject.Instantiate(spawnGameObjectComponent.Target, spawnGameObjectComponent.Position, spawnGameObjectComponent.Quaternion);
            }
        }
    }
}
