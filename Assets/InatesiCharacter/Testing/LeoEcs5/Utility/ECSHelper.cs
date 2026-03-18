using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Utility
{
    public static class ECSHelper
    {
        public static ref T Create<T>(EcsWorld ecsWorld) where T : struct
        {
            var newDamageEntity = ecsWorld.NewEntity();
            ref var component = ref ecsWorld.GetPool<T>().Add(newDamageEntity);

            return ref component;
        }

        public static ref T Get<T>(EcsWorld ecsWorld, EcsFilter filter = null) where T : struct
        {
            if (filter == null)
            {
                filter = ecsWorld.Filter<T>().End();
            }
            
            foreach (var entity in filter)
            {
                ref var component = ref ecsWorld.GetPool<T>().Get(entity);
                return ref component;
            }
            
            return ref ecsWorld.GetPool<T>().Get(1);
        }
    }
}
