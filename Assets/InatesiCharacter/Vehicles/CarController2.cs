using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController2 : MonoBehaviour
    {
        private Rigidbody _rigidbody;


        [Serializable]
        public class WheelProperties
        {
            [SerializeField] private Transform _wheelStartPoint;
            [SerializeField] private Transform _wheel;
            [SerializeField] private float _suspensionLength = 0.5f;
            [SerializeField] private float _size = 0.5f;

            [Header("Visual")]
            [SerializeField] private Transform _wheelVisualize;
            [SerializeField] private Vector3 _wheelRotateAxis = new Vector3(1,0,0);
            

            private float _lastSuspensionLength = 0.0f;
            private Vector3 _localPosition;

            public float LastSuspensionLength { get => _lastSuspensionLength; set => _lastSuspensionLength = value; }
            public float SuspensionLength { get => _suspensionLength; set => _suspensionLength = value; }
            public float Size { get => _size; set => _size = value; }
            public Vector3 LocalPosition { get => _localPosition; set => _localPosition = value; }
            public Transform WheelTransform { get => _wheel; set => _wheel = value; }
            public Transform WheelVisualize { get => _wheelVisualize; set => _wheelVisualize = value; }
            public Transform WheelStartPoint { get => _wheelStartPoint; set => _wheelStartPoint = value; }
            public Vector3 WheelRotateAxis { get => _wheelRotateAxis; set => _wheelRotateAxis = value; }

            public float normalForce;
            public float lastSuspensionLength;
            public Vector3 localVelocity;
            public float angularVelocity;
            public float turnAngle = 45f;
            public bool isFrontLeft;
            public bool isFrontRight;

            public float brakeStrength = 0.5f;
            [HideInInspector] public float brake = 0;


            WheelProperties()
            {
                _suspensionLength = .5f;
            }
        }

        [SerializeField] private List<WheelProperties> _wheelProperties = new();
        [SerializeField] private float dampAmount = 1f;
        [SerializeField] private float suspensionForce = 1f;
        [SerializeField] private float suspensionForceClamp = 1f;
        [SerializeField] private float coefStaticFriction = 1f;
        [SerializeField] private float longitudinalFriction = 1f;
        [SerializeField] private float wheelGripX = 1f;
        [SerializeField] private float wheelGripZ = 1f;
        [SerializeField] private float speed = 100f;

        [Header("Settings")]
        [SerializeField] private float _WheelBase = 2.629f;
        [SerializeField] private float _RearTrack = 0;


        private Vector2 _moveInput;
        private bool _isBreaking;
        private float _SteerAngle;
        private float _currentSpeed;

        public Vector2 MoveInput { get => _moveInput; set => _moveInput = value; }
        public bool IsBreaking { get => _isBreaking; set => _isBreaking = value; }


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            foreach (var wheel in _wheelProperties)
            {
                wheel.LocalPosition = wheel.WheelStartPoint.localPosition;
            }
        }

        private void Update()
        {
            foreach (var wheel in _wheelProperties)
            {


                wheel.brake = _isBreaking ? 1 : 0;
            }
            /*if (_moveInput.y > 0)
            {
                _angleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * _steerInput;
                _angleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * _steerInput;
            }
            else if (_moveInput.y < 0)
            {
                _angleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * _steerInput;
                _angleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * _steerInput;
            }
            else
            {
                _angleLeft = 0;
                _angleRight = 0;
            }

            foreach (var wheel in _wheelProperties)
            {
                if (wheel.isFrontLeft) wheel.SteerAngle = _angleLeft;
                if (wheel.isFrontRight) wheel.SteerAngle = _angleRight;
            }*/
        }

        private void FixedUpdate()
        {
            foreach (var wheel in _wheelProperties)
            {
                if (wheel.isFrontLeft || wheel.isFrontRight)
                {
                    float turnAngleResult = Mathf.Lerp(wheel.turnAngle, 10, wheel.localVelocity.z / (30));

                    if (Mathf.Abs(wheel.localVelocity.x) > 14f)
                    {
                        //turnAngleResult = wheel.turnAngle;
                    }

                    //turnAngleResult = turnAngleResult * _moveInput.x;


                    var TurnRadius = wheel.turnAngle;
                    //Debug.Log(_moveInput.x);
                    if (_moveInput.x > 0)
                    {
                        if (wheel.isFrontLeft)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (TurnRadius + (_RearTrack / 2))) * _moveInput.x;
                        }

                        if (wheel.isFrontRight)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (TurnRadius - (_RearTrack / 2))) * _moveInput.x;
                        }

                    }
                    else if (_moveInput.x  < 0)
                    {
                        if (wheel.isFrontLeft)
                        {
                            _SteerAngle = Mathf.Rad2Deg * Mathf.Atan(_WheelBase / (TurnRadius - (_RearTrack / 2))) * _moveInput.x;
                        }

                        if (wheel.isFrontRight)
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
                float inertia = 1f * wheel.Size * wheel.Size / 2f;
                inertia =  _rigidbody.mass * wheel.Size * wheel.Size / 2f;
                float rayLen = wheel.Size * 1f + wheel.SuspensionLength;
                var wheelWorldPosition = wheel.WheelTransform.TransformPoint(wheel.WheelStartPoint.localPosition);
                //wheelWorldPosition = wheel.WheelTransform.position;
                wheelWorldPosition = transform.TransformPoint(wheel.WheelStartPoint.localPosition);
                bool grounded = Physics.Raycast(wheelWorldPosition, -transform.up, out hit, rayLen);

                Vector3 velocityAtWheel = _rigidbody.GetPointVelocity(wheelWorldPosition);
                wheel.localVelocity = wheel.WheelStartPoint.InverseTransformDirection(velocityAtWheel);
                float lateralVel = wheel.localVelocity.x;
                Vector3 worldVelAtHit = _rigidbody.GetPointVelocity(hit.point);
                float lateralHitVel = wheel.WheelStartPoint.InverseTransformDirection(worldVelAtHit).x;
                float lateralFriction = -wheelGripX * lateralVel - 1 * lateralHitVel;

                float longitudinalFriction = -wheelGripZ * (wheel.localVelocity.z - wheel.angularVelocity * wheel.Size);
                //longitudinalFriction = -wheel.Size * (wheel.localVelocity.z - wheel.angularVelocity);

                float groundTorque = grounded ? longitudinalFriction * wheel.Size : 0f;
                groundTorque = grounded ? longitudinalFriction : 0f;

                float moveInput = _moveInput.y;
                if (wheel.isFrontLeft || wheel.isFrontRight)
                {
                    moveInput = 0;
                }

                float speedd = speed * 10;
                if (moveInput < 0)
                {
                    speedd /= 1.4f;
                }

                if (Mathf.Abs(moveInput) > 0)
                {
                    _currentSpeed = Mathf.Lerp(_currentSpeed, speedd, Time.fixedDeltaTime * 5f);
                }
                else
                {
                    _currentSpeed = Mathf.Lerp(_currentSpeed, speedd, Time.fixedDeltaTime * 10f);
                }

                //wheel.angularVelocity += ((speedd - groundTorque) * moveInput) / inertia * Time.fixedDeltaTime;
                wheel.angularVelocity += ((_currentSpeed - groundTorque) * moveInput) / inertia * Time.fixedDeltaTime;
                //wheel.angularVelocity +=  ((_currentSpeed - groundTorque) * moveInput) * Time.fixedDeltaTime;
                //wheel.angularVelocity *= 1 - wheel.brake * wheel.brakeStrength * Time.fixedDeltaTime;

                //wheel.angularVelocity = Mathf.Clamp(wheel.angularVelocity, -speed / 2, speed);


                float brake = 1;

                //brake = 1;
/*
                if (Mathf.Abs(wheel.localVelocity.x) > 2)
                {
                    brake = Mathf.Lerp(1, 30.30f, wheel.localVelocity.x / 2);
                    if (!wheel.isFrontLeft && !wheel.isFrontRight)
                    {
                        lateralFriction = Mathf.Lerp(lateralFriction, lateralFriction * .1f, (Mathf.Abs(wheel.localVelocity.x) / 2));
                        //brake = 0;
                    }
                    else
                    {
                        lateralFriction = Mathf.Lerp(lateralFriction, lateralFriction * .1f, Mathf.Abs(wheel.localVelocity.x) / 2);
                        brake = 1;
                    }
                }*/
                

                wheel.angularVelocity *= 1 - brake * wheel.brakeStrength * Time.fixedDeltaTime;


                Vector3 totalLocalForce = new Vector3(lateralFriction, 0f, longitudinalFriction)
                    * wheel.normalForce * coefStaticFriction * Time.fixedDeltaTime;
                /*totalLocalForce = new Vector3(lateralFriction, 0f, longitudinalFriction)
                    * wheel.normalForce * 0.1f;*/
                //totalLocalForce = new Vector3(_moveInput.x, 0, _moveInput.y) * 60f * Time.fixedDeltaTime;


                float currentMaxFrictionForce = wheel.normalForce * coefStaticFriction;
                bool slidding = totalLocalForce.magnitude > currentMaxFrictionForce;
                //totalLocalForce = Vector3.ClampMagnitude(totalLocalForce, currentMaxFrictionForce);
                //totalLocalForce *= slidding ? (9.8f / coefStaticFriction) : 1;
                totalLocalForce *= slidding ? (.4f) : 1;


                Vector3 totalWorldForce = wheel.WheelStartPoint.TransformDirection(totalLocalForce);



                Debug.DrawRay(wheel.WheelTransform.position, wheel.WheelStartPoint.TransformDirection(totalLocalForce), Color.red);



                //var vel = _rigidbody.GetPointVelocity(hit.point);
                //wheel.localVelocity = wheel.WheelTransform.InverseTransformDirection(vel);

                if (grounded)
                {
                    float compression = rayLen - hit.distance;
                    float damping = (wheel.lastSuspensionLength - hit.distance) * dampAmount;
                    wheel.normalForce = (compression + damping) * suspensionForce;
                    wheel.normalForce = Mathf.Clamp(wheel.normalForce, 0f, suspensionForceClamp);

                    Vector3 springDir = hit.normal * wheel.normalForce;
                    //wheel.suspensionForceDirection = springDir;
                    
                    _rigidbody.AddForceAtPosition(springDir + totalWorldForce, hit.point, ForceMode.Impulse);
                    //_rigidbody.AddForceAtPosition(springDir, hit.point);

                    wheel.lastSuspensionLength = hit.distance;

                    wheel.WheelTransform.position = hit.point + transform.up * wheel.Size;

                    Debug.DrawLine(wheelWorldPosition, wheelWorldPosition + -transform.up * hit.distance, Color.green);
                }
                else
                {
                    wheel.WheelTransform.position = wheelWorldPosition + transform.up * (wheel.Size - rayLen);

                    Debug.DrawLine(wheelWorldPosition, wheelWorldPosition + -transform.up * rayLen, Color.red);
                }

                Transform wheelVisual = wheel.WheelTransform;
                /*wheelVisual.Rotate(
                    Vector3.right,
                    (wheel.angularVelocity + wheel.localVelocity.z) * Mathf.Rad2Deg * Time.fixedDeltaTime,
                    Space.Self
                );*/


                if (wheel.WheelVisualize)
                {
                    float r = (wheel.angularVelocity + wheel.localVelocity.z) * Mathf.Rad2Deg * Time.fixedDeltaTime;
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