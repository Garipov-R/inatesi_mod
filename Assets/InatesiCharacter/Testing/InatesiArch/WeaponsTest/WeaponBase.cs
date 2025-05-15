using InatesiCharacter.SuperCharacter;
using System;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        [SerializeField] protected AudioSource _AudioSource;
        [SerializeField] protected float _ReloadTime = 1f;

        [Header("View model")]
        [SerializeField] protected Vector3 _ViewModelPosition = new Vector3(0.32f, -0.45f, 0.45f);
        [SerializeField] protected Vector3 _ViewmodelRotation = Vector3.zero;

        [Header("Sway Bob")]
        [SerializeField] protected SwayBob _SwayBob = new SwayBob();
        [SerializeField] protected SwayBobItemSO _SwayBobItemSO;
        [Space(10)]

        protected GameObject _viewModel;
        protected GameObject _SpawnedViewModel;
        protected Camera.CameraMotion _CameraMotion;
        protected TimeSince _TimeSinceAttack;

        public AudioSource AudioSource { get => _AudioSource; set => _AudioSource = value; }
        public CharacterMotionBase CharacterMotion { get; set ; }
        public GameObject ViewModel { get => _viewModel; set => _viewModel = value; }


        private void OnValidate()
        {
            if (_SpawnedViewModel != null)
            {
                _SpawnedViewModel.transform.localPosition = _ViewModelPosition;
                _SpawnedViewModel.transform.localEulerAngles = _ViewmodelRotation;
            }
        }

        public virtual void Init()
        {
            _CameraMotion = CharacterMotion.LookSource.GameObject.GetComponent<Camera.CameraMotion>();

            if (_SwayBobItemSO == null)
            {
                try
                {
                    SwayBobItemSO p = ScriptableObject.CreateInstance<SwayBobItemSO>();
                    SwayBobItemSO e = Resources.Load<SwayBobItemSO>(Configs.Configs.c_SwayBobItemPath);
                    p = e;
                    _SwayBobItemSO = p;
                }
                catch (Exception _)
                {
                    _SwayBobItemSO = ScriptableObject.CreateInstance<SwayBobItemSO>();
                }
            }

            _SwayBob = _SwayBobItemSO.SwayBob;

            CreateViewModel();
        }

        public virtual void Enable()
        {
            if (Game.PlayerInstance != null)
            {
                _SpawnedViewModel.SetActive(Game.PlayerInstance.FirstPersonCamera);
            }
        }

        public virtual void Disable()
        {
            if (_SpawnedViewModel != null)
            {
                Destroy(_SpawnedViewModel);
            }

            Rigging(false);
        }

        public virtual void Attack() { }

        public virtual void UpdateTick()
        {
            if (_SpawnedViewModel)
            {
                _SwayBob.Sway(_CameraMotion.LookInputVector);
                _SwayBob.SwayRotation(_CameraMotion.LookInputVector);
                _SwayBob.Movement(
                    new Vector2(
                        Mathf.Clamp(Mathf.Abs(CharacterMotion.InputDirection.x), 0, 1),
                        Mathf.Clamp(Mathf.Abs(CharacterMotion.InputDirection.y), 0, 1)
                    ), 
                    CharacterMotion.OnGrounded
                );
                _SwayBob.BobRotation(CharacterMotion.InputDirection);
                _SwayBob.ShakeProccess();

                //_SwayBob.IsOffset = CharacterMotion.;

                _TimeSinceAttack += Time.deltaTime;

                if (Inatesi.Inputs.Input.Pressed("Attack") || Inatesi.Inputs.Input.Pressed("Secondary Attack"))
                {
                    _TimeSinceAttack = 0;
                }

                if (_TimeSinceAttack > .5f)
                {
                    _SwayBob.IsOffset = Inatesi.Inputs.Input.Down("Run");
                }
                else
                {
                    _SwayBob.IsOffset = false;
                }
                


                _SpawnedViewModel.transform.localPosition = Vector3.Lerp(
                    _SpawnedViewModel.transform.localPosition,
                    _SwayBob.SwayPos + _SwayBob.BobPosition,
                    Time.deltaTime * _SwayBob.Smooth
                );

                _SpawnedViewModel.transform.localRotation = Quaternion.Slerp(
                    _SpawnedViewModel.transform.localRotation,
                    Quaternion.Euler(_SwayBob.SwayEulerRot) * Quaternion.Euler(_SwayBob.BobEulerRotation),
                    Time.deltaTime * _SwayBob.SmoothRot
                );
            }

            if (Game.PlayerInstance != null)
            {
                if (_SpawnedViewModel != null)
                {
                    _SpawnedViewModel.SetActive(Game.PlayerInstance.FirstPersonCamera);
                }
            }
        }

        public virtual void Drop()
        {
            Rigging(false);
        }


        public void Rigging(bool state = true)
        {
            var player = Game.PlayerInstance as Player;

            if (player == null)
                return;

            var CharacterVars = player.CharacterVars;

            CharacterVars.RiggingTest.Rig.weight = state == true ? 1 : 0;

            if (CharacterVars.RiggingTest.SpineRig != null)
            {
                CharacterVars.RiggingTest.SpineRig.transform.rotation =
                    Quaternion.LookRotation(
                       CharacterMotion.LookSource.Transform.forward//,
                                                                //CharacterMotion.Up
                    );
            }
            

            /*CharacterVars.RiggingTest.RHandRig.transform.rotation =
                    Quaternion.LookRotation(
                        CharacterMotion.LookSource.Transform.right
                    //characterComponent.characterMotionTest.Up
                    );

            CharacterVars.RiggingTest.LHandRig.transform.rotation =
                Quaternion.LookRotation(
                    CharacterMotion.LookSource.Transform.right
                //characterComponent.characterMotionTest.Up
                );*/
        }

        public void PlayAudio(AudioClip audioClip)
        {
            /*if (AudioSource != null)
            {
                if (audioClip != null)
                {
                    AudioSource.PlayOneShot(audioClip);
                }
            }*/

            if (audioClip != null)
            {
                CharacterMotion.AudioSource.PlayOneShot(audioClip);
            }
        }

        public void PlayAudioLoop(AudioClip audioClip)
        {
            /*if (AudioSource != null)
            {
                if (audioClip != null)
                {
                    if (audioClip == AudioSource.clip)
                        return;

                    AudioSource.loop = true;
                    AudioSource.clip = audioClip;
                    AudioSource.Play();
                }
            }*/

            if (audioClip != null)
            {
                if (audioClip == CharacterMotion.AudioSource.clip)
                    return;

                CharacterMotion.AudioSource.loop = true;
                CharacterMotion.AudioSource.clip = audioClip;
                CharacterMotion.AudioSource.Play();
            }
        }

        public void StopAudioLoop()
        {
            /*if (AudioSource != null)
            {
                AudioSource.loop = false;
                AudioSource.clip = null;
            }*/

            CharacterMotion.AudioSource.loop = false;
            CharacterMotion.AudioSource.clip = null;
        }

        public void StopAudio()
        {
            /*if (AudioSource != null)
            {
                AudioSource.Stop();
            }*/

            CharacterMotion.AudioSource.Stop();
        }

        public bool Raycast(
            Vector3 start, 
            Vector3 direction, 
            LayerMask layerMask,
            out RaycastHit hit,
            float distance = 100f,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore
        )
        {
            return Physics.Raycast(
                start,
                direction,
                out hit,
                distance,
                layerMask,
                queryTriggerInteraction
            );
        }

        public virtual void CreateViewModel()
        {
            if (Game.PlayerInstance == null)
                return;

            if (ViewModel == null)
                return;

            var viewModel = Instantiate(ViewModel, CharacterMotion.LookSource.Transform.GetChild(0));
            viewModel.transform.localPosition = _ViewModelPosition;
            viewModel.transform.localEulerAngles = _ViewmodelRotation;

            viewModel.transform.localPosition = _SwayBob.Position;
            viewModel.transform.localEulerAngles = _SwayBob.Rotation;

            viewModel.layer = LayerMask.NameToLayer("FPC");
            _SpawnedViewModel = viewModel;

            _SwayBob.Init(_SpawnedViewModel);

            
        }    
    }

    
}