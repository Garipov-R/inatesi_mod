using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class DamageObserverSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _DamageFilter;
        private EcsPool<DamageComponent> _DamagePool;


        public void Init(IEcsSystems systems)
        {
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(entity);

                if (damageComponent.target.TryGetComponent(out CollisionEvent collisionEvent))
                {
                    collisionEvent.Damage();
                }
            }
        }
    }
}
