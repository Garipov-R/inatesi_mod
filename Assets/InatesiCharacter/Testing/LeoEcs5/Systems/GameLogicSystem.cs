using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using Leopotam.EcsLite;
using NUnit.Framework.Internal.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class GameLogicSystem : IEcsRunSystem, IEcsInitSystem
    {
        private SharedData _sharedData;
        private List<IGameLogic> _gameLogics = new();
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsFilter _CharacterFilter;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsFilter _DamageFilter;
        private EcsFilter _PlayerInitEventFilter;
        private EcsFilter _BotFilter;
        private EcsFilter _UseItemEvent;
        private EcsPool<UseItemEvent> _UseItemEventPool;
        private EcsPool<ItemPickupEvent> _ItemPickupEventPool;
        private EcsFilter _ItemPickupFilter;
        private EcsPool<SelectedItemEvent> _SelectedItemEventPool;
        private EcsFilter _SelectedItemEventFilter;
        private EcsPool<CombatEvent> _CombatEventPool;
        private EcsFilter _CombatEventFilter;
        private EcsFilter _CollisionEventFilter;
        private EcsPool<CollisionComponentEvent> _CollisionEventPool;
        private EcsFilter _ShootEventFilter;
        private EcsPool<ShootEvent> _ShootEventPool;
        private EcsFilter _CharacterDeadEventFilter;
        private EcsPool<CharacterDeadEvent> _CharacterDeadEventPool;

        public void Init(IEcsSystems systems)
        {
            var gameLogicList = GameObject.FindObjectsByType<GameLogicBase>(FindObjectsSortMode.None);
            foreach (var i in gameLogicList)
            {
                _gameLogics.Add(i);
            }

            _sharedData = systems.GetShared<SharedData>();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _PlayerInitEventFilter = systems.GetWorld().Filter<PlayerInitEvent>().End();
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _UseItemEvent = systems.GetWorld().Filter<UseItemEvent>().End();
            _UseItemEventPool = systems.GetWorld().GetPool<UseItemEvent>();
            _ItemPickupFilter = systems.GetWorld().Filter<ItemPickupEvent>().End();
            _ItemPickupEventPool = systems.GetWorld().GetPool<ItemPickupEvent>();
            _SelectedItemEventFilter = systems.GetWorld().Filter<SelectedItemEvent>().End();
            _SelectedItemEventPool = systems.GetWorld().GetPool<SelectedItemEvent>();
            _CombatEventPool = systems.GetWorld().GetPool<CombatEvent>();
            _CombatEventFilter = systems.GetWorld().Filter<CombatEvent>().End();
            _CollisionEventFilter = systems.GetWorld().Filter<CollisionComponentEvent>().End();
            _CollisionEventPool = systems.GetWorld().GetPool<CollisionComponentEvent>();
            _ShootEventFilter = systems.GetWorld().Filter<ShootEvent>().End();
            _ShootEventPool = systems.GetWorld().GetPool<ShootEvent>();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterDeadEventPool = systems.GetWorld().GetPool<CharacterDeadEvent>();
            _CharacterDeadEventFilter = systems.GetWorld().Filter<CharacterDeadEvent>().End();

            //_gameLogic.StartGame();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerCharacterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(playerCharacterEntity);
                ref var playerComponent = ref _PlayerPool.Get(playerCharacterEntity);

                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.PlayerAlive = characterComponent.health > 0;
                }

                foreach (var damageEntity in _DamageFilter)
                {
                    ref var damageC = ref _DamagePool.Get(damageEntity);

                    if (damageC.target == characterComponent.gameObject)
                    {
                        if (characterComponent.health <= 0)
                        {
                            //systems.GetWorld().DelEntity(playerCharacterEntity);

                            foreach (var gameLogic in _gameLogics)
                            {
                                gameLogic.OnPlayerDead();
                            }

                            characterComponent.characterMotion.OnLanded.RemoveAllListeners();
                        }
                    }
                }
            }

            foreach (var playerInitEntity in _PlayerInitEventFilter)
            {
                ref var playerInitComponent = ref systems.GetWorld().GetPool<PlayerInitEvent>().Get(playerInitEntity);

                if (systems.GetWorld().GetPool<PlayerComponent>().Has(playerInitComponent.entity) == false)
                    continue;

                ref var playerComponent = ref systems.GetWorld().GetPool<PlayerComponent>().Get(playerInitComponent.entity);

                ref var playerCharacterComponent = ref systems.GetWorld().GetPool<CharacterComponent>().Get(playerInitComponent.entity);
                playerCharacterComponent.characterMotion.OnLanded.RemoveAllListeners();

                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerSpawn();
                    gameLogic.Player = playerComponent.gameObject;
                    playerCharacterComponent.characterMotion.OnLanded.AddListener(gameLogic.OnPlayerLanded);
                }
            }

            foreach (var eventEntity in _UseItemEvent)
            {
                ref var useItemComponent = ref _UseItemEventPool.Get(eventEntity);

                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerUseItem(useItemComponent.target != null);
                }
            }

            foreach (var eventEntity in _ItemPickupFilter)
            {
                ref var itemPickupEvent = ref _ItemPickupEventPool.Get(eventEntity);

                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerPickupItem(itemPickupEvent.itemScriptableObject);
                }
            }

            foreach (var eventEntity in _SelectedItemEventFilter)
            {
                ref var selectedItemEvent = ref _SelectedItemEventPool.Get(eventEntity);

                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerSelectItem(selectedItemEvent.itemScriptableObject);
                }
            }

            foreach (var damageEntity in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(damageEntity);
                foreach (var playerCharacterEntity in _PlayerCharacterFilter)
                {
                    ref var playerComponent = ref _PlayerPool.Get(playerCharacterEntity);
                    if (damageComponent.target == playerComponent.gameObject)
                    {
                        foreach (var gameLogic in _gameLogics)
                        {
                            gameLogic.OnPlayerDamage(damageComponent);
                        }
                    }
                }

                foreach (var characterEntity in _CharacterFilter)
                {
                    ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                    if (damageComponent.target == characterComponent.gameObject)
                    {
                        foreach (var gameLogic in _gameLogics)
                        {
                            gameLogic.OnCharacterDamage(characterComponent, damageComponent);
                        }
                    }
                }
            }

            foreach (var combatEntity in _CombatEventFilter)
            {
                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerAimEvent();
                }
            }

            foreach (var shootEntity in _ShootEventFilter)
            {
                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnPlayerShoot();
                }
            }

            foreach (var collisionEntity in _CollisionEventFilter)
            {
                ref var col = ref _CollisionEventPool.Get(collisionEntity);
                foreach (var gameLogic in _gameLogics)
                {
                    gameLogic.OnCameraCollision(col);
                }
            }

            foreach (var characterDeadEntity in _CharacterDeadEventFilter)
            {
                ref var characterDeadEntityComponent = ref _CharacterDeadEventPool.Get(characterDeadEntity);

                if (_CharacterPool.Has(characterDeadEntityComponent.entity))
                {
                    ref var characterComponent = ref _CharacterPool.Get(characterDeadEntityComponent.entity);

                    foreach (var gameLogic in _gameLogics)
                    {
                        gameLogic.OnCharacterDead(characterComponent);
                    }
                }
            }
        }
    }
}
