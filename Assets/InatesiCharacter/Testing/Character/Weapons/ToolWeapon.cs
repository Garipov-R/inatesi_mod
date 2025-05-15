using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using Mirror.Examples.Common;
using Mirror.Examples.Tanks;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class ToolWeapon : WeaponBase
    {
        [Header("Settings ToolWeapon")]
        [SerializeField] private float _ReloadTime = .5f;
        [SerializeField] private float _AttackForce = 10f;
        [SerializeField] private float _ZoomLerp = 1f;
        [SerializeField] private float _SelectedRigidbodyVelocityLerp = 1f;
        [SerializeField] private LineRenderer _LineRenderer;



        private Rigidbody _SelectedRigidbody;
        private float _distanceSelectedRigidbody;
        private Quaternion _SelecterdRigidbodyRotation;


        public override void Init()
        {
            base.Init();
        }

        public override void Enable()
        {
            base.Enable();
            LineRender(Vector2.zero, Vector2.zero, false);
        }

        public override void Disable()
        {
            LostObject();
            CameraMotion.InputEnabled = true;

            base.Disable();
        }

        public override void UpdateTick()
        {

            if (Input.Pressed("Attack"))
            {
                SelectObject();
                _SwayBob.Shake(_ForceShake);
            }

            if (Input.Down("Attack"))
            {
                var isHit = Raycast(out RaycastHit hit);

                if (isHit)
                {
                    LineRender(_SpawnedViewModel.transform.position, hit.point);
                }
                else
                {
                    LineRender(_SpawnedViewModel.transform.position, _SpawnedViewModel.transform.position + CharacterMotion.LookSource.LookDirection() * 100f);
                }


                KeepObject();
                _TimeSinceAttack = 0;
            }
            else
            {
                LostObject();
                LineRender(Vector3.zero, Vector3.zero, false);
            }

            if (Input.Down("Rotate"))
            {
                CameraMotion.InputEnabled = false;
                RotateObject();
            }
            else
            {
                SetupObjectRotate();
                CameraMotion.InputEnabled = true;
            }

            if (Input.Pressed("Secondary Attack"))
            {
                FreezeObject();
            }

            _distanceSelectedRigidbody = Mathf.Lerp(_distanceSelectedRigidbody, _distanceSelectedRigidbody + Input.GetVector("Zoom").y * 10f, Time.deltaTime * _ZoomLerp);
            _distanceSelectedRigidbody = Mathf.Clamp(_distanceSelectedRigidbody, CharacterMotion.Radius * 2.5f, _distanceSelectedRigidbody);

            base.UpdateTick();

            _TimeSinceAttack += Time.deltaTime;

            if (_TimeSinceAttack > .5f)
            {
                SwayBob.IsOffset = Inatesi.Inputs.Input.Down("Run") && CharacterMotion.InputDirection.magnitude > 0;
            }
            else
            {
                SwayBob.IsOffset = false;
            }
        }

        private void LostObject()
        {
            if (_SelectedRigidbody != null)
            {
                _SelectedRigidbody.useGravity = true;
                _SelectedRigidbody.freezeRotation = false;
                _SelectedRigidbody = null;
            }
        }

        private void SelectObject()
        {
            if (_SelectedRigidbody != null) return;

            var isHit = Raycast(out RaycastHit hit);

            if (isHit)
            {
                if (hit.rigidbody != null)
                {
                    _distanceSelectedRigidbody = Vector3.Distance(CharacterMotion.transform.position + CharacterMotion.Up * (CharacterMotion.Height - CharacterMotion.Radius / 2), hit.transform.position);
                    _SelectedRigidbody = hit.rigidbody;
                    //_SelectedRigidbody.isKinematic = true;
                    _SelectedRigidbody.freezeRotation = false;
                    _SelectedRigidbody.constraints = RigidbodyConstraints.None;
                    _SelectedRigidbody.useGravity = false;
                    _SelecterdRigidbodyRotation = _SelectedRigidbody.transform.localRotation;
                    //_SelectedRigidbody.freezeRotation = true;
                }
            }
        }

        private void KeepObject()
        {
            if (_SelectedRigidbody == null)
                return;

            var playerPos = (CharacterMotion.transform.position + CharacterMotion.Up * (CharacterMotion.Height - CharacterMotion.Radius / 2));
            var magnetposition = playerPos +  CharacterMotion.LookSource.LookDirection() * _distanceSelectedRigidbody;
            var resultPosition = (magnetposition - _SelectedRigidbody.worldCenterOfMass);


            var force = 0f;
            if (Vector3.Distance(magnetposition, _SelectedRigidbody.worldCenterOfMass) < .01f)
            {
                force = 1;
            }
            else
            {
                force = 30;
            }

            _SelectedRigidbody.linearVelocity = Vector3.MoveTowards(_SelectedRigidbody.linearVelocity, resultPosition * force, Time.deltaTime * _SelectedRigidbodyVelocityLerp); 
            _SelectedRigidbody.centerOfMass = Vector3.zero;

            LineRender(_SpawnedViewModel.transform.position, _SelectedRigidbody.worldCenterOfMass);

            //CharacterMotion.AudioSource.PlayOneShot(_ShootAudioClip, _VolumeShoot);
        }

        private void FreezeObject()
        {
            if (_SelectedRigidbody == null)
                return;

            _SelectedRigidbody.freezeRotation = true;
            _SelectedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _SelectedRigidbody = null;

            _SwayBob.Shake(_ForceShake);
        }

        private void RotateObject()
        {
            if (_SelectedRigidbody == null)
                return;

            _SelectedRigidbody.freezeRotation = false;

            var axis = Vector3.Scale(CharacterMotion.LookSource.Transform.up, Vector3.up);
            var input = Input.GetVector("Look");


            _SelectedRigidbody.angularVelocity = Vector3.MoveTowards(
                _SelectedRigidbody.angularVelocity, 
                //_SelectedRigidbody.angularVelocity + 
                new Vector3(
                    input.y * 10f,
                    input.x * 10f, 
                    0
                ), Time.deltaTime * 500f);

            _SelecterdRigidbodyRotation = _SelectedRigidbody.transform.localRotation;
        }

        private void SetupObjectRotate()
        {
            if (_SelectedRigidbody == null)
                return;

            //_SelectedRigidbody.freezeRotation = false;
            _SelectedRigidbody.transform.localRotation = Quaternion.Lerp(_SelectedRigidbody.transform.localRotation, _SelecterdRigidbodyRotation, Time.deltaTime * 10f);
            _SelectedRigidbody.angularVelocity = Vector3.zero;
        }

        private void LineRender(Vector3 startPos, Vector3 endPos, bool state = true)
        {
            if (_LineRenderer == null)
            {
                return;
            }


            if (_SpawnedViewModel != null && _Fpc == true)
            {
                var shootPoint = _SpawnedViewModel.transform.Find("shootPoint");
                if (shootPoint != null)
                {
                    startPos = shootPoint.position;
                }
            }

            _LineRenderer.SetPosition(0, startPos);
            _LineRenderer.SetPosition(1, endPos);

            _LineRenderer.enabled = state;
        }
    }
}