using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using InatesiCharacter.Testing.LeoEcs3;
using Leopotam.EcsLite;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        [Header("Base Settings")]
        [SerializeField] protected float _reloadTime = 2f;
        [SerializeField] protected float _delayShootTime = .3f;
        [SerializeField] protected float _forceVelocityDamage = 1f;
        [SerializeField] protected float _Damage = 1f;
        [SerializeField] protected ForceMode _ForceMode = ForceMode.VelocityChange;
        [SerializeField] protected float _RaycastDistance = 100f;

        [Header("Base Settings")]
        [SerializeField] protected CarriableObjectData _carriableObjectData = new();

        [Header("scatter")]
        [SerializeField] protected float _scatter = 0.04f;
        [SerializeField] protected float _scatterTime = 0.5f;

        [Header("Audio")]
        [SerializeField] protected AudioClip _ShootAudioClip;

        [Header("effects")]
        [SerializeField] protected VisualEffect _ShootVisualEffect;
        [SerializeField] protected float _ShootLightTime = 1f;
        [SerializeField] protected float _ShootLightIntensity = 1f;
        [SerializeField] protected Light _ShootLight;
        [SerializeField] protected AudioClip _OutOfAmmoClip;
        [SerializeField] protected float _VolumeShoot = 1f;
        [SerializeField] protected ParticleSystem _ShootParticle;
        [SerializeField] protected float _ForceShake = -1f;

        protected SwayBob _SwayBob;
        protected GameObject _SpawnedViewModel;
        protected CameraMotion _CameraMotion;
        protected SetupLeoEcs _SetupLeoEcs;
        protected RaycastHit _RaycastHit;
        protected float _TimeSinceReload;
        protected bool _reloading = false;
        protected Action<int> _onAmmoChanged;
        protected Action _onReload;
        protected Vector3 _startRaycastPosition;
        protected Vector3 _startRaycastDirection;
        protected float _shootTimeSince;
        protected CharacterMotionBase _CharacterMotionBase;
        protected float _TimeSinceAttack;
        protected bool _Fpc;
        protected int _CurrentAmmo = 0;
        protected bool _scatterEnabled = true;
        protected Transform _ShootPoint;

        protected EcsWorld EcsWorld 
        {
            get 
            {
                if (_SetupLeoEcs == null) return null; 
                if (_SetupLeoEcs.World == null) return null; 
                return _SetupLeoEcs.World; 
            } 
        }
        public CarriableObjectData CarriableObjectData { get => _carriableObjectData; set => _carriableObjectData = value; }
        public Action<int> OnAmmoChanged { get => _onAmmoChanged; set => _onAmmoChanged = value; }
        public CharacterMotionBase CharacterMotion { get => _CharacterMotionBase; set => _CharacterMotionBase = value; }
        public CameraMotion CameraMotion { get => _CameraMotion; set => _CameraMotion = value; }
        public GameObject SpawnedViewModel { get => _SpawnedViewModel; set => _SpawnedViewModel = value; }
        public SwayBob SwayBob { get => _SwayBob; set => _SwayBob = value; }
        public bool FPC 
        { 
            get => _Fpc; 
            set
            {
                _SpawnedViewModel?.SetActive(value);
                _Fpc = value;
            }
        }


        [Zenject.Inject]
        protected void Construct(SetupLeoEcs setupLeoEcs)
        {
            Debug.Log("weapon inject");
            _SetupLeoEcs = setupLeoEcs;
        }

        public virtual void Init()
        {

        }

        public virtual void UpdateEffect()
        {
            if (_SwayBob == null) return;
            if (_SpawnedViewModel == null) return;
            if (_CameraMotion == null) return;


            if (_SpawnedViewModel)
            {
                var dir = CharacterMotion.Velocity;

                var forward = Vector3.Dot(CharacterMotion.transform.forward, dir);
                var right = Vector3.Dot(CharacterMotion.transform.right, dir);

                _SwayBob.Sway(CameraMotion.LookInputVector);
                _SwayBob.SwayRotation(CameraMotion.LookInputVector);
                _SwayBob.Movement(
                    new Vector2(
                            Mathf.Clamp(-right, -1, 1),
                            Mathf.Clamp(forward, -1, 1)
                    ),
                    CharacterMotion.OnGrounded
                );
                _SwayBob.BobRotation(new Vector2(Mathf.Abs(right), Mathf.Abs(forward)));
                _SwayBob.ShakeProccess();

                _SwayBob.Update();
            }


            if (_SpawnedViewModel != null)
            {
                _SpawnedViewModel.SetActive(FPC);
            }
        }

        public virtual void UpdateTick()
        {
            if (IsEmpty() && Inatesi.Inputs.Input.Pressed("Attack")) 
            {
                if (_OutOfAmmoClip) CharacterMotion.AudioSource.PlayOneShot(_OutOfAmmoClip, _VolumeShoot);
                //return;
            } 

            if (_reloading == true)
            {
                _TimeSinceReload -= Time.deltaTime;

                if (_TimeSinceReload <= 0) 
                {
                    _reloading = false;
                    OnReloaded();
                }
            }

            _TimeSinceAttack += Time.deltaTime;

            if (_SwayBob == null) return;
            if (_SpawnedViewModel == null) return;
            if (_CameraMotion == null) return;
            if (_CharacterMotionBase == null) return;
            if (enabled == false) return;



            if (_TimeSinceAttack > .5f)
            {
                _SwayBob.IsOffset = Inatesi.Inputs.Input.Down("Run") && CharacterMotion.InputDirection.magnitude > 0;
            }
            else
            {
                _SwayBob.IsOffset = false;
            }

            if (_reloading) 
            {
                _SwayBob.IsOffset = true;
            }
        }

        public virtual void Attack() 
        {
            
        }
        public virtual void Drop() 
        {
            if (_SpawnedViewModel) Destroy(_SpawnedViewModel);
            Destroy(gameObject);
        }

        public virtual void Disable()
        {
            _onAmmoChanged = null;
            _onReload = null;
            enabled = false; 
            _SpawnedViewModel?.SetActive(false);
            //Destroy(gameObject); 
        }
        public virtual void Enable() 
        {
            if (_ShootVisualEffect != null)
            {
                var shootPoint = _SpawnedViewModel.transform.Find("shootPoint");
                if (shootPoint != null)
                {
                    _ShootVisualEffect.transform.SetParent(shootPoint.transform, false);
                    _ShootVisualEffect.transform.position = shootPoint.position;
                    _ShootVisualEffect.Stop();
                    //var s = _ShootVisualEffect.transform.localScale - _ShootVisualEffect.transform.lossyScale;
                    //_ShootVisualEffect.transform.localScale = _ShootVisualEffect.transform.localScale * ((s.magnitude * 100) % 100);
                    _ShootPoint = shootPoint;
                }
            }


            _CurrentAmmo = _carriableObjectData.Ammo;

            _onAmmoChanged?.Invoke(_CurrentAmmo);

            enabled = true;
            _SpawnedViewModel?.SetActive(true);
        }

        public bool RaycastSphere(float radius, out RaycastHit raycastHit)
        {
            _startRaycastPosition = CharacterMotion.transform.position +
                (CharacterMotion.LookSource.Transform.right * CharacterMotion.LookSource.CameraMotion.AnchorOffset.x) +
                //(CharacterMotion.LookSource.Transform.forward * CharacterMotion.Radius) +
                (CharacterMotion.Up * (CharacterMotion.Height - CharacterMotion.Radius / 2));
            var scatter =
                CharacterMotion.LookSource.Transform.right * UnityEngine.Random.Range(-_scatter, _scatter) 
                + CharacterMotion.LookSource.Transform.up * UnityEngine.Random.Range(-_scatter, _scatter);
            scatter = _scatterEnabled ? scatter : Vector3.zero;
            _startRaycastDirection = CharacterMotion.LookSource.Transform.forward;
            var hit = RaycastSphere(
                _startRaycastPosition,
                radius,
                _startRaycastDirection,
                LayerMask.NameToLayer("Everything"),
                out raycastHit,
                _RaycastDistance,
                QueryTriggerInteraction.Collide
            );

            _RaycastHit = raycastHit;

            return hit && raycastHit.transform.gameObject != CharacterMotion.gameObject;
        }

        public bool RaycastSphere(
            Vector3 start,
            float radius,
            Vector3 direction,
            LayerMask layerMask,
            out RaycastHit hit,
            float distance = 100f,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return Physics.SphereCast(
                start,
                radius,
                direction,
                out hit,
                distance,
                layerMask,
                queryTriggerInteraction
            );
        }

        public bool Raycast(out RaycastHit raycastHit, Vector2 scatterFactor = new(), bool hasScatter = true)
        {
            //var anchor = new Vector3(CharacterMotion.LookSource.CameraMotion.AnchorOffset.x, 0,0);
            _startRaycastPosition = CharacterMotion.transform.position + 
                (CharacterMotion.LookSource.Transform.right * CharacterMotion.LookSource.CameraMotion.AnchorOffset.x) + 
                //(CharacterMotion.LookSource.Transform.forward * CharacterMotion.Radius) +
                (CharacterMotion.Up * (CharacterMotion.Height - CharacterMotion.Radius / 2)) ;

            var scatter = Vector3.zero;
            if (hasScatter)
            {
                scatter = CharacterMotion.LookSource.Transform.right * scatterFactor.x
                        + CharacterMotion.LookSource.Transform.up * scatterFactor.y;
            }
            else
            {
                scatter = CharacterMotion.LookSource.Transform.right * UnityEngine.Random.Range(-_scatter, _scatter)
                        + CharacterMotion.LookSource.Transform.up * UnityEngine.Random.Range(-_scatter, _scatter);
            }
            
            scatter = _scatterEnabled ? scatter : Vector3.zero;
            _startRaycastDirection = CharacterMotion.LookSource.Transform.forward + scatter;
            var hit = Raycast(
                _startRaycastPosition,
                _startRaycastDirection,
                LayerMask.NameToLayer("Everything"),
                out raycastHit,
                _RaycastDistance,
                QueryTriggerInteraction.Collide
            );

            _RaycastHit = raycastHit;

            return hit && raycastHit.transform.gameObject != CharacterMotion.gameObject;
        }

        public bool Raycast(out RaycastHit raycastHit, string layer, Vector2 scatterFactor = new(), bool hasScatter = false)
        {
            _startRaycastPosition = CharacterMotion.transform.position +
                (CharacterMotion.LookSource.Transform.right * CharacterMotion.LookSource.CameraMotion.AnchorOffset.x) +
                //(CharacterMotion.LookSource.Transform.forward * CharacterMotion.Radius) +
                (CharacterMotion.Up * (CharacterMotion.Height - CharacterMotion.Radius / 2));

            var scatter = Vector3.zero;
            if (hasScatter)
            {
                scatter = CharacterMotion.LookSource.Transform.right * scatterFactor.x
                        + CharacterMotion.LookSource.Transform.up * scatterFactor.y;
            }
            else
            {
                scatter = CharacterMotion.LookSource.Transform.right * UnityEngine.Random.Range(-_scatter, _scatter)
                        + CharacterMotion.LookSource.Transform.up * UnityEngine.Random.Range(-_scatter, _scatter);
            }

            scatter = _scatterEnabled ? scatter : Vector3.zero;
            _startRaycastDirection = CharacterMotion.LookSource.Transform.forward + scatter;
            var hit = Raycast(
                _startRaycastPosition,
                _startRaycastDirection,
                LayerMask.NameToLayer(layer),
                out raycastHit,
                _RaycastDistance,
                QueryTriggerInteraction.Collide
            );

            _RaycastHit = raycastHit;

            return hit && raycastHit.transform.gameObject != CharacterMotion.gameObject;
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

        public void Reload()
        {
            if (_reloading == true)
                return;

            if (_TimeSinceReload > 0)
                return;

            if (IsEmptyMagazine()) return;

            if (!CanReload()) return;

            _reloading = true;
            _TimeSinceReload = _reloadTime;

            OnReload();
        }

        public virtual void OnReload() 
        {
            if (IsEmptyMagazine()) return;

            _onReload?.Invoke();
            _onAmmoChanged?.Invoke(_CurrentAmmo);
        }

        public virtual void OnReloaded()
        {
            int ammo2 = _carriableObjectData.MagCapacity - _carriableObjectData.Ammo;
            int totalAmmo = _carriableObjectData.TotalAmmo - ammo2;
            if (_carriableObjectData.TotalAmmo < ammo2)
            {
                ammo2 = _carriableObjectData.Ammo + _carriableObjectData.TotalAmmo;
                totalAmmo = 0;
            }
            else
            {
                ammo2 += _carriableObjectData.MagCapacity;
            }
            _carriableObjectData.Ammo = Mathf.Abs(ammo2);
            _carriableObjectData.TotalAmmo = totalAmmo;

            _CurrentAmmo = _carriableObjectData.Ammo;
            _onAmmoChanged?.Invoke(_CurrentAmmo);

            return;


            int ammo = 0;
            if (_carriableObjectData.TotalAmmo <= _carriableObjectData.MagCapacity)
            {
                ammo = _carriableObjectData.TotalAmmo;
                _carriableObjectData.TotalAmmo = 0;
            }
            else
            {
                ammo = _carriableObjectData.TotalAmmo - Mathf.Abs(_carriableObjectData.TotalAmmo - (_carriableObjectData.MagCapacity - _carriableObjectData.Ammo));
            }

            _carriableObjectData.Ammo = Mathf.Abs(ammo);
            _carriableObjectData.TotalAmmo = _carriableObjectData.TotalAmmo - ammo;

            _CurrentAmmo = _carriableObjectData.Ammo;
            _onAmmoChanged?.Invoke(_CurrentAmmo);
        }

        public virtual void Shoot(int ammoAmount = 0)
        {
            /*if (_RaycastHit.transform != null && _RaycastHit.transform.gameObject != CharacterMotion.gameObject)
            {
                if (_RaycastHit.transform.TryGetComponent(out CollisionCheckerView component)) { component.OnHit(); }
            }*/

            if (IsEmpty()) return;

            _carriableObjectData.Ammo -= ammoAmount;
            _CurrentAmmo = _carriableObjectData.Ammo;

            if (_CurrentAmmo <= 0 && _carriableObjectData.TotalAmmo > 0)
            {
                Reload();
            }

            _onAmmoChanged?.Invoke(_CurrentAmmo);

            if (_ShootVisualEffect != null) 
            {
                if (_Fpc == true)
                {
                    var shootPoint = _SpawnedViewModel.transform.Find("shootPoint");
                    if (shootPoint != null)
                    {
                        _ShootVisualEffect.transform.position = shootPoint.position;
                        _ShootVisualEffect.transform.rotation = shootPoint.rotation;
                        _ShootVisualEffect.Play();
                    }
                }
                
            }
        }

        public virtual void Shoot()
        {
            Shoot(1);
        }

        private void OnDestroy()
        {
            _onAmmoChanged = null;
            _onReload = null;
        }

        public bool IsEmpty()
        {
            return (_carriableObjectData.Ammo < 1);
        }

        public bool CanReload()
        {
            return (_carriableObjectData.Ammo < _carriableObjectData.MagCapacity);
        }

        public bool IsEmptyMagazine()
        {
            return _carriableObjectData.TotalAmmo < 1;
        }
    }
}