using InatesiCharacter.Testing.Character.Bot2;
using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class BotsBehaviourSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _BotFilter;
        private EcsFilter _DamageFilter;
        private EcsPool<BotComponent> _BotPool;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsFilter _CharacterFilter;


        public void Init(IEcsSystems systems)
        {
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _BotPool = systems.GetWorld().GetPool<BotComponent>();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();

            var bots = GameObject.FindObjectsByType<InatesiCharacter.Testing.Character.Bot3.BotBehaviourBase>(FindObjectsSortMode.None);
            foreach (var bot in bots)
            {
                var newBotEntity = systems.GetWorld().NewEntity();
                ref var botComponent = ref systems.GetWorld().GetPool<BotComponent>().Add(newBotEntity);
                botComponent.botBehaviourBase = bot;
                botComponent.gameObject = bot.gameObject;

                ref var characterInit = ref systems.GetWorld().GetPool<CharacterInitEvent>().Add(systems.GetWorld().NewEntity());
                characterInit.entity = newBotEntity;
                characterInit.gameObject = botComponent.gameObject;
                characterInit.health = (int)bot.Health;
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
                    ref var botCharacterComponent = ref _CharacterPool.Get(entityBot);

                    if (damageComponent.target != botCharacterComponent.gameObject)
                        continue;

                    if (botComponent.botBehaviourBase.DamageVisualEffect)
                    {
                        SendEventObjectPool.Send(
                            systems.GetWorld(),
                            botComponent.botBehaviourBase.DamageVisualEffect.gameObject,
                            damageComponent.hit.point + -damageComponent.ray.direction * 0.5f,
                            Quaternion.LookRotation(-damageComponent.ray.direction),
                            damageComponent.hit,
                            PoolType.Particle
                        );
                    }

                    if (botComponent.botBehaviourBase.DamageMaterial != null)
                    {
                        ref var objectPoolSendEvent = ref ECSHelper.Create<ObjectPoolSendEvent>(systems.GetWorld());
                        objectPoolSendEvent.objectToSpawn = systems.GetShared<SharedData>().ParticleSettingsSO.MeshDecalPrefab.gameObject;
                        objectPoolSendEvent.poolType = PoolType.Particle;
                        objectPoolSendEvent.data = botComponent.botBehaviourBase.DamageMaterial;

                        var isHit = Physics.Raycast(
                            damageComponent.hit.point,
                            damageComponent.ray.direction,
                            out RaycastHit hit,
                            5f,
                            Configs.Config.s_DefaultLayerMask,
                            QueryTriggerInteraction.Ignore
                        );

                        if (isHit)
                        {
                            objectPoolSendEvent.rotation = Quaternion.LookRotation(damageComponent.ray.direction);
                            objectPoolSendEvent.position = hit.point;
                            objectPoolSendEvent.parent = hit.transform;
                        }
                        else
                        {
                            var isHit2 = Physics.Raycast(
                                damageComponent.hit.point,
                                Vector3.down,
                                out RaycastHit hit2,
                                4f,
                                Configs.Config.s_DefaultLayerMask,
                                QueryTriggerInteraction.Ignore
                            );

                            if (isHit2)
                            {
                                objectPoolSendEvent.rotation = Quaternion.LookRotation(-hit2.normal);
                                objectPoolSendEvent.position = hit2.point;
                                objectPoolSendEvent.parent = hit2.transform;
                            }
                            else
                            {

                            }
                        }
                    }

                    if (botCharacterComponent.health <= 0) 
                    {
                        //botComponent.botBehaviourBase.Die();
                        continue; 
                    }

                    botCharacterComponent.health -= (int)damageComponent.damage;

                    if (botCharacterComponent.health <= 0)
                    {
                        if (systems.GetShared<SharedData>().BotsEvents != null)
                        {
                            foreach(var botEvent in systems.GetShared<SharedData>().BotsEvents)
                            {
                                //botEvent.DeathBot(botCharacterComponent.gameObject);
                            }
                        }
                        
                        botComponent.botBehaviourBase.Die();

                        ref var characterDeadEventComponent = ref ECSHelper.Create<CharacterDeadEvent>(systems.GetWorld());
                        characterDeadEventComponent.entity = entityBot;
                        characterDeadEventComponent.gameObject = botCharacterComponent.gameObject;
                    }
                    else 
                    {
                        botComponent.botBehaviourBase.Damage();

                        var attack = true;

                        foreach (var attackedCharacterEntity in _CharacterFilter)
                        {
                            ref var attackedCharacterComponent = ref _CharacterPool.Get(attackedCharacterEntity);

                            if (damageComponent.owner == attackedCharacterComponent.gameObject)
                            {
                                if (_CharacterPool.Has(entityBot))
                                {
                                    if (attackedCharacterComponent.entityFlag == botCharacterComponent.entityFlag)
                                    {
                                        attack = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (attack)
                        {
                            if (botComponent.botBehaviourBase.IsVisibleTarget(damageComponent.owner.transform))
                            {
                                botComponent.botBehaviourBase.SetTarget(damageComponent.owner.transform);
                            }
                        }
                        
                    }
                }
            }

            foreach (var botEntity in _BotFilter)
            {
                ref var botComponent = ref _BotPool.Get(botEntity);

                if (!_CharacterPool.Has(botEntity))
                    continue;

                ref var botCharacterComponent = ref _CharacterPool.Get(botEntity);

                if (botCharacterComponent.health <= 0f)
                    continue;
                

                foreach (var targetCharacterEntity in _CharacterFilter)
                {
                    if (botEntity == targetCharacterEntity)
                    {
                        continue;
                    }

                    ref var targetCharacterComponent = ref _CharacterPool.Get(targetCharacterEntity);

                    //Debug.Log($"{botCharacterComponent.gameObject.name} {targetCharacterComponent.gameObject.name} {targetCharacterComponent.health}");

                    if (botComponent.botBehaviourBase.CurrentBotState == Character.Bot3.BotState.Chasing)
                    {
                        foreach (var targetEntityFlag in botComponent.botBehaviourBase.TargetsEntityFlag)
                        {
                            if (targetEntityFlag == targetCharacterComponent.entityFlag)
                            {
                                if (botComponent.botBehaviourBase.Target == targetCharacterComponent.transform)
                                {
                                    if (targetCharacterComponent.health <= 0)
                                    {
                                        botComponent.botBehaviourBase.SetTarget(null);
                                    }
                                    else
                                    {
                                        if (botComponent.botBehaviourBase.InRaduisAttack())
                                        {
                                            botComponent.botBehaviourBase.Attack();

                                            break;
                                        }
                                        else
                                        {
                                            botComponent.botBehaviourBase.StopAttack();
                                        }
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    foreach (var target in botComponent.botBehaviourBase.TargetsEntityFlag)
                    {
                        if (botEntity == targetCharacterEntity)
                        {
                            continue;
                        }

                        if (targetCharacterComponent.health <= 0)
                            continue;

                        if (target == targetCharacterComponent.entityFlag)
                        {
                            if (botComponent.botBehaviourBase.IsVisibleTarget(targetCharacterComponent.transform))
                            {
                                botComponent.botBehaviourBase.SetTarget(targetCharacterComponent.transform);
                            }
                        }
                    }
                }
            }
        }
    }
}