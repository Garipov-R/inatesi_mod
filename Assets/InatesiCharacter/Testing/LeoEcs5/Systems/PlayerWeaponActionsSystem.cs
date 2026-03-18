using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Events;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class PlayerWeaponActionsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsPool<CharacterComponent> _PlayerCharacterPool;
        private EcsPool<PlayerInput> _PlayerInputEventPool;
        private EcsFilter _playerInputEventFilter;

        public void Init(IEcsSystems systems)
        {
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _PlayerCharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _playerInputEventFilter = systems.GetWorld().Filter<PlayerInput>().End ();
            _PlayerInputEventPool= systems.GetWorld().GetPool<PlayerInput>();
        }

        public void Run(IEcsSystems systems)
        {
            if (G.IsPause) return;

            foreach (var characterEntity in _CharacterFilter)
            {
                ref var characterComponent = ref _PlayerCharacterPool.Get(characterEntity);
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);

                foreach(var entity in _playerInputEventFilter)
                {
                    ref var p = ref _PlayerInputEventPool.Get(entity);
                    if (p.enable)
                    {
                        continue;
                    }
                }

                
                if (characterComponent.health <= 0) continue;
                if (playerComponent.inputEnabled == false) continue;

                if (characterComponent.InventoryInteraction2 != null)
                {
                    characterComponent.InventoryInteraction2.UpdateTick();
                }
            }
        }
    }
}