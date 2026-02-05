using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class TestlWeapon : WeaponBase
    {
        [Header("Settings Test Weapon")]
        [SerializeField] private float _RecoilShooting = 16f;
        [SerializeField] private float _projectileMoveSpeed = 10f;
        [SerializeField] private GameObject _Projectile;
        [SerializeField] protected AudioClip[] _ShootAudioClips;


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
            if (Input.Down("Secondary Attack") && _TimeSinceAttack >= .1f && _reloading == false)
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

            _CharacterMotionBase.AddForce(_CharacterMotionBase.LookSource.Transform.forward * -_RecoilShooting);

            _SwayBob.Shake(_ForceShake);

            CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClips[Random.Range(0, _ShootAudioClips.Length - 1)], _VolumeShoot);

            SpawnBulletProjectile();

            base.Shoot();
        }

        private void SpawnBulletProjectile()
        {
            var entity = EcsWorld.NewEntity();
            var bulletPool = EcsWorld.GetPool<BulletProjectileComponent>();
            bulletPool.Add(entity);
            ref var bulletComponent = ref bulletPool.Get(entity);

            var projectile = Instantiate(_Projectile);

            bulletComponent.gameObject = projectile;
            bulletComponent.transform = projectile.transform;
            bulletComponent.remainLifeTime = 10f;
            bulletComponent.moveSpeed = _projectileMoveSpeed;
            bulletComponent.moveDirection = (_CharacterMotionBase.LookSource.LookDirection());
            var startPosition = 
                (_CharacterMotionBase.transform.forward * _CharacterMotionBase.Radius * 2
                + (_CharacterMotionBase.Height - _CharacterMotionBase.Radius / 2) * _CharacterMotionBase.Up)  + _CharacterMotionBase.transform.position;

            if (FPC)
            {
                startPosition = _CharacterMotionBase.LookSource.LookPosition() + (_CharacterMotionBase.LookSource.LookDirection() * _CharacterMotionBase.Radius * 2);
            }

            bulletComponent.transform.position = startPosition;
            bulletComponent.isTriggerOnDestroy = true;
            bulletComponent.lifetimeAfterDestroy = 0f;
            bulletComponent.isRigidbody = projectile.GetComponent<Rigidbody>() != null;
            bulletComponent.addForce = new Vector3(bulletComponent.moveDirection.x, bulletComponent.moveDirection.y * 5f, bulletComponent.moveDirection.z * 11f);

            if (bulletComponent.isRigidbody)
            {
                bulletComponent.rigidbody = projectile.GetComponent<Rigidbody>();
                bulletComponent.rigidbody.AddForce(bulletComponent.moveDirection * _projectileMoveSpeed, ForceMode.Impulse);
            }
        }
    }
}