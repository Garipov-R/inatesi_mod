using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class PlayerFixedUpdateSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;

        public void Init(IEcsSystems systems)
        {
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                characterComponent.InventoryInteraction2.UpdateEffectTick();
            }
        }
    }
}