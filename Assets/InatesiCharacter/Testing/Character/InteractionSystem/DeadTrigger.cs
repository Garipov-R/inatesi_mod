using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using System.Collections;
using UnityEngine;
using Zenject;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class DeadTrigger : MonoBehaviour
    {
        [Inject] private StartEcs _StartEcs;

        private void OnTriggerEnter(Collider other)
        {
            if (_StartEcs != null)
            {
                try
                {
                    ref var playerCharacterComponent = ref ECSHelper.Get<CharacterComponent>(
                        _StartEcs.EcsWorld,
                        _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End()
                    );

                    if (playerCharacterComponent.gameObject == other.gameObject)
                    {
                        if (playerCharacterComponent.health > 0)
                        {
                            ref var damageComponent = ref ECSHelper.Create<DamageComponent>(_StartEcs.EcsWorld);
                            damageComponent.damage = 999999;
                            damageComponent.target = playerCharacterComponent.gameObject;
                        }
                    }
                }
                catch
                {

                }

                
            }
        }
    }
}