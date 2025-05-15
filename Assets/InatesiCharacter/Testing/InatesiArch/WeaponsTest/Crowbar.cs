using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public class Crowbar : WeaponBase
    {
        [SerializeField] private ForceMode _ForceMode = ForceMode.VelocityChange;
        [SerializeField] private float _Force = 10f;
        [SerializeField] private bool _SphereAttack = false;
        [SerializeField] private float _attackDelay = .4f;
        [SerializeField] private float _attackDistance = 3f;
        [SerializeField] private float _ForceShake = 1f;
        TimeSince timeSinceAttack;

        public override void UpdateTick()
        {
            base.UpdateTick();

            timeSinceAttack += Time.deltaTime;

            if ((Input.Down("Attack") || Input.Pressed("Attack")) && timeSinceAttack > _attackDelay)
            {
                if (_SphereAttack)
                {
                    var ray = new Ray(CharacterMotion.RagdollMonitor.HeadBone.transform.position, CharacterMotion.LookSource.LookDirection());

                    if (Game.PlayerInstance != null)
                    {
                        if (Game.PlayerInstance.FirstPersonCamera == true)
                        {
                            ray = new Ray(CharacterMotion.LookSource.LookPosition(), CharacterMotion.LookSource.LookDirection());
                        }
                    }

                    var casts = Physics.SphereCastAll(
                        ray.origin,
                        .5f,
                        ray.direction,
                        _attackDistance,
                        CharacterMotion.RaycastLayer,
                        QueryTriggerInteraction.Ignore
                    );

                    for (int i = 0; i < casts.Length; i++)
                    {
                        var rb = casts[i].rigidbody;

                        if (rb != null)
                        {
                            rb.AddForce((casts[i].point - CharacterMotion.transform.position).normalized * _Force, _ForceMode);
                        }
                    }
                }
                else
                {
                    var ray = new Ray(CharacterMotion.RagdollMonitor.HeadBone.transform.position, CharacterMotion.LookSource.LookDirection());

                    if (Game.PlayerInstance != null)
                    {
                        if (Game.PlayerInstance.FirstPersonCamera == true)
                        {
                            ray = new Ray(CharacterMotion.LookSource.LookPosition(), CharacterMotion.LookSource.LookDirection());
                        }
                    }

                    var cast = Physics.Raycast(
                        ray,
                        out RaycastHit raycastHit,
                        _attackDistance,
                        CharacterMotion.RaycastLayer,
                        QueryTriggerInteraction.Ignore
                    );

                    if (cast)
                    {
                        var rb = raycastHit.rigidbody;

                        if (rb != null)
                        {
                            rb.AddForce((raycastHit.point - CharacterMotion.transform.position).normalized * _Force, _ForceMode);
                        }
                    }
                }

                

                CharacterMotion.AnimatorMonitor.SetSlot0(0);

                timeSinceAttack = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(101);

                _SwayBob.Shake(_ForceShake);
            }

            if (timeSinceAttack > _attackDelay)
            {
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }
        }

        public override void CreateViewModel()
        {
            base.CreateViewModel();



            
            var pos = new Vector3(0.409999996f, -0.8f, 0.550000012f);
            var rot = new Vector3(16.7000046f, 10.46000171f, 1.39276457e-08f);
            _SpawnedViewModel.transform.localPosition = pos;
            _SpawnedViewModel.transform.localEulerAngles = rot;

            _SwayBob.Init(_SpawnedViewModel);
        }
    }
}