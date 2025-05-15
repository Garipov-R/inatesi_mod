using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Decals;
using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.Shared.Components;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class AutoMatWeapon : WeaponBase
    {


        private Animation _Animation;
        private Animator _Animator;


        public override void Enable()
        {
            if (_SpawnedViewModel != null)
            {
                if (!_Animator && SpawnedViewModel.TryGetComponent(out Animator component2)) { _Animator = component2; }
                if (!_Animation && SpawnedViewModel.TryGetComponent(out Animation component)) { _Animation = component; }
                if (!_Animator && SpawnedViewModel.GetComponentInChildren<Animator>()) { _Animator = SpawnedViewModel.GetComponentInChildren<Animator>(); }
                if (!_Animation && SpawnedViewModel.GetComponentInChildren<Animation>()) { _Animation = SpawnedViewModel.GetComponentInChildren<Animation>(); }
            }

            base.Enable();
        }

        public override void UpdateTick()
        {
            base.UpdateTick();

            if (Input.Down("Attack"))
            {
                _shootTimeSince += Time.fixedDeltaTime;
            }

            if (Input.Pressed("Attack"))
            {
                _shootTimeSince += Time.fixedDeltaTime;
            }

            if (Input.Released("Reload"))
            {
                Reload();
            }

            if (_reloading == false && Input.Down("Attack") && _TimeSinceAttack >= _delayShootTime)
            {
                Shoot();
                _TimeSinceAttack = 0;
            }

            if (_reloading == false && !Input.Down("Attack") && _TimeSinceAttack >= .2f)
            {
                if (_Animation) 
                { 
                    _Animation.Play(PlayMode.StopAll); 
                    _Animation.Stop(); 
                }

                _shootTimeSince = _shootTimeSince > 0 ? _shootTimeSince - Time.deltaTime : 0;
                _scatterEnabled = false;
            }

            if (_ShootLight) _ShootLight.intensity -= Time.deltaTime * _ShootLightTime;
        }

        public override void Shoot()
        {
            if (IsEmpty()) 
            {
                Reload();
                return;
            }

            if (EcsWorld == null) return;

            RaycastHit hit;

            var isHit = Raycast(out hit);
            var startLinePosition =
                FPC ?
                _ShootPoint.position :
                _CharacterMotionBase.transform.position + _CharacterMotionBase.transform.up * (_CharacterMotionBase.Height - _CharacterMotionBase.Radius);
            
            if (isHit)
            {
                Debug.Log(hit.transform.name);
                var entity = EcsWorld.NewEntity();

                var hitPool = EcsWorld.GetPool<DamageComponent>();
                hitPool.Add(entity);
                ref var hitComponent = ref hitPool.Get(entity);

                var velocity = (hit.point - _startRaycastPosition).normalized;
                velocity.y = 0;
                //hitComponent.first = transform.root.gameObject;
                //hitComponent.other = cast.transform.gameObject;
                hitComponent.owner = CharacterMotion.gameObject;
                hitComponent.target = hit.transform.gameObject;
                hitComponent.damage = _Damage;
                hitComponent.velocity = velocity * _forceVelocityDamage;
                hitComponent.position = hit.point;


                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * _forceVelocityDamage, _ForceMode);
                }

                if (_ShootParticle != null)
                {
                    var g = Instantiate(_ShootParticle, hit.point, Quaternion.identity);
                    g.Play();
                    Destroy(g.gameObject, 2f);
                }

                if (_ShootLight) _ShootLight.intensity = _ShootLightIntensity;

                if (DecalsManager.instance != null && isHit && false)
                {
                    var decal = DecalsManager.instance.GetDecal();
                    decal.transform.rotation = Quaternion.LookRotation(hit.normal);
                    decal.transform.position = hit.point;
                }

                LineSystem.Instance.SetLine(startLinePosition, hit.distance < 10 ? hit.point : _startRaycastPosition + _startRaycastDirection * 10f);
            }
            else
            {
                LineSystem.Instance.SetLine(startLinePosition, _startRaycastPosition + _startRaycastDirection * 10f);
            }

            if (_shootTimeSince > _scatterTime)
            {
                _scatterEnabled = true;
                CharacterMotion.LookSource.CameraMotion.Shake(new Vector2(Random.Range(_scatter, 0), Random.Range(_scatter, -_scatter)));
            }
            

            _SwayBob.Shake(_ForceShake);
            CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip, _VolumeShoot);

            if (_Animation)
            {
                _Animation.Play(PlayMode.StopSameLayer);
            }




            base.Shoot(1);
        }

        public override void OnReload()
        {
            base.OnReload();

            if (_Animator)
            {
                _Animator.Play("reload", 0);
            }
        }
    }
}