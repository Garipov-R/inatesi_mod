using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Camera
{
    public class CameraMotion : MonoBehaviour
    {
        enum UpdateMethod { Fixed, Update, Late };

        [SerializeField] private Transform _Follow;
        [SerializeField] private Vector2 _SpeedLook = new Vector2(4f, 4f);
        [SerializeField] private LayerMask _Mask;
        [SerializeField] private float _Radius = .4f;
        [SerializeField] private UpdateMethod _UpdateMethod = UpdateMethod.Late;
        [SerializeField] private float _StartZoom = 2.25f;
        [SerializeField][Range(0.25f, 1f)] private float _ZoomChangeValue = .25f;
        [SerializeField] private UnityEngine.Camera _camera;

        [Header("Input")]
        [SerializeField] private bool _InputEnabled = true;
        [SerializeField] private bool _MouseInput = true;
        [SerializeField] private bool _NewInputSystem = true;
        [SerializeField] private string _InputName = "Look";
        [SerializeField] private string _ScrollInputName = "Scroll";
        [SerializeField] private string _HorizontalInputName = "Horizontal";
        [SerializeField] private string _VerticalInputName = "Vertical";

        [Header("Settings")]
        [SerializeField] private Vector3 _AnchorOffset = new Vector3(0,2.3f,0);
        [SerializeField] private Vector3 _CameraOffset;
        [SerializeField] private Vector2 _ZoomRange = new Vector2(0, 5);
        [SerializeField] private Vector2 _LimitXRotation =  new Vector2(-80,80);
        [SerializeField] private float _SmoothTime = 1f;
        [SerializeField] private bool _IsSmooth = false;

        [Header("Mouse")]
        [SerializeField] private bool _IsEditor = false;
        [SerializeField] private bool _CursorVisible = false;
        [SerializeField] private CursorLockMode _CursorLockMode;

        private Vector2 _LookInputVector;
        private Vector3 _LookRotationEuler;
        private Vector3 _VirtualPosition;
        private float _ZoomAmount;
        private RaycastHit _ColliderRaycast;
        private Quaternion _LookRotation;
        private Vector3 _ShakeRotation;
        private Vector3 _ShakeLerp;

        public Transform Follow { get => _Follow; set => _Follow = value; }
        public Vector3 AnchorOffset { get => _AnchorOffset; set => _AnchorOffset = value; }
        public float ZoomAmount 
        { 
            get => _ZoomAmount;
            set
            {
                _ZoomAmount = _ZoomAmount = Mathf.Clamp(value, _ZoomRange.x, _ZoomRange.y);
            }
        }
        public CursorLockMode CursorLockMode { get => _CursorLockMode; set => _CursorLockMode = value; }
        public bool CursorVisible { get => _CursorVisible; set => _CursorVisible = value; }
        public bool MouseInput { get => _MouseInput; set => _MouseInput = value; }
        public Vector2 LookInputVector { get => _LookInputVector; set => _LookInputVector = value; }
        public float StartZoom { get => _StartZoom; set => _StartZoom = value; }
        public bool InputEnabled { get => _InputEnabled; set => _InputEnabled = value; }
        public UnityEngine.Camera Camera { get => _camera; private set => _camera = value; }


        #region Unity

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private void Start()
        {
            if (_IsEditor)
            {
                Cursor.lockState = _CursorLockMode;
                Cursor.visible = _CursorVisible;
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            MouseInput = true;

#elif UNITY_ANDROID
           _NewInputSystem = true;
           _SpeedLook = new Vector2(20, 20);
#endif

            //_ZoomAmount = Mathf.Clamp(_StartZoom, _ZoomRange.x, _ZoomRange.y);

            
        }

        private void OnValidate()
        {
            Cursor.lockState = _CursorLockMode;
            Cursor.visible = _CursorVisible;
        }

        private void Update()
        {
            InputProcess();

            if (_UpdateMethod != UpdateMethod.Update)
                return;

            MoveRotateProcess();


        }

        private void LateUpdate()
        {
            if (_UpdateMethod != UpdateMethod.Late)
                return;

            MoveRotateProcess();
        }

        private void FixedUpdate()
        {
            if (_UpdateMethod != UpdateMethod.Fixed)
                return;

            MoveRotateProcess();
        }
        #endregion


        private void InputProcess()
        {
            if (_InputEnabled == false)
                return;

            //_LookInputVector = Inatesi.Inputs.Input.GetVector(_InputName) * 10f;


            if (_MouseInput == true)
            {
                if (_NewInputSystem)
                {
                    var delta = new Vector2(
                        Inatesi.Inputs.Input.GetVector(_InputName).x, 
                        Inatesi.Inputs.Input.GetVector(_InputName).y
                    );
                    delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
                    delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.
                    _LookInputVector = delta;
                    //_LookInputVector = UnityEngine.InputSystem.Mouse.current.delta.ReadValue() * _SpeedLook.x;

                    _LookInputVector = Vector2.Lerp(
                            _LookInputVector,
                                new Vector2(
                                    Inatesi.Inputs.Input.GetVector(_InputName).x,
                                    Inatesi.Inputs.Input.GetVector(_InputName).y),
                                Time.deltaTime * 60
                        );
                }
                else
                {
                    try
                    {
                        /*_LookInputVector = new Vector2(
                            UnityEngine.Input.GetAxis(_HorizontalInputName) * _SpeedLook.x,
                            UnityEngine.Input.GetAxis(_VerticalInputName) * _SpeedLook.y
                        );*/

                        _LookInputVector = Vector2.Lerp(
                            _LookInputVector,
                                new Vector2(
                                    UnityEngine.Input.GetAxis(_HorizontalInputName),
                                    UnityEngine.Input.GetAxis(_VerticalInputName)),
                                Time.deltaTime * 60
                        );
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                _LookInputVector = Vector2.Lerp(
                            _LookInputVector,
                                new Vector2(
                                    UnityEngine.Input.GetAxis(_HorizontalInputName),
                                    UnityEngine.Input.GetAxis(_VerticalInputName)),
                                Time.deltaTime * 60
                        );
            }

            _LookRotationEuler += GameSettings.GameSettingsValue.MouseSens  * new Vector3(_LookInputVector.y, _LookInputVector.x, 0) + _ShakeRotation;
            //_LookRotationEuler += GameSettings.GameSettingsValue.MouseSens * _SpeedLook.x * new Vector3(_LookInputVector.y, _LookInputVector.x, 0) + _ShakeRotation;
            _LookRotationEuler = new Vector3(Mathf.Clamp(_LookRotationEuler.x, _LimitXRotation.x, _LimitXRotation.y), _LookRotationEuler.y, 0);
            _LookRotation = Quaternion.Euler(_LookRotationEuler.x, _LookRotationEuler.y, 0);
            
            if (_LookRotationEuler.y > 360)
            {
                _LookRotationEuler.y -= 360;
            }
            else if (_LookRotationEuler.y < 0)
            {
                _LookRotationEuler.y += 360;
            }

            if (Mathf.Abs(Inatesi.Inputs.Input.GetVector(_ScrollInputName).y) > 0)
            {
                _ZoomAmount -= Inatesi.Inputs.Input.GetVector(_ScrollInputName).y * _ZoomChangeValue;
                _ZoomAmount = Mathf.Clamp(_ZoomAmount, _ZoomRange.x, _ZoomRange.y);
            }
        }
        
        private void MoveRotateProcess()
        {
            var followPositionOrbit = GetFollowPositionOrbit();
            var followPosition = GetAnchorPosition();

            if (_ZoomAmount <= 0.01f) // first player camera ...... FPC
            {
                if (_Follow)
                {
                    transform.SetPositionAndRotation(
                        _Follow.forward * _CameraOffset.z + followPosition,
                        Quaternion.Euler(new Vector3(_LookRotationEuler.x * -1, _LookRotationEuler.y, 0) + _ShakeRotation)
                    );
                }
            }
            else
            {   
                if (_IsSmooth == true)
                {
                    transform.position = Vector3.Lerp(transform.position, followPositionOrbit, _SmoothTime * Time.deltaTime);
                }
                else
                {
                    transform.position = followPositionOrbit;
                }
                
                transform.LookAt(followPosition);
                //transform.RotateAround(_FollowPosition, Vector3.up, _LookVector.x);
            }


            _ShakeRotation = Vector3.Lerp(_ShakeRotation, Vector3.zero, Time.deltaTime * 10f);
        }

        private float GetDistance()
        {
            _VirtualPosition = GetAnchorPosition() - transform.forward * _ZoomAmount;

            var startPosition = GetAnchorPosition();
            var endPosition = _VirtualPosition;

            var startResultPosition = startPosition;
            var endResultPosition = endPosition;

            var dirToFollow = (endResultPosition - startResultPosition).normalized;
            var distanceFromFollow = _ZoomAmount;

            bool cast = Physics.SphereCast(
                startResultPosition,
                _Radius,
                dirToFollow,
                out _ColliderRaycast,
                distanceFromFollow,
                _Mask,
                QueryTriggerInteraction.Ignore
            );

            if (cast)
            {
                return _ColliderRaycast.distance;
            }

            return _ZoomAmount;
        }

        private Vector3 GetAnchorPosition()
        {
            if (_Follow == null)
                return Vector3.zero;

            return
                _Follow.position +
                (_AnchorOffset.x * (_LookRotation * -Vector3.right)) +
                (_Follow.up * _AnchorOffset.y) +
                (_AnchorOffset.z * (_LookRotation * Vector3.forward))
                ;
        }

        private Vector3 GetFollowPositionOrbit()
        {
            float distance = GetDistance();
            Vector3 orbit = Vector3.forward * distance;
            orbit = Quaternion.Euler(_LookRotationEuler.x, _LookRotationEuler.y, 0) * orbit;

            return GetAnchorPosition() + orbit;
        }

        public void Shake(Vector3 shake)
        {
            _ShakeLerp = shake;

            _ShakeRotation = _ShakeLerp;
        }
    }
}