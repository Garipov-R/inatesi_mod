using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.Shared.Components;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class Shotgun : WeaponBase
    {
        private Rigidbody[] _rigidbodies;
        private float _secondaryShootTimeSince = 1;
        private bool _secondaryShooting;

        public override void UpdateTick()
        {
            base.UpdateTick();

            if (Input.Released("Reload"))
            {
                Reload();
            }

            if ((Input.Pressed("Attack") || Input.Pressed("Secondary Attack")) && _TimeSinceAttack >= _delayShootTime && _reloading == false)
            {
                SendDamageComponent();
                Shoot();
                _TimeSinceAttack = 0;

                if (Input.Pressed("Secondary Attack"))
                {
                    _secondaryShootTimeSince = 0;
                    _secondaryShooting = true;
                }
            }

            if (_secondaryShootTimeSince > 0.1f && _secondaryShooting == true)
            {
                Shoot();
                _secondaryShooting = false;
            }

            _secondaryShootTimeSince += Time.deltaTime; 
        }

        public override void Shoot()
        {
            if (IsEmpty())
            {
                Reload();
                return;
            }


            if (EcsWorld != null)
            {
                int size = 4;
                RaycastHit hit;
                _rigidbodies = new Rigidbody[size];
                var startLinePosition =
                    FPC ?
                    _ShootPoint.position :
                    _CharacterMotionBase.transform.position + _CharacterMotionBase.transform.up * (_CharacterMotionBase.Height - _CharacterMotionBase.Radius);
                Vector2[] scatters =
                {
                new Vector2(1, 1),
                new Vector2(1, -1),
                new Vector2(-1, -1),
                new Vector2(-1, 1)
            };

                for (int i = 0; i < size; i++)
                {
                    var isHit = Raycast(
                        out hit,
                        scatters[i] * Random.Range(.01f, _scatter)
                    );

                    if (isHit)
                    {
                        var entity = EcsWorld.NewEntity();

                        var hitPool = EcsWorld.GetPool<DamageComponent>();
                        hitPool.Add(entity);
                        ref var hitComponent = ref hitPool.Get(entity);

                        var velocity = (hit.point - _startRaycastPosition).normalized;
                        velocity.y = 0;
                        //hitComponent.first = transform.root.gameObject;
                        //hitComponent.other = cast.transform.gameObject;
                        hitComponent.owner = CharacterMotion.gameObject;
                        hitComponent.target = hit.transform.root.gameObject;
                        hitComponent.damage = _Damage;
                        /*hitComponent.velocity =
                            velocity * (_forceVelocityDamage) +
                            new Vector3(0,Random.Range(2,2f),0);*/
                        hitComponent.position = hit.point;
                        hitComponent.isHit = isHit;
                        hitComponent.hit = hit;
                        hitComponent.sizeParticle = _SizeParticle;
                        hitComponent.speedParticle = _VelocityParticle;
                        hitComponent.ray = new Ray(_startRaycastPosition, _startRaycastDirection);


                        if (hit.rigidbody != null)
                        {
                            hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * (_forceVelocityDamage), _ForceMode);
                        }

                        Shared.ParticlesManager.SendParticleEvent(EcsWorld, hitComponent.hit);
                    }
                }




                for (int i = 0; i < _rigidbodies.Length; i++)
                {
                    if (_rigidbodies[i] == null) continue;
                    _rigidbodies[i].AddForce(CharacterMotion.LookSource.Transform.forward * (_forceVelocityDamage), _ForceMode);
                }
            }
            



            _SwayBob.Shake(_ForceShake);

            CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip, _VolumeShoot);

            base.Shoot();
        }
    }
}