using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class BotsSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _BotFilter;
        private EcsFilter _DamageFilter;
        private EcsPool<BotComponent> _BotPool;
        private EcsPool<DamageComponent> _DamagePool;


        public void Init(IEcsSystems systems)
        {
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _BotPool = systems.GetWorld().GetPool<BotComponent>();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();

            var bots = GameObject.FindObjectsByType<BotTest>(FindObjectsSortMode.None);
            foreach (var bot in bots)
            {
                var newBotEntity = systems.GetWorld().NewEntity();
                ref var botComponent = ref systems.GetWorld().GetPool<BotComponent>().Add(newBotEntity);
                botComponent.botTest = bot;
                botComponent.gameObject = bot.gameObject;
                botComponent.health = 2;
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entityDamage in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(entityDamage);

                foreach (var entityBot in _BotFilter)
                {
                    ref var botComponent = ref _BotPool.Get(entityBot);

                    if (damageComponent.target != botComponent.gameObject)
                        continue;

                    if (botComponent.health <= 0) 
                    {
                        //botComponent.botTest.Die();
                        continue; 
                    }

                    botComponent.health -= (int)damageComponent.damage;

                    if (botComponent.health <= 0)
                    {
                        botComponent.botTest.Die();
                    }
                    else 
                    {
                        botComponent.botTest.Damage();
                    }
                }
            }
        }
    }
}