using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerWeaponActionsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsPool<CharacterComponent> _PlayerCharacterPool;
        private EcsPool<PlayerInputEvent> _PlayerInputEventPool;
        private EcsFilter _playerInputEventFilter;

        public void Init(IEcsSystems systems)
        {
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _PlayerCharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _playerInputEventFilter = systems.GetWorld().Filter<PlayerInputEvent>().End ();
            _PlayerInputEventPool= systems.GetWorld().GetPool<PlayerInputEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

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

                
                if (characterComponent.Dead == true) continue;
                if (playerComponent.moveInputEnabled == false) continue;

                characterComponent.InventoryInteraction2.UpdateTick();
            }
        }
    }
}