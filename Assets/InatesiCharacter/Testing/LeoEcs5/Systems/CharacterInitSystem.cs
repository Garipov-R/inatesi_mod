using InatesiCharacter.Testing.Character.Bot3;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class CharacterInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterInitEventFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<CharacterInitEvent> _CharacterInitEventPool;

        public void Init(IEcsSystems systems)
        {
            _CharacterInitEventFilter = systems.GetWorld().Filter<CharacterInitEvent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _CharacterInitEventPool = systems.GetWorld().GetPool<CharacterInitEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var characterInitEventEntity in _CharacterInitEventFilter)
            {
                ref var characterInitComponent = ref _CharacterInitEventPool.Get(characterInitEventEntity);
                

                // character component setup ================================
                ref var characterComponent = ref _CharacterPool.Add(characterInitComponent.entity);
                characterComponent.transform = characterInitComponent.gameObject.transform;
                characterComponent.gameObject = characterInitComponent.gameObject;
                characterComponent.health = characterInitComponent.health;
                characterComponent.entity = characterInitComponent.entity;
                if (characterComponent.gameObject.TryGetComponent(out EntityFlagMono entityFlagMono))
                {
                    characterComponent.entityFlag = entityFlagMono.EntityFlag;
                }
                // ================================================================
            }
        }
    }
}
