using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Utility
{
	public static class SendDamageEvent
	{
		public static ref DamageComponent Send(EcsWorld ecsWorld)
		{
            var newDamageEntity = ecsWorld.NewEntity();
            ref var damageComponent = ref ecsWorld.GetPool<DamageComponent>().Add(newDamageEntity);

			return ref damageComponent;
        }
	}
}