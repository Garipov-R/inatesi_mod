using InatesiCharacter.Testing.Character.Weapons;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Props
{
    public class HealthProp : Prop
    {
        [SerializeField] private float _health = 100;
        [SerializeField] private AudioClip _AudioClip;


        protected override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (_SetupLeoEcs == null)
                return;

            var characterFilter = _SetupLeoEcs.World.Filter<InatesiCharacter.Testing.LeoEcs4.Components.CharacterComponent>().End();
            var characterPool = _SetupLeoEcs.World.GetPool<InatesiCharacter.Testing.LeoEcs4.Components.CharacterComponent>();
            var playerPool = _SetupLeoEcs.World.GetPool<InatesiCharacter.Testing.LeoEcs4.Components.PlayerComponent>();

            foreach (var characterEntity in characterFilter)
            {
                ref var characterComponent = ref characterPool.Get(characterEntity);

                if (characterComponent.GameObject == collider.gameObject)
                {
                    if (playerPool.Has(characterEntity))
                    {
                        object[] data = new object[3];
                        data[0] = _PropType;
                        data[1] = _health;
                        data[2] = _AudioClip;

                        Hit(collider.gameObject, true, data);

                        OnTriggerEffect(transform.position);

                        var destroyTime = _OnTriggerAudioClip ? _OnTriggerAudioClip.length : 0;

                        if (_DestroyCollision)
                            Destroy(gameObject);
                    }
                }
            }
        }
    }
}