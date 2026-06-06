using InatesiCharacter.Testing.Character.Bot2;
using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class BotsBehaviourSystem : IEcsRunSystem, IEcsInitSystem
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

            var bots = GameObject.FindObjectsByType<InatesiCharacter.Testing.Character.Bot3.BotBehaviourBase>(FindObjectsSortMode.None);
            foreach (var bot in bots)
            {
                var newBotEntity = systems.GetWorld().NewEntity();
                ref var botComponent = ref systems.GetWorld().GetPool<BotComponent>().Add(newBotEntity);
                botComponent.botBehaviourBase = bot;
                botComponent.gameObject = bot.gameObject;
                botComponent.health = (int)bot.Health;
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

                    if (botComponent.botBehaviourBase.DamageVisualEffect)
                    {
                        SendEventObjectPool.Send(
                            systems.GetWorld(),
                            botComponent.botBehaviourBase.DamageVisualEffect.gameObject,
                            damageComponent.hit.point,
                            Quaternion.LookRotation(-damageComponent.ray.direction),
                            damageComponent.hit,
                            PoolType.Particle
                        );
                    }

                    if (botComponent.health <= 0) 
                    {
                        //botComponent.botTest.Die();
                        continue; 
                    }

                    botComponent.health -= (int)damageComponent.damage;

                    if (botComponent.health <= 0)
                    {
                        if (systems.GetShared<SharedData>().BotsEvents != null)
                        {
                            foreach(var botEvent in systems.GetShared<SharedData>().BotsEvents)
                            {
                                botEvent.DeathBot(botComponent.gameObject);
                            }
                        }
                        
                        botComponent.botBehaviourBase.Die();
                    }
                    else 
                    {
                        botComponent.botBehaviourBase.Damage();
                    }
                }
            }
        }
    }
}