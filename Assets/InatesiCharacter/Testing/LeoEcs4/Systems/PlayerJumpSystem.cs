using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerJumpSystem : IEcsInitSystem, IEcsRunSystem
    {
        EcsPool<CharacterComponent> _characterPool;
        EcsPool<PlayerComponent> _playerPool;
        EcsFilter _playerFilter;
        EcsFilter _characterFilter;


        public void Init(IEcsSystems systems)
        {
            _playerFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _playerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _characterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _characterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            foreach (var playerEntity in _playerFilter)
            {
                ref var characterComponent = ref _characterPool.Get(playerEntity);
                ref var playerComponent = ref _playerPool.Get(playerEntity);

                if (characterComponent.Dead == true)
                {
                    return;
                }

                var wishJump = Inatesi.Inputs.Input.Pressed("Jump");
                var wishJumpDown = Inatesi.Inputs.Input.Down("Jump");

                if (GameSettings.IsPause == true)
                {
                    wishJump = false;
                    wishJumpDown = false;
                }

                if ((wishJump || (characterComponent.CharacterMotionBase.MoveConfig.AutoBhop && wishJumpDown)) && characterComponent.CharacterMotionBase.OnGrounded == true)
                {
                    characterComponent.CharacterMotionBase.AddForce(characterComponent.CharacterMotionBase.Up * characterComponent.CharacterMotionBase.MoveConfig.JumpForce);
                    characterComponent.CharacterMotionBase.CharacterFootstep.TryPlayFootstep();
                }
            }
        }
    }
}