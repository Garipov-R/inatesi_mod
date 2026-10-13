using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController3 : MonoBehaviour
    {
        [SerializeField] private List<WheelProperties> _wheelProperties = new();
        [SerializeField] private float _dampAmount = 1f;
        [SerializeField] private float _suspensionForce = 1f;
        [SerializeField] private float _suspensionForceClamp = 1f;
        [SerializeField] private float _speed = 100f;
        [SerializeField] private float _friction = 1f;
        [SerializeField] private float _wheelGripRight = 1f;
        [SerializeField] private float _wheelGripFront = 1f;


        [Header("Settings")]
        [SerializeField] private float _WheelBase = 2.629f;
        [SerializeField] private float _RearTrack = 1;

        [Serializable]
        public class WheelProperties
        {
            [SerializeField] private Transform _wheelStartPoint;
            [SerializeField] private Transform _wheel;

            [Header("Settings")]
            [SerializeField] private float _suspensionLength = 0.05f;
            [SerializeField] private float _size = 0.5f;
            [SerializeField] private float _turnAngle = 45f;
            [SerializeField] private float _brakeStrength = 0.5f;
            [SerializeField] private bool _isTurnable = false;
            [SerializeField] private bool _isMotor = false;


            [Header("other")]
            [SerializeField] private bool _isRight = false;
            [SerializeField] private bool _isLeft = false;

            [Header("Visual")]
            [SerializeField] private Transform _wheelVisualize;
            [SerializeField] private Vector3 _wheelRotateAxis = new Vector3(1, 0, 0);

            private float _brake;

            public float LastSuspensionLength { get => _lastSuspensionLength; set => _lastSuspensionLength = value; }
            public float SuspensionLength { get => _suspensionLength; set => _suspensionLength = value; }
            public float Size { get => _size; set => _size = value; }
            public Vector3 LocalPosition { get => _localPosition; set => _localPosition = value; }
            public Transform WheelTransform { get => _wheel; set => _wheel = value; }
            public Transform WheelVisualize { get => _wheelVisualize; set => _wheelVisualize = value; }
            public Transform WheelStartPoint { get => _wheelStartPoint; set => _wheelStartPoint = value; }
            public Vector3 WheelRotateAxis { get => _wheelRotateAxis; set => _wheelRotateAxis = value; }
            public float NormalForce { get => _normalForce; set => _normalForce = value; }
            public Vector3 LocalVelocity { get => _localVelocity; set => _localVelocity = value; }
            public float AngularVelocity { get => _angularVelocity; set => _angularVelocity = value; }
            public bool IsTurnable { get => _isTurnable; set => _isTurnable = value; }
            public float TurnAngle { get => _turnAngle; set => _turnAngle = value; }
            public bool IsRight { get => _isRight; set => _isRight = value; }
            public bool IsLeft { get => _isLeft; set => _isLeft = value; }
            public bool IsMotor { get => _isMotor; set => _isMotor = value; }
            public float BrakeStrength { get => _brakeStrength; set => _brakeStrength = value; }
            public float Brake { get => _brake; set => _brake = value; }

            private float _normalForce;
            private float _lastSuspensionLength;
            private Vector3 _localVelocity;
            private float _angularVelocity;
            private Vector3 _localPosition;

            WheelProperties()
            {
                _suspensionLength = .05f;
            }
        }

        private Vector2 _moveInput;
        private float _SteerAngle;
        private Rigidbody _rigidbody;
        private float _currentSpeed;

        private Vector3 _wheelVelocity;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            foreach (var wheel in _wheelProperties)
            {
                wheel.LocalPosition = wheel.WheelStartPoint.localPosition;
            }
        }

        private void FixedUpdate()
        {
            foreach (var wheel in _wheelProperties)
            {
                if (wheel.IsTurnable)
                {
                    float turnAngleResult = Mathf.Lerp(wheel.TurnAngle, 10, wheel.LocalVelocity.z / (30));

                    var TurnRadius = wheel.TurnAngle;

                    if (_moveInput.x > 0)
                    {
                        if (wheel.IsLeft)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (wheel.TurnAngle + (_RearTrack / 2))) * _moveInput.x;
                        }

                        if (wheel.IsRight)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (wheel.TurnAngle - (_RearTrack / 2))) * _moveInput.x;
                        }

                    }
                    else if (_moveInput.x < 0)
                    {
                        if (wheel.IsLeft)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (TurnRadius - (_RearTrack / 2))) * _moveInput.x;
                        }

                        if (wheel.IsRight)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (TurnRadius + (_RearTrack / 2))) * _moveInput.x;
                        }
                    }
                    else
                    {
                        _SteerAngle = 0;
                    }


                    /* wheel.WheelStartPoint.localEulerAngles = new Vector3(
                         wheel.WheelStartPoint.eulerAngles.x,
                         turnAngleResult * _moveInput.x,
                         wheel.WheelStartPoint.eulerAngles.z
                     );*/

                    //turnAngleResult = Mathf.Lerp(_SteerAngle, 10, wheel.localVelocity.z / (30));
                    turnAngleResult = _SteerAngle;

                    //wheel.WheelTransform.localRotation = Quaternion.Euler(0, turnAngleResult * _moveInput.x, 0);
                    wheel.WheelTransform.localRotation = Quaternion.Euler(0, turnAngleResult, 0);
                    //wheel.WheelTransform.localRotation = Quaternion.Euler(wheel.WheelTransform.eulerAngles.x, wheel.WheelStartPoint.eulerAngles.y, 0);
                    //wheel.WheelStartPoint.localRotation = Quaternion.Euler(0, turnAngleResult * _moveInput.x, 0);
                    wheel.WheelStartPoint.localRotation = Quaternion.Euler(0, turnAngleResult, 0);
                    //wheel.WheelStartPoint.localRotation = Quaternion.Euler(0, _SteerAngle, 0);
                }

                RaycastHit hit;
                float inertia = _rigidbody.mass * wheel.Size * wheel.Size / 2f;
                inertia = wheel.Size * wheel.Size / 2f;
                float rayLen = wheel.Size + wheel.SuspensionLength;
                var wheelWorldPosition = wheel.WheelTransform.TransformPoint(wheel.WheelStartPoint.localPosition);
                wheelWorldPosition = transform.TransformPoint(wheel.WheelStartPoint.localPosition);

                bool grounded = Physics.Raycast(wheelWorldPosition, -transform.up, out hit, rayLen);
                Vector3 velocityAtWheel = _rigidbody.GetPointVelocity(wheelWorldPosition);
                wheel.LocalVelocity = wheel.WheelStartPoint.InverseTransformDirection(velocityAtWheel);
                float lateralVel = wheel.LocalVelocity.x;
                Vector3 worldVelAtHit = _rigidbody.GetPointVelocity(hit.point);
                float lateralHitVel = wheel.WheelStartPoint.InverseTransformDirection(worldVelAtHit).x;
                float lateralFriction = -_wheelGripRight * lateralVel - 2f * lateralHitVel;
                //lateralFriction = lateralHitVel;
                float longitudinalFriction = -_wheelGripFront * (wheel.LocalVelocity.z - wheel.AngularVelocity * wheel.Size);
                longitudinalFriction = _wheelGripFront * ((wheel.AngularVelocity ) * wheel.Size);
                float groundTorque = grounded ? longitudinalFriction * wheel.Size : 0f;
                float moveInput = _moveInput.y;



                if (wheel.IsMotor)
                {
                    if (Mathf.Abs(moveInput) > 0)
                    {
                        _currentSpeed = Mathf.Lerp(_currentSpeed, _speed, Time.fixedDeltaTime * 15f);
                    }
                    else
                    {
                        _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.fixedDeltaTime * 1f);
                    }

                    wheel.AngularVelocity += ((_currentSpeed - groundTorque) * moveInput) / inertia * Time.fixedDeltaTime;
                }

                wheel.Brake = 1;
                wheel.AngularVelocity *= 1 - wheel.Brake * wheel.BrakeStrength * Time.fixedDeltaTime;


                Vector3 totalLocalForce = new Vector3(lateralFriction, 0f, longitudinalFriction)
                    * wheel.NormalForce * _friction * Time.fixedDeltaTime;

                float currentMaxFrictionForce = wheel.NormalForce * _friction;
                bool slidding = totalLocalForce.magnitude > currentMaxFrictionForce;
                totalLocalForce = Vector3.ClampMagnitude(totalLocalForce, currentMaxFrictionForce);
                //totalLocalForce *= slidding ? (9.8f / coefStaticFriction) : 1;
                totalLocalForce *= slidding ? (.1f / _friction) : 1;

                Vector3 totalWorldForce = wheel.WheelStartPoint.TransformDirection(totalLocalForce);


                if (grounded)
                {
                    float compression = rayLen - hit.distance;
                    float damping = (wheel.LastSuspensionLength - hit.distance) * _dampAmount;
                    wheel.NormalForce = (compression + damping) * _suspensionForce;
                    wheel.NormalForce = Mathf.Clamp(wheel.NormalForce, 0f, _suspensionForceClamp);

                    Vector3 springDir = hit.normal * wheel.NormalForce;
                    //wheel.suspensionForceDirection = springDir;

                    //_rigidbody.AddForceAtPosition(springDir + totalWorldForce, hit.point);
                    //_rigidbody.AddForceAtPosition(springDir + totalWorldForce, hit.point, ForceMode.Impulse);
                    _rigidbody.AddForceAtPosition(springDir + totalWorldForce, hit.point, ForceMode.Impulse);
                    //_rigidbody.AddForceAtPosition(springDir, hit.point, ForceMode.Impulse);
                    //_rigidbody.AddForceAtPosition(springDir, hit.point);

                    wheel.LastSuspensionLength = hit.distance;

                    wheel.WheelTransform.position = hit.point + transform.up * wheel.Size;

                    Debug.DrawLine(wheelWorldPosition, wheelWorldPosition + -transform.up * hit.distance, Color.green);
                }
                else
                {
                    wheel.WheelTransform.position = wheelWorldPosition + transform.up * (wheel.Size - rayLen);

                    Debug.DrawLine(wheelWorldPosition, wheelWorldPosition + -transform.up * rayLen, Color.red);
                }

                if (wheel.WheelVisualize)
                {
                    float r = (wheel.AngularVelocity + wheel.LocalVelocity.z) * Mathf.Rad2Deg * Time.fixedDeltaTime;
                    wheel.WheelVisualize.Rotate(wheel.WheelRotateAxis, r, Space.Self);
                }
            }
        }

        public void Move(Vector2 move)
        {
            //_moveInput = move;

            move.x = Mathf.Abs(move.x) > 0 ? Mathf.Clamp(move.x * 2, -1, 1) : 0;
            move.y = Mathf.Abs(move.y) > 0 ? Mathf.Clamp(move.y * 2, -1, 1) : 0;

            _moveInput.x = Mathf.Lerp(_moveInput.x, move.x, Time.deltaTime * 10.501f);
            _moveInput.y = move.y;
        }

        public void Force(Vector3 force)
        {
            _rigidbody.AddForce(force);
            _rigidbody.AddTorque(Vector3.forward * 50);

            _rigidbody.isKinematic = true;
            transform.position = new Vector3(0, 10, 0);
            _rigidbody.isKinematic = false;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_wheelProperties != null)
            {
                foreach (var wheel in _wheelProperties)
                {
                    if (wheel.WheelStartPoint == null)
                        continue;

                    Gizmos.DrawWireSphere(wheel.WheelStartPoint.position, wheel.Size);
                }
            }


            Vector3 frontCenter = transform.position + transform.forward * (_WheelBase / 2);
            Vector3 rearCenter = transform.position - transform.forward * (_WheelBase / 2);

            Gizmos.color = Color.sandyBrown;
            Gizmos.DrawLine(rearCenter - transform.right * (_RearTrack), frontCenter - transform.right * (_RearTrack));

            Gizmos.color = Color.brown;
            Gizmos.DrawLine(rearCenter - transform.right * (_RearTrack / 2) - transform.forward * (_WheelBase / 3), rearCenter + transform.right * (_RearTrack / 2) - transform.forward * (_WheelBase / 3f));
        }

#endif

    }
}