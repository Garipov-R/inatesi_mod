using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.Testing.Props;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3.Shared.Systems
{
    public class DamageObserverSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter _damageFilter;
        EcsPool<DamageComponent> _damagePool;

        public void Init(IEcsSystems systems)
        {
            _damageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _damagePool = systems.GetWorld().GetPool<DamageComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _damageFilter) 
            {
                ref var damageComponent = ref _damagePool.Get(entity);

                if (damageComponent.target.TryGetComponent(out Prop prop))
                {
                    prop.OnHit();
                }
            }
        }
    }
}
