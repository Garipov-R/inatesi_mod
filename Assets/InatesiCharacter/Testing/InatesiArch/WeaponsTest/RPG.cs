using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public class RPG : WeaponBase
    {
        [SerializeField] private float _ForceDamage = 10f;
        [SerializeField] private float _AttackDelay = 1f;
        [SerializeField] private float _ForceShake = -1f;
        [SerializeField] private GameObject _Projectile;
        [SerializeField] private AudioClip _ShootAudioClip;
        private TimeSince timeSinceAttack;


        public override void Enable()
        {
            base.Enable();

            timeSinceAttack = 1;
        }

        public override void UpdateTick()
        {
            base.UpdateTick();

            timeSinceAttack += Time.deltaTime;

            if (Input.Pressed("Attack") && timeSinceAttack > _AttackDelay)
            {
                if (_Projectile != null)
                {
                    var projectile = Instantiate(
                        _Projectile, 
                        CharacterMotion.LookSource.LookDirection() * 1f + CharacterMotion.RagdollMonitor.HeadBone.position, 
                        Quaternion.identity
                    );
                    projectile.GetComponent<Rigidbody>().AddForce(CharacterMotion.LookSource.LookDirection() * _ForceDamage, ForceMode.VelocityChange);

                    PlayAudio(_ShootAudioClip);

                    timeSinceAttack = 0;

                    CharacterMotion.AnimatorMonitor.SetSlot0(102);

                    _SwayBob.Shake(_ForceShake);
                }
            }

            if (Input.Down("Secondary Attack"))
            {
                Rigging(true);
                CharacterMotion.AnimatorMonitor.SetSlot0(102);
            }

            if (Input.Released("Secondary Attack"))
            {
                Rigging(false);
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }

            if (timeSinceAttack < _AttackDelay) 
            {
                Rigging(true);
                CharacterMotion.AnimatorMonitor.SetSlot0(102);
            }
            else
            {
                Rigging(false);
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }
        }
    }
}