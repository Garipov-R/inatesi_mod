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

            if (Input.Down("Attack") && _TimeSinceAttack >= _delayShootTime)
            {
                RaycastHit hit;
                _RaycastDistance = 1.5f;
                var isHit = RaycastSphere(
                    .1f,
                    out hit
                );

                //isHit = Raycast(out  hit, Vector2.zero, false);

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
                    hitComponent.isHit = isHit;
                    hitComponent.hit = hit;
                    hitComponent.sizeParticle = _SizeParticle;
                    hitComponent.speedParticle = _VelocityParticle;
                    hitComponent.ray = new Ray(_startRaycastPosition, _startRaycastDirection);


                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * _forceVelocityDamage, _ForceMode);
                    }

                    // CharacterMotion.AudioSource.PlayOneShot(_HitAudioClips[Random.Range(0, _HitAudioClips.Length - 1)]);
                    CharacterMotion.AudioSource.PlayOneShot(_AudioClips[Random.Range(0, _AudioClips.Length - 1)]);
                    Shared.ParticlesManager.SendParticleEvent(EcsWorld, _RaycastHit);
                    //LineSystem.Instance.SetLine(startLinePosition, hit.distance < 10 ? hit.point : _startRaycastPosition + _startRaycastDirection * 10f);
                }
                else
                {
                    //LineSystem.Instance.SetLine(startLinePosition, _startRaycastPosition + _startRaycastDirection * 10f);
                    CharacterMotion.AudioSource.PlayOneShot(_AudioClips[Random.Range(0, _AudioClips.Length - 1)]);
                }


                if (_SpawnedViewModel.activeSelf) 
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