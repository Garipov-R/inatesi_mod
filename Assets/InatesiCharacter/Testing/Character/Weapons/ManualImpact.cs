using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Effects;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
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

            if (Input.Pressed("Secondary Attack"))
            {
                ref var combatEvent = ref ECSHelper.Create<CombatEvent>(_StartEcs.EcsWorld);
                combatEvent.aim = true;
            }

            if (Input.Released("Secondary Attack"))
            {
                ref var combatEvent = ref ECSHelper.Create<CombatEvent>(_StartEcs.EcsWorld);
                combatEvent.aim = false;
            }

            if (Input.Down("Attack") && _TimeSinceAttack >= _delayShootTime)
            {
                CharacterMotion.AnimatorMonitor.SetSlotData((int)SlotData.AttackMontirovka);
                ECSHelper.Create<ShootEvent>(_StartEcs.EcsWorld);


                RaycastHit hit;
                _RaycastDistance = 1.5f;
                var isHit = RaycastSphere(
                    .1f,
                    out hit
                );

                //isHit = Raycast(out  hit, Vector2.zero, false);

                if (isHit)
                {
                    var entity = _StartEcs.EcsWorld.NewEntity();

                    var hitPool = _StartEcs.EcsWorld.GetPool<DamageComponent>();
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


                    Shared.ParticlesManager.SendParticleEvent(_StartEcs.EcsWorld, _RaycastHit);
                    //LineSystem.Instance.SetLine(startLinePosition, hit.distance < 10 ? hit.point : _startRaycastPosition + _startRaycastDirection * 10f);
                }

                CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip);



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