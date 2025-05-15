using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class CharacterFixedUpdateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;


        public void Init(IEcsSystems systems)
        {
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            foreach (var character in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(character);

                characterComponent.CharacterMotionBase.FixedUpdateCharacterMethod();
            }
        }
    }
}