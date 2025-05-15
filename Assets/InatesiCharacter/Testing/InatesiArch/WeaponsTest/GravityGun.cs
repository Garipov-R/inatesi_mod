using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public class GravityGun : WeaponBase
    {
        [SerializeField] private ForceMode _PullForceMode = ForceMode.Force;
        [SerializeField] private ForceMode _PickForceMode = ForceMode.Force;
        [SerializeField] private float _AttackDelay = 0.1f;
        [SerializeField] private float _PullForce = 30f;
        [SerializeField] private float _Force = 30f;
        [SerializeField] private float _PickForce = 30f;
        [SerializeField] private float _KeepDistance = 3f;
        [SerializeField] private float _MaxKeepDistance = 6f;
        [SerializeField] private ParticleSystem _AttackParticle;
        [SerializeField] private AudioClip _ShootClip;
        [SerializeField] private AudioClip _PullClip;
        [SerializeField] private AudioClip _KeepClip;
        [SerializeField] private AudioClip _EndClip;
        [SerializeField] private float _SmoothTime = 30f;
        TimeSince timeSinceAttack;
        private Rigidbody _currentRigidbody;
        private bool _firstPull = false;
        private bool _pressedSecondaryAttack = false;
        

        public override void Attack()
        {
            if (_AttackParticle != null)
            {
                _AttackParticle.Play();
            }

            StopAudioLoop();
            PlayAudio(_ShootClip);

            timeSinceAttack = 0;

            if (_currentRigidbody != null)
            {
                _currentRigidbody.linearVelocity = Vector3.zero;
                _currentRigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * _Force, ForceMode.VelocityChange);
                _currentRigidbody.useGravity = true;
                _currentRigidbody = null;
            }
        }

        private void PickUp()
        {
            if (_currentRigidbody == null)
                return;

            var magnetposition = CharacterMotion.LookSource.LookPosition() + CharacterMotion.LookSource.Transform.forward * _KeepDistance;
            
            
            var player = Game.PlayerInstance as Player;
            if (player != null)
            {
                if (player.FirstPersonCamera == true)
                {
                    magnetposition = CharacterMotion.LookSource.LookPosition() + CharacterMotion.LookSource.Transform.forward * _KeepDistance;
                }
                else
                {
                    magnetposition = CharacterMotion.RagdollMonitor.HeadBone.position + CharacterMotion.LookSource.Transform.forward * _KeepDistance;
                }
            }

            var resultPosition = (magnetposition - _currentRigidbody.worldCenterOfMass);


            var force = 0f;
            if (Vector3.Distance(magnetposition, _currentRigidbody.worldCenterOfMass) < 0.5f)
            {
                force = 1;
            }
            else
            {
                force = _PickForce;
            }

            _currentRigidbody.linearVelocity = resultPosition.normalized * force;
            _currentRigidbody.centerOfMass = Vector3.zero;


            var castSphere = 
                Physics.Raycast(
                    CharacterMotion.RagdollMonitor.HeadBone.position,
                    //.5f,
                    CharacterMotion.LookSource.Transform.forward, //( CharacterMotion.LookSource.LookPosition() - _currentRigidbody.position).normalized,
                    out RaycastHit raycastHit,
                    Vector3.Distance(_currentRigidbody.position, CharacterMotion.LookSource.LookPosition()) ,
                    CharacterMotion.RaycastLayer,
                    QueryTriggerInteraction.Ignore
                );

            if (castSphere)
            {
                if (raycastHit.transform == null || raycastHit.transform.gameObject != _currentRigidbody.gameObject)
                {
                    LostObject();

                    return;
                }
            }


            PlayAudioLoop(_KeepClip);
        }

        private void PullForce()
        {
            if (_firstPull == true)
                return;

            if (_currentRigidbody != null)
                return;


            RaycastHit hit;
            var isHit = Physics.Raycast(
                CharacterMotion.RagdollMonitor.HeadBone.position, //CharacterMotion.LookSource.LookPosition(),
                //0.5f,
                CharacterMotion.LookSource.Transform.forward,
                out hit,
                15f,
                CharacterMotion.RaycastLayer,
                QueryTriggerInteraction.Ignore
            );

            if (isHit)
            {
                if (hit.rigidbody != null)
                {
                    var magnetDirection = Vector3.zero;

                    if(Game.PlayerInstance.FirstPersonCamera == true)
                    {
                        magnetDirection = CharacterMotion.LookSource.Transform.forward;
                    }
                    else
                    {
                        magnetDirection = CharacterMotion.RagdollMonitor.HeadBone.forward;
                    }

                    hit.rigidbody.AddForce(magnetDirection * -1 * _PullForce, _PullForceMode);

                    if (Vector3.Distance(hit.point, CharacterMotion.transform.position) < _KeepDistance)
                    {
                        _firstPull = true;
                        _currentRigidbody = hit.rigidbody;
                        _currentRigidbody.useGravity = false;

                        _pressedSecondaryAttack = true;
                    }
                }
            }

            CharacterMotion.AnimatorMonitor.SetSlot0(100);
        }

        private void LostObject()
        {
            CharacterMotion.AnimatorMonitor.SetSlot0(0);
            Rigging(false);

            if (_currentRigidbody == null)
                return;

            _currentRigidbody.linearVelocity = Vector3.zero;
            _currentRigidbody.useGravity = true;
            _currentRigidbody = null;

            StopAudioLoop(); 
            StopAudio();
            PlayAudio(_EndClip);
        }

        public override void UpdateTick()
        {
            base.UpdateTick();


            timeSinceAttack += Time.deltaTime;

            if (Input.Pressed("Secondary Attack"))
            {
                PlayAudio(_PullClip);

                if (_pressedSecondaryAttack == true)
                {
                    LostObject();
                    _firstPull = false;
                    _pressedSecondaryAttack = false;
                }
            }

            if (Input.LongPress("Secondary Attack", 0.2f, false))
            {
                if (_pressedSecondaryAttack == false)
                {
                    PullForce();
                }
            }

            if (_pressedSecondaryAttack == true)
            {
                PickUp();
                Rigging(true);
            }

            /*if (Input.Released("Secondary Attack"))
            {
                LostObject();

                _firstPull = false;
            }*/

            if (Input.Pressed("Attack") && timeSinceAttack > 0.3f)
            {
                Attack();
                timeSinceAttack = 0; 
            }
        }

        public override void CreateViewModel()
        {
            base.CreateViewModel();

            var pos = _SpawnedViewModel.transform.localPosition;
            pos.y = -0.45f;
            _SpawnedViewModel.transform.localPosition = pos;

            _SwayBob.Init(_SpawnedViewModel);
        }
    }
}