using InatesiCharacter.Testing.Character.Bot3;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class BotInitSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _BotInitEventFilter;
        private EcsPool<BotInitEvent> _BotInitEventPool;

        public void Init(IEcsSystems systems)
        {
            _BotInitEventFilter = systems.GetWorld().Filter<BotInitEvent>().End();
            _BotInitEventPool = systems.GetWorld().GetPool<BotInitEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var botInitEntity in _BotInitEventFilter)
            {
                ref var botInitComponent = ref _BotInitEventPool.Get(botInitEntity);
                var spawnedBot = GameObject.Instantiate(botInitComponent.prefab, botInitComponent.position, botInitComponent.rotation);

                var newBotEntity = systems.GetWorld().NewEntity();
                ref var botComponent = ref systems.GetWorld().GetPool<BotComponent>().Add(newBotEntity);
                botComponent.botBehaviourBase = spawnedBot.GetComponent<BotBehaviourBase>();
                botComponent.gameObject = spawnedBot.gameObject;

                ref var characterInit = ref systems.GetWorld().GetPool<CharacterInitEvent>().Add(systems.GetWorld().NewEntity());
                characterInit.entity = newBotEntity;
                characterInit.gameObject = botComponent.gameObject;
                characterInit.health = (int)botComponent.botBehaviourBase.Health;
            }
        }
    }
}
