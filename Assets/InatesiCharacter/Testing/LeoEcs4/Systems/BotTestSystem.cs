using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class BotTestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsPool <CharacterComponent> _CharacterPool;
        private EcsFilter _SpawnFilter;
        private EcsPool <SpawnComponentEvent> _SpawnFilterEvent;
        private EcsFilter _BotFilter;
        private EcsFilter _CharacterPlayerFilter;
        private EcsFilter _damageFilter;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsPool<BotComponent> _BotPool;

        public void Init(IEcsSystems systems)
        {
            _CharacterFilter  = systems.GetWorld().Filter<CharacterComponent>().Exc<PlayerComponent>().Exc<BotComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _SpawnFilter  = systems.GetWorld().Filter<SpawnComponentEvent>().End();
            _SpawnFilterEvent = systems.GetWorld().GetPool<SpawnComponentEvent>();
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _CharacterPlayerFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _damageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _BotPool = systems.GetWorld().GetPool<BotComponent>();  
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            foreach (var characterEntity in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);

                if (characterComponent.CharacterSO.BotConfig.BotComponent == null)
                    break;

                ref var botComponent = ref systems.GetWorld().GetPool<BotComponent>().Add(characterEntity);
                botComponent.enable = true;

                var botComponentGameObject = GameObject.Instantiate(characterComponent.CharacterSO.BotConfig.BotComponent);
                botComponentGameObject.transform.SetParent(characterComponent.GameObject.transform);
                botComponentGameObject.transform.localPosition = Vector3.zero;
                botComponentGameObject.transform.localRotation = Quaternion.identity;
                var botBehaviorBase = botComponentGameObject.GetComponent<BotBehaviorBase2>();
                botBehaviorBase.CharacterMotion = characterComponent.CharacterMotionBase;
                botBehaviorBase.GameObject = characterComponent.GameObject;
                botBehaviorBase.Transform = characterComponent.GameObject.transform;
                botBehaviorBase.NavMeshPath = characterComponent.NavMeshPath;
                botBehaviorBase.EcsWorld = systems.GetWorld();
                botComponent.Bot = botBehaviorBase;
                botComponent.Bot.IsEnabled = true;
                botComponent.Bot.Enabled();
            }

            foreach (var damageEntity in _damageFilter)
            {
                ref var hitComponent = ref _DamagePool.Get(damageEntity);
                foreach (var botEntity in _BotFilter)
                {
                    ref var botComponent = ref _BotPool.Get(botEntity);

                    if (hitComponent.target == botComponent.Bot.GameObject)
                    {
                        //if (characterComponent.Die == true) continue;

                        if (systems.GetWorld().GetPool<CharacterComponent>().Has(botEntity) == false)
                            continue;

                        ref var characterComponent = ref systems.GetWorld().GetPool<CharacterComponent>().Get(botEntity);

                        if (characterComponent.Health <= 0)
                        {
                            botComponent.Bot.Died();
                        }
                        else
                        {
                            botComponent.Bot.Damage();
                        }
                    }
                }
            }

            foreach (var botEntity in _BotFilter)
            {
                ref var botComponent = ref _BotPool.Get(botEntity);


                if (_CharacterPool.Has(botEntity) == true)
                {
                    ref var characterComponent = ref _CharacterPool.Get(botEntity);
                    
                    if (characterComponent.Dead == false)
                    {
                        botComponent.Bot.UpdateTick();
                    }
                }
                

                foreach (var player in _CharacterPlayerFilter)
                {
                    ref var playerCharacterComponent = ref _CharacterPool.Get(player);

                    if (playerCharacterComponent.Dead == true)
                    {
                        botComponent.Bot.Target = null;
                    }
                    else
                    {
                        botComponent.Bot.Target = playerCharacterComponent.GameObject;
                    }
                }
            }
        }
    }
}