using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public class Pistol : WeaponBase
    {
        [SerializeField] private float _AttackDelay = 0.1f;
        [SerializeField] private float _Force = 10f;
        [SerializeField] private float _ForceShake = 10f;
        [SerializeField] private ParticleSystem _AttackParticle;
        [SerializeField] private ParticleSystem _ShootParticle;
        [SerializeField] private AudioClip _ShootClip;
        TimeSince timeSinceAttack;


        public override void Enable()
        {
            base.Enable();

            timeSinceAttack = 10;
        }

        public override void Disable()
        {
            base.Disable();

            CharacterMotion.AnimatorMonitor.SetSlot0(0);
            Rigging(false);
        }

        public override void Attack()
        {
            if (_AttackParticle != null)
            {
                _AttackParticle.Play();
            }

            timeSinceAttack = 0;

            PlayAudio(_ShootClip);

            RaycastHit hit;
            var isHit = Raycast(
                CharacterMotion.LookSource.LookPosition(), 
                CharacterMotion.LookSource.Transform.forward, 
                CharacterMotion.RaycastLayer,
                out hit
            );

            if (isHit)
            {
                //Test
                if (hit.transform.TryGetComponent(out Bot.BotTest bot))
                {
                    bot.Damage(1);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * _Force, ForceMode.VelocityChange);
                }

                if (_ShootParticle != null)
                {
                    var g = Instantiate(_ShootParticle, hit.point, Quaternion.identity);
                    g.Play();
                    Destroy(g.gameObject, 2f);
                }
            }
        }

        public override void UpdateTick()
        {
            base.UpdateTick();


            timeSinceAttack += Time.deltaTime;

            if ((Input.Pressed("Attack")) && timeSinceAttack > _AttackDelay)
            {
                Attack();
                CharacterMotion.AnimatorMonitor.SetSlot0(102);

                //_SwayBob.Shake(_ForceShake);
                _SwayBob.Shake();
            }

            if (Input.Down("Secondary Attack"))
            {
                Debug.DrawLine(
                    CharacterMotion.RagdollMonitor.RightHand.position,
                    CharacterMotion.RagdollMonitor.RightHand.position + CharacterMotion.LookSource.Transform.forward * 100f,
                    Color.red
                );

                Rigging(true);
                CharacterMotion.AnimatorMonitor.SetSlot0(102);
            }

            if (Input.Down("Secondary Attack") == false && timeSinceAttack > _AttackDelay + 1)
            {
                Rigging(false);
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }

            if (timeSinceAttack < _AttackDelay + 1)
            {
                Rigging(true);
            }
        }

        public override void CreateViewModel()
        {
            // 0 .017 .1926

            base.CreateViewModel();

            return;

            if (Game.PlayerInstance == null)
                return;

            if (ViewModel == null)
                return;

            var viewModel = Instantiate(ViewModel, CharacterMotion.LookSource.Transform.GetChild(0));
            viewModel.transform.localPosition = new Vector3(0.41f, -0.35f, 0.55f);
            viewModel.transform.localEulerAngles = Vector3.zero;
            _SpawnedViewModel = viewModel;
        }
    }
}