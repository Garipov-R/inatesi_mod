using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerNoclipSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerFilter;
        private EcsFilter _CharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;

        public void Init(IEcsSystems systems)
        {
            _PlayerFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            var wishNoclip = Inatesi.Inputs.Input.Pressed("Noclip");
            var input = Inatesi.Inputs.Input.GetVector("Move");
            var wishJumpDown = Inatesi.Inputs.Input.Down("Jump");
            var wishCrouchDown = Inatesi.Inputs.Input.Down("Crouch");

            if (GameSettings.IsPause == true)
            {
                wishNoclip = false;
                input = Vector2.zero;
                wishJumpDown = false;
                wishCrouchDown = false;
            }

            foreach (int entity in _PlayerFilter)
            {
                if (_CharacterPool.Has(entity))
                {
                    ref var characterComponent = ref _CharacterPool.Get(entity);
                    ref var playerComponent = ref _PlayerPool.Get(entity);

                    if (characterComponent.Dead) continue;

                    if (wishNoclip)
                    {
                        playerComponent.noclip = !playerComponent.noclip;
                        if (playerComponent.noclip) characterComponent.CharacterMotionBase.Velocity = Vector3.zero;
                        characterComponent.CharacterMotionBase.IsNoclip = playerComponent.noclip;
                    }

                    var heightVector = (characterComponent.CharacterMotionBase.transform.rotation * characterComponent.CharacterMotionBase.LookSource.Transform.forward * input.y).normalized.y;
                    heightVector = wishJumpDown ? 1 : heightVector;
                    heightVector = wishCrouchDown ? -1 : heightVector;
                    characterComponent.CharacterMotionBase.InputVector3 = new Vector3(0, heightVector, 0);
                }
            }
        }
    }
}