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

            _gameLogic.StartGame();
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
                            systems.GetWorld().DelEntity(playerCharacterEntity);
                            _gameLogic.OnPlayerDead();
                        }
                    }
                }
            }

            foreach (var playerInitEntity in _PlayerInitEventFilter)
            {
                _gameLogic.StartGame();
            }
        }
    }
}
