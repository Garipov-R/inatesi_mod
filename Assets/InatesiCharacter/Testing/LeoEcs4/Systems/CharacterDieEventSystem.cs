using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.LeoEcs3.Shared;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class CharacterDieEventSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _DieFilter;
        private EcsFilter _OnlyCharacterFilter;
        private EcsFilter _CharacterFilter;
        private EcsPool <CharacterDieEvent> _DiePool;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsFilter _DamageFilter;

        public void Init(IEcsSystems systems)
        {
            _DieFilter = systems.GetWorld().Filter<CharacterDieEvent>().End();
            _DiePool = systems.GetWorld().GetPool<CharacterDieEvent>();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _OnlyCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Exc<PlayerComponent>().End();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var damageEntity in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(damageEntity);
                foreach (var characterEntity in _CharacterFilter)
                {
                    ref var characterComponent = ref _CharacterPool.Get(characterEntity);

                    if (characterComponent.GameObject == damageComponent.target && characterComponent.Health > 0)
                    {
                        //characterComponent.Health -= (int)damageComponent.damage;

                        //characterComponent.OnHealthChanged?.Invoke(characterComponent.Health);
                    }
                }
            }

            foreach (var characterEntity in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                if (characterComponent.Dead == false && characterComponent.Health <= 0)
                {
                    var die = systems.GetWorld().NewEntity();
                    var diePool = systems.GetWorld().GetPool<CharacterDieEvent>();
                    diePool.Add(die);

                    ref var dieComponent = ref diePool.Get(die);
                    dieComponent.entityCharacter = characterEntity;

                    characterComponent.Dead = true;
                }

                if (characterComponent.Dead == true)
                {
                    characterComponent.TimeOfDeath += Time.deltaTime;
                }
            }
            

            

            foreach (var entity in _OnlyCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(entity);

                if (characterComponent.Dead == true)
                {
                    if (characterComponent.TimeOfDeath > characterComponent.CharacterSO.CharacterConfig.TimeAfterDeath)
                    {
                        GameObject.Destroy(characterComponent.GameObject);

                        //Debug.Log("character die " + entity);
                        systems.GetWorld().DelEntity(entity);
                    }
                }
            }
        }
    }
}