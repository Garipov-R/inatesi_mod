using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.LeoEcs4.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class BotTestSystem2 : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerFilter;
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<PlayerComponent> _PlayerPool;

        public void Init(IEcsSystems systems)
        {
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();

            var bots = GameObject.FindObjectsByType<BotBehaviourBase3>(FindObjectsSortMode.None);

            if (bots != null && bots.Length > 0)
            {
                Debug.Log(bots.Length);

                foreach (var item in bots)
                {
                    item.EcsWorld = systems.GetWorld();
                }
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity  in _PlayerFilter)
            {
                ref var playerComponent = ref _PlayerPool.Get(playerEntity);
            }

            foreach (var playerCharacterEntity  in _PlayerCharacterFilter)
            {
                ref var playerComponent = ref _PlayerPool.Get(playerCharacterEntity);
            }
        }
    }
}
