using UnityEngine;



namespace InatesiCharacter.Testing.Props
{
    public class Barrel : Prop
    {
        [Header("Explosion settings")]
        [SerializeField] private float _ExplosionForce = 10f;
        [SerializeField] private float _RadiusExplosion = 10f;
        [SerializeField] private float _UpwardsModifier = 10f;
        [SerializeField] private float _damageAmount = 2f;

        public void Explosion()
        {
            var casts = Physics.SphereCastAll(transform.position, _RadiusExplosion, transform.up, .1f, LayerMask.NameToLayer("Everything"), QueryTriggerInteraction.Ignore);

            if (casts.Length > 0)
            {
                foreach (var cast in casts) 
                {
                    if (cast.rigidbody != null) 
                    {
                        var directionForce = cast.transform.position - transform.position;
                        directionForce.Normalize();

                        cast.rigidbody.AddExplosionForce(_ExplosionForce, transform.position, _RadiusExplosion, _UpwardsModifier, ForceMode.VelocityChange);

                        if (cast.transform.TryGetComponent(out Prop prop))
                        {
                            prop.OnHit(_damageAmount);
                        }
                    }
                }
            }
        }

        public override void OnHit(float damageAmount = 1)
        {
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
    }
}

