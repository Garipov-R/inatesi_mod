using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using Leopotam.EcsLite;
using NUnit.Framework.Internal.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class GameLogicSystem : IEcsRunSystem, IEcsInitSystem
    {
        private SharedData _sharedData;
        private IGameLogic _gameLogic;
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
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

        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _gameLogic = _sharedData.GameLogic;
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

            //_gameLogic.StartGame();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerCharacterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(playerCharacterEntity);
                ref var playerComponent = ref _PlayerPool.Get(playerCharacterEntity);

                _gameLogic.PlayerAlive = characterComponent.health > 0;


                foreach (var damageEntity in _DamageFilter)
                {
                    ref var damageC = ref _DamagePool.Get(damageEntity);

                    if (damageC.target == characterComponent.gameObject)
                    {
                        if (characterComponent.health <= 0)
                        {
                            //systems.GetWorld().DelEntity(playerCharacterEntity);
                            _gameLogic.OnPlayerDead();
                        }
                    }
                }
            }

            foreach (var playerInitEntity in _PlayerInitEventFilter)
            {
                ref var playerInitComponent = ref systems.GetWorld().GetPool<PlayerInitEvent>().Get(playerInitEntity);
                ref var playerComponent = ref systems.GetWorld().GetPool<PlayerComponent>().Get(playerInitComponent.entity);
                _gameLogic.OnPlayerSpawn();
                _gameLogic.Player = playerComponent.gameObject;

                ref var playerCharacterComponent = ref systems.GetWorld().GetPool<CharacterComponent>().Get(playerInitComponent.entity);
                playerCharacterComponent.characterMotion.OnLanded.RemoveAllListeners();
                playerCharacterComponent.characterMotion.OnLanded.AddListener(_gameLogic.OnPlayerLanded);
            }

            foreach (var eventEntity in _UseItemEvent)
            {
                ref var useItemComponent = ref _UseItemEventPool.Get(eventEntity);
                _gameLogic.OnPlayerUseItem(useItemComponent.target != null);
            }

            foreach (var eventEntity in _ItemPickupFilter)
            {
                ref var itemPickupEvent = ref _ItemPickupEventPool.Get(eventEntity);
                _gameLogic.OnPlayerPickupItem(itemPickupEvent.itemScriptableObject);
            }

            foreach (var eventEntity in _SelectedItemEventFilter)
            {
                ref var selectedItemEvent = ref _SelectedItemEventPool.Get(eventEntity);
                _gameLogic.OnPlayerSelectItem(selectedItemEvent.itemScriptableObject);
            }

            foreach (var damageEntity in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(damageEntity);
                foreach (var playerCharacterEntity in _PlayerCharacterFilter)
                {
                    ref var playerComponent = ref _PlayerPool.Get(playerCharacterEntity);
                    if (damageComponent.target == playerComponent.gameObject)
                    {
                        _gameLogic.OnPlayerDamage(damageComponent);
                    }
                }
            }

            foreach (var combatEntity in _CombatEventFilter)
            {
                _gameLogic.OnPlayerCombatEvent();
            }

            foreach (var collisionEntity in _CollisionEventFilter)
            {
                ref var col = ref _CollisionEventPool.Get(collisionEntity);
                _gameLogic.OnCameraCollision(col);
            }
        }
    }
}
