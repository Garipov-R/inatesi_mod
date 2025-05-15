using InatesiCharacter.Testing.LeoEcs3.Shared;
using InatesiCharacter.Testing.Shared.Components;
using System.Collections;
using UnityEngine;

namespace Assets.InatesiCharacter.Testing.LeoEcs3.Shared
{
    public class DiePlayerCollision : CollisionCheckerView
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            if (ecsWorld == null)
                return;

            var hit = ecsWorld.NewEntity();

            var hitPool = ecsWorld.GetPool<DamageComponent>();
            hitPool.Add(hit);
            ref var hitComponent = ref hitPool.Get(hit);

            hitComponent.owner = transform.root.gameObject;
            hitComponent.target = collision.gameObject;
            hitComponent.damage = 999999;
        }
    }
}