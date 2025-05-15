using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class PistolWeapon : WeaponBase
    {
        [Header("Settings PistolWeapon")]
        [SerializeField] private float _AttackForce = 10f;


        public override void UpdateTick()
        {
            base.UpdateTick();

            if (Input.Released("Reload"))
            {
                Reload();
            }

            if (Input.Pressed("Attack") && _TimeSinceAttack >= _delayShootTime && _reloading == false)
            {
                Shoot();
                _TimeSinceAttack = 0;
            }
        }

        public override void Shoot()
        {
            if (IsEmpty())
            {
                Reload();
                return;
            }
            if (IsEmpty()) return;
            if (EcsWorld == null) return;


            RaycastHit hit;
            var isHit = Raycast(
                out hit
            );

            var startLinePosition = 
                FPC ?
                _ShootPoint.position : 
                _CharacterMotionBase.transform.position + _CharacterMotionBase.transform.up * (_CharacterMotionBase.Height - _CharacterMotionBase.Radius);

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
                hitComponent.velocity = velocity * _forceVelocityDamage;
                hitComponent.position = hit.point;

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * _AttackForce, _ForceMode);
                }

                if (_ShootParticle != null)
                {
                    var g = Instantiate(_ShootParticle, hit.point, Quaternion.identity);
                    g.Play();
                    Destroy(g.gameObject, 2f);
                }

                LineSystem.Instance.SetLine(startLinePosition, hit.distance < 10 ? hit.point : _startRaycastPosition + _startRaycastDirection * 10f);
            }
            else
            {
                LineSystem.Instance.SetLine(startLinePosition, _startRaycastPosition + _startRaycastDirection * 10f);
            }

            _SwayBob.Shake(_ForceShake);

            CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip, _VolumeShoot);

            base.Shoot();
        }
    }
}