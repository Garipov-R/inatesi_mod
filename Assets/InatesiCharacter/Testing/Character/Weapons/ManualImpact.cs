using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.Shared.Components;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class ManualImpact : WeaponBase
    {
        [SerializeField] private AnimationClip[] _Animation;
        [SerializeField] private AudioClip[] _AudioClips;
        [SerializeField] private AudioClip[] _HitAudioClips;
        private int _animID = 0;
        private Animator _Animator;



        public override void Enable()
        {
            if (_SpawnedViewModel != null)
            {
                if (!_Animator && SpawnedViewModel.TryGetComponent(out Animator component2)) { _Animator = component2; }
                if (!_Animator && SpawnedViewModel.GetComponentInChildren<Animator>()) { _Animator = SpawnedViewModel.GetComponentInChildren<Animator>(); }
            }

            base.Enable();
        }

        public override void UpdateTick()
        {
            base.UpdateTick();

            if (Input.Released("Reload"))
            {
                Reload();
            }

            if (Input.Pressed("Attack") && _TimeSinceAttack >= _delayShootTime)
            {
                RaycastHit hit;
                var isHit = RaycastSphere(
                    .2f,
                    out hit
                );

                if (isHit)
                {
                    var entity = EcsWorld.NewEntity();

                    var hitPool = EcsWorld.GetPool<DamageComponent>();
                    hitPool.Add(entity);
                    ref var hitComponent = ref hitPool.Get(entity);

                    //hitComponent.first = transform.root.gameObject;
                    //hitComponent.other = cast.transform.gameObject;
                    hitComponent.owner = CharacterMotion.gameObject;
                    hitComponent.target = hit.transform.root.gameObject;
                    hitComponent.damage = _Damage;
                    hitComponent.velocity = (hit.point - CharacterMotion.gameObject.transform.position).normalized * _forceVelocityDamage;
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

                    CharacterMotion.AudioSource.PlayOneShot(_HitAudioClips[Random.Range(0, _HitAudioClips.Length - 1)]);
                    //LineSystem.Instance.SetLine(startLinePosition, hit.distance < 10 ? hit.point : _startRaycastPosition + _startRaycastDirection * 10f);
                }
                else
                {
                    //LineSystem.Instance.SetLine(startLinePosition, _startRaycastPosition + _startRaycastDirection * 10f);
                    CharacterMotion.AudioSource.PlayOneShot(_AudioClips[_animID]);
                }


                _SpawnedViewModel.GetComponent<Animator>().Play(_Animation[_animID].name);
                
                _animID++;
                if (_Animation.Length <= _animID) _animID = 0;



                _SwayBob.Shake(_ForceShake);
                _TimeSinceAttack = 0;
            }
        }

        public override void Shoot()
        {
            base.Shoot();
        }
    }
}