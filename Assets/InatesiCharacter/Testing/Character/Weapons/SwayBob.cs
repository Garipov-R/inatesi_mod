using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    [System.Serializable]
    public class SwayBob
    {
        [SerializeField] private float _Smooth = 7f;
        [SerializeField] private float _SmoothRot = 11f;

        [Header("Start")]
        [SerializeField] private Vector3 _PickPosition = new Vector3(0.32f, -1f, 0.45f);
        [SerializeField] private Vector3 _PickRotation = Vector3.zero;

        [Header("Transform")]
        [SerializeField] private Vector3 _Position = new Vector3(0.32f, -0.45f, 0.45f);
        [SerializeField] private Vector3 _Rotation = Vector3.zero;

        [Header("Sway")]
        [SerializeField] private float _step = 0.01f;
        [SerializeField] private float _maxXStepDistance = 0.3f;
        [SerializeField] private float _maxYStepDistance = 0.3f;

        [Header("Sway Rot")]
        [SerializeField] private float _stepRotation = 4;
        [SerializeField] private float _maxStepDistanceRotation = 6;

        [Header("Bobbing")]
        [SerializeField] private float _bobExaggeration = 10f;
        [SerializeField] private Vector3 _travelLimit = Vector3.one * 0.025f;
        [SerializeField] private Vector3 _bobLimit = Vector3.one * 0.01f;

        private float speedCurve;
        private float curveSin { get => Mathf.Sin(speedCurve); }
        private float curveCos { get => Mathf.Cos(speedCurve); }

        
        private Vector3 _bobPosition;

        [Header("Bob Rotation")]
        [SerializeField] private Vector3 _multiplier = new Vector3(5, 5, 5);
        private Vector3 _bobEulerRotation;

        [Header("Shake")]
        [SerializeField] private float _SmoothShake = 4f;
        [SerializeField] private Vector3 _ForceShake = new Vector3(.3f, .3f, .3f);
        [SerializeField] private Vector3 _ForceRotateShake = new Vector3(30f, 0, 3f);
        [SerializeField] private Vector3 _MaxLimeitShake = new Vector3(.3f, .3f, .3f);

        [Header("Offset")]
        [SerializeField] private Vector3 _OffsetRotation;
        [SerializeField] private Vector3 _OffsetPoosition;

        private bool _isOffset = false;
        private GameObject _viewModel;



        private Vector3 _swayPos;
        private Vector3 _swayEulerRot;
        private Vector3 _startPosition;
        private Quaternion _startRotation;


        private Vector3 _shake;
        private Quaternion _shakeRot;

        public Vector3 SwayPos { get => _swayPos; set => _swayPos = value; }
        public float Smooth { get => _Smooth; set => _Smooth = value; }
        public float SmoothRot { get => _SmoothRot; set => _SmoothRot = value; }
        public Vector3 SwayEulerRot { get => _swayEulerRot; set => _swayEulerRot = value; }
        public Vector3 StartPosition { get => _startPosition; set => _startPosition = value; }
        public Quaternion StartRotation { get => _startRotation; set => _startRotation = value; }
        public Vector3 BobPosition { get => _bobPosition; set => _bobPosition = value; }
        public Vector3 BobEulerRotation { get => _bobEulerRotation; set => _bobEulerRotation = value; }
        public bool IsOffset { get => _isOffset; set => _isOffset = value; }
        public Vector3 ForceShake { get => _ForceShake; set => _ForceShake = value; }
        public Vector3 Position { get => _Position; set => _Position = value; }
        public Vector3 Rotation { get => _Rotation; set => _Rotation = value; }


        public void Init(GameObject g)
        {
            g.transform.localPosition = _Position;
            g.transform.localEulerAngles = _Rotation;

            _startPosition = g.transform.localPosition;
            _startRotation = g.transform.localRotation;

            _viewModel = g;
        }

        public void Shake()
        {
            _shake.x = Mathf.Clamp(_ForceShake.x, -_MaxLimeitShake.x, _MaxLimeitShake.x);
            _shake.y = Mathf.Clamp(_ForceShake.y, -_MaxLimeitShake.y, _MaxLimeitShake.y);
            _shake.z = Mathf.Clamp(_ForceShake.z, -_MaxLimeitShake.z, _MaxLimeitShake.z);

            _shakeRot = Quaternion.Euler(
                _ForceRotateShake.x,
                _ForceRotateShake.y,
                _ForceRotateShake.z
            );
        }

        public void Shake(float force)
        {
            _shake.z = force;
        }
        public void Shake(Vector3 force)
        {
            _shake = force;

            //_shake.x = Mathf.Clamp(_shake.x, -_MaxLimeitShake.x, _MaxLimeitShake.x);
            //_shake.y = Mathf.Clamp(_shake.y, -_MaxLimeitShake.y, _MaxLimeitShake.y);
            //_shake.z = Mathf.Clamp(_shake.z, -_MaxLimeitShake.z, _MaxLimeitShake.z);
        }

        public void ShakeProccess()
        {
            //_shake.z = Mathf.Lerp(_shake.z, 0, Time.deltaTime * _SmoothShake);
            _shake = Vector3.Lerp(_shake, Vector3.zero, Time.deltaTime * _SmoothShake);
            _shakeRot = Quaternion.Lerp(_shakeRot, Quaternion.identity, Time.deltaTime * _SmoothShake);
        }

        public void Sway(Vector2 lookInput)
        {
            Vector3 invertLook = lookInput * -_step;

            invertLook.x = Mathf.Clamp(invertLook.x, -_maxXStepDistance, _maxXStepDistance);
            invertLook.y = Mathf.Clamp(invertLook.y, -_maxYStepDistance, _maxYStepDistance);

            var offset = _isOffset ? _OffsetPoosition : Vector3.zero;
            _swayPos = invertLook + _startPosition + _shake + offset;
            //_swayPos = invertLook;
        }

        public void SwayRotation(Vector2 lookInput)
        {
            Vector2 invertLook = lookInput * -_stepRotation;

            invertLook.x = Mathf.Clamp(invertLook.x, -_maxStepDistanceRotation, _maxStepDistanceRotation);
            invertLook.y = Mathf.Clamp(invertLook.y, -_maxStepDistanceRotation, _maxStepDistanceRotation);

            //_swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x) + _startRotation.eulerAngles;

            var offset = _isOffset ? _OffsetRotation : Vector3.zero;
            _swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x) + _shakeRot.eulerAngles + offset;
        }

        public void Movement(Vector2 walkInput, bool isGrounded)
        {
            //speedCurve += Time.deltaTime * (isGrounded ? (walkInput.x + walkInput.y) * _bobExaggeration : 1f) + 0.01f;
            speedCurve += Time.deltaTime * (isGrounded ? walkInput.magnitude * _bobExaggeration : 1f) + 0.01f;

            _bobPosition.x = (curveCos * _bobLimit.x * (isGrounded ? 1 : 0)) - (walkInput.x * _travelLimit.x);
            _bobPosition.y = (curveSin * _bobLimit.y) - ( Mathf.Abs(walkInput.y) * _travelLimit.y);
            _bobPosition.z = -(walkInput.y * _travelLimit.z);

            //_bobPosition = _swayPos;
        }

        public void BobRotation(Vector2 walkInput)
        {
            _bobEulerRotation.x = (walkInput != Vector2.zero ? _multiplier.x * (Mathf.Sin(2 * speedCurve)) : _multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
            _bobEulerRotation.y = (walkInput != Vector2.zero ? _multiplier.y * curveCos : 0);
            _bobEulerRotation.z = (walkInput != Vector2.zero ? _multiplier.z * curveCos * walkInput.y : 0);


            //_bobEulerRotation = _bobEulerRotation + _startRotation.eulerAngles;
        }

        public void Update()
        {
            if (_viewModel != null)
            {
                _viewModel.transform.localPosition = Vector3.Lerp(
                    _viewModel.transform.localPosition,
                    _swayPos + _bobPosition,
                    Time.deltaTime * _Smooth
                );

                _viewModel.transform.localRotation = Quaternion.Slerp(
                    _viewModel.transform.localRotation,
                    Quaternion.Euler(_swayEulerRot) * Quaternion.Euler(_bobEulerRotation),
                    Time.deltaTime * _SmoothRot
                );
            }
        }
    }
}
