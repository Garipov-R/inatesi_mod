using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs4.Events;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class TestSpawnGameObjectSystem : IEcsInitSystem, IEcsRunSystem
    {
        private SharedData _data;

        private int _i;

        public void Init(IEcsSystems systems)
        {
            _data = systems.GetShared<SharedData>();
        }

        public void Run(IEcsSystems systems)
        {
            if (Inatesi.Inputs.Input.Pressed("Attack") && _i == 0)
            {
                ++_i;
                var entity = systems.GetWorld().NewEntity();
                ref var spawnComponent = ref systems.GetWorld().GetPool<SpawnComponentEvent>().Add(entity);
                spawnComponent.data = _data.PlayerCharacterSO;
                spawnComponent.position = _data.SpawnPoint.position; 
                spawnComponent.gameObject = _data.PlayerCharacterSO.Prefab;
                spawnComponent.isPlayer = true;
            }
        }
    }
}