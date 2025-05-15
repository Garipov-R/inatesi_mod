using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using Mirror.Examples.Common;
using Mirror.Examples.Tanks;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class RPGWeapon : WeaponBase
    {
        [Header("Settings RPGWeapon")]
        [SerializeField] private float _AttackForce = 10f;
        [SerializeField] private GameObject _Projectile;
        [SerializeField] private bool _autoShoot = false;

        private float _secondaryAttackForce = 4f;
        private float _currentAttackForce = 0;

        public override void Init()
        {
            base.Init();
        }

        public override void UpdateTick()
        {
            base.UpdateTick();

            if (IsEmpty())
            {
                Reload();
                return;
            }
            if (_reloading)
                return;

            if (_autoShoot == false && (Input.Pressed("Attack") || Input.Pressed("Secondary Attack")) && _TimeSinceAttack >= _delayShootTime)
            {
                _currentAttackForce = Input.Pressed("Secondary Attack") ? _secondaryAttackForce : _AttackForce;
                Shoot();
                _TimeSinceAttack = 0;
            }
            
            if (_autoShoot && Input.Down("Attack") && _TimeSinceAttack >= _delayShootTime)
            {
                Shoot();
                _TimeSinceAttack = 0;
            }
        }

        public override void Shoot()
        {
            if (_Projectile == null) return;

            var position = CharacterMotion.transform.position  + 
                (CharacterMotion.Up * (CharacterMotion.Height)) + CharacterMotion.LookSource.LookDirection() * CharacterMotion.Radius;


            var radiusMultiple = 2f;
            var hit = Physics.Raycast(
                position, 
                CharacterMotion.LookSource.LookDirection(), 
                out RaycastHit hitInfo,
                radiusMultiple, 
                CharacterMotion.RaycastLayer, 
                QueryTriggerInteraction.Ignore
            );

            if (hit && hitInfo.distance < CharacterMotion.Radius)
            {
                radiusMultiple = .5f;
            }

            //position += (CharacterMotion.transform.forward * radiusMultiple);

            var projectile = Instantiate(
                _Projectile,
                position + CharacterMotion.transform.forward * (CharacterMotion.Radius * radiusMultiple), 
                Quaternion.Euler(CharacterMotion.LookSource.LookDirection() + Vector3.right)
            );

            if (projectile.TryGetComponent(out Rigidbody rb) == false) return;

            rb.AddForce(CharacterMotion.LookSource.LookDirection() * _currentAttackForce, _ForceMode);
            rb.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0));

            _SwayBob.Shake(_ForceShake);

            CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip, _VolumeShoot);

            base.Shoot();
        }
    }
}