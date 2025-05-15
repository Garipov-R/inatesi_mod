using InatesiCharacter.Testing.LeoEcs3.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Props
{
    public class Prop : CollisionCheckerView
    {
        [SerializeField] protected bool _God = false;
        [SerializeField] protected float _Health = 1;
        [SerializeField] protected UnityEvent _OnDead;
        [SerializeField] protected ParticleSystem _OnDeadParticle;
        [SerializeField] protected PropType _PropType = PropType.Ammo;
        [SerializeField] protected bool _DestroyCollision = true;
        [SerializeField] protected bool _DisableOnCollision = true;
        [SerializeField] protected string _CollisionMaskName;

        public float Health { get => _Health; set { _Health = value; if (_Health <= 0) _Health = 0; } }


        public override void OnHit(float damageAmount = 1)
        {
            base.OnHit();

            if (_God) return;

            if (Health <= 0)
            {
                return;
            }

            Health -= damageAmount;

            if (Health <= 0)
            {
                _OnDead?.Invoke();
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_DisableOnCollision && _CollisionMaskName == LayerMask.LayerToName(other.gameObject.layer))
            {
                gameObject.SetActive(false);
            }
        }

        public void SpawnParticleOnDead()
        {
            if (_OnDeadParticle)
            {
                var g = Instantiate(_OnDeadParticle, transform.position, Quaternion.identity);
                Destroy(g.gameObject, 2f);
            }
        }
    }
}