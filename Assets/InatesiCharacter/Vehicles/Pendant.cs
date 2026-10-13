using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

namespace InatesiCharacter.Vehicles
{
    public class Pendant : MonoBehaviour
    {
        public Rigidbody Body;
        public float Amount = 50;

        public bool isFrontLeft;
        public bool isFrontRight;
        public bool isRearLeft;
        public bool isRearRight;

        public float SteerAngle;

        [Header("Wheel")]

        public float WheelRadius;
        [Header("Suspension")]
        public float RestLength = 0.5f; // Length of the spring when it is not contracted or extracted 
        public float SpringTravel = 0.2f; // Length that the spring can contract and extract;
        public float SpringStiffness;
        public float DamperStiffness;

        private Vector3 _wheelVelocity;
        private Vector3 _wheelForce;

        private float _minLength;
        private float _maxLength;
        private float _lastLength;

        private float _springLength;
        private float _springForce;
        private float _springVelocity;

        private float _damperForce;

        private Vector3 _suspensionForce;

        InputAction moveAction;
        private float _accelerationInput;

        private Transform _wheelMesh;

        private Vector3 _hitPoint;

        public float InputMove;
        private float _linearDumping;


        private void Start()
        {
            _minLength = RestLength - SpringTravel;
            _maxLength = RestLength + SpringTravel;

            moveAction = InputSystem.actions.FindAction("Move");
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            _minLength = RestLength - SpringTravel;
            _maxLength = RestLength + SpringTravel;

        }

#endif 

        private void Update()
        {
            _accelerationInput = GetInput();

            transform.localRotation = Quaternion.Euler(
                                        transform.localRotation.x,
                                        transform.localRotation.y + SteerAngle,
                                        transform.localRotation.z);



            /*            // using (Draw.WithLineWidth(7)) {
                        *//*Draw.Line(transform.position + transform.up * 0.1f, transform.position +
                                                                            ((_wheelForce.x * transform.forward) / 3000)  + transform.up * 0.1f, Color.dodgerBlue);
                        Draw.Line(transform.position, transform.position + (_wheelForce.y * -transform.right) /3000, Color.red);
                        Draw.Line(Body.transform.position, Body.transform.position + Body.linearVelocity, Color.darkOrange);*//*

                        //     Draw.Line(_hitPoint, _hitPoint + _suspensionForce / 3000, Color.green);
                        // }

                        *//*using (Draw.WithLineWidth(2)) {
                            if (isFrontLeft ||  isFrontRight)
                                Draw.Arrow(transform.position, transform.position + transform.forward * 1.5f, Color.green);
                        }*/
            

            _wheelMesh = transform.GetChild(0);
        }

        private void FixedUpdate()
        {


            if (Vector3.Scale(Body.angularVelocity, new Vector3(1, 0, 1)).magnitude <= 0)
            {
                _linearDumping = Mathf.Lerp(
                    _linearDumping,
                    0,
                    Time.deltaTime * 11.1f
                );
            }
            else
            {
                _linearDumping = Mathf.Lerp(_linearDumping, 1, Time.deltaTime * .1f);

            }


            Debug.DrawLine(transform.position, transform.position + (-Body.transform.up * (  + WheelRadius)), Color.green);
            if (Physics.SphereCast(
                transform.position ,
                WheelRadius,
                - Body.transform.up, 
                out RaycastHit hit, 
                _maxLength + WheelRadius))
            {
                //_linearDumping = Mathf.Lerp(_linearDumping, 1f, Time.deltaTime * .1f);


                _lastLength = _springLength;

                _springLength = hit.distance - WheelRadius; // if pshysics raycast
                _springLength = hit.distance;
                _springLength = Mathf.Clamp(_springLength, _minLength / 2, _maxLength);

                _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime * 1f;

                _springForce = SpringStiffness * (RestLength - _springLength);
                //_springForce *= Body.mass * .01f;
                //Debug.Log(_springForce);
                _damperForce = DamperStiffness * _springVelocity;

                //_damperForce = Mathf.Clamp(_damperForce, 0f, DamperStiffness);
                //_suspensionForce = (_springForce + _damperForce) * Body.transform.up;
                _suspensionForce = (Mathf.Clamp(_springForce, -1, SpringStiffness) + _damperForce) * Body.transform.up;
                //_suspensionForce *= Body.mass * .03f;

                var vel = Body.GetPointVelocity(hit.point);
                _wheelVelocity = transform.InverseTransformDirection(vel);
                _wheelVelocity *= _linearDumping;


                
                if (Vector3.Angle(Body.transform.up, Vector3.up) > 10f)
                {
                    //_springForce = 0;
                }

                _wheelForce.x = (_accelerationInput * 1f) * _springForce;
                //_wheelForce.x *= Body.mass / 20;


                var accelLerp = Mathf.Lerp(1f, 1f, _accelerationInput);

                //_wheelForce.y = hit.distance > _lastLength - 1f ? _wheelVelocity.x * _springForce * 1 : 0f;
                _wheelForce.y = _wheelVelocity.x * (_springForce );
                //_wheselForce.y *= Body.mass / 100;


                //_wheelForce = Vector3.ClampMagnitude(_wheelForce, 1f);
                //_wheelForce.y = Mathf.Clamp(_wheelForce.y, -50, 50);

                //_wheelForce.x *= 2f;
                //_wheelForce.y = 1f ;

                if (hit.distance < _maxLength - .1f)
                {

                    Body.AddForceAtPosition(_suspensionForce +
                                        (_wheelForce.x * transform.forward)
                                       + (_wheelForce.y * -transform.right),
                    hit.point, ForceMode.Acceleration);
                }



                Debug.DrawLine(transform.position, transform.position + _wheelVelocity, Color.dodgerBlue);
                Debug.DrawLine(transform.position, transform.position + Body.GetPointVelocity(hit.point), Color.red);

                Vector3 pos = transform.position + (_springLength) * -Body.transform.up;
                _wheelMesh.position = pos;
                _hitPoint = hit.point;

            }
            else
            {
                //_wheelMesh.position = transform.position; 
                _wheelForce = Vector3.zero;

                _linearDumping = 0;
            }


            //Body.linearVelocity *= _linearDumping  ;


            /*
            if (Physics.Raycast(transform.position, -Body.transform.up, out RaycastHit hit, _maxLength + WheelRadius))
            {
                _springLength = hit.distance - WheelRadius;

                _lastLength = _springLength;
                _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;

                _springForce = SpringStiffness * (RestLength - _springLength);
                damperForce = DamperStiffness * _springVelocity;

                _suspensionForce = (_springForce + _damperForce) * Body.transform.up;


                Body.AddForceAtPosition(_suspensionForce, hit.point);

                //  _wheelMesh.position = hit.point;


            }
            */


            // _wheelMesh.localPosition = new Vector3(_wheelMesh.localPosition.x, (_springLength - WheelRadius), _wheelMesh.localPosition.z);
        }

        private float GetInput()
        {
            float target = InputMove;

            float accel = Mathf.Abs(target) > 0.01f
                ? 3f
                : 1f;

            if (Mathf.Abs(target) > 0.01f)
            {
                _linearDumping = 1;
            }

            return Mathf.MoveTowards(
                _accelerationInput,
                target,
                accel * Time.deltaTime
            );
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;

            Gizmos.color = Color.black;
            Gizmos.DrawCube(position, Vector3.one * 0.1f);

            Gizmos.color = Color.gray;
            Gizmos.DrawCube(position + RestLength * -Body.transform.up, Vector3.one * 0.1f);

            Gizmos.color = Color.white;
            Gizmos.DrawCube(position + (_springLength + WheelRadius) * -Body.transform.up, Vector3.one * 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(position + (_springLength) * -Body.transform.up, Vector3.one * 0.1f);

            Gizmos.DrawSphere(position + (_springLength) * -Body.transform.up, WheelRadius);



            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + transform.right * 0.5f, transform.position + transform.right * 0.5f +
                                                                            ((_wheelForce.x * transform.forward) ) + transform.up * 0.1f);
            
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, transform.position + (_wheelForce.y * -transform.right));


            Gizmos.color = Color.deepPink;
            Gizmos.DrawLine(_hitPoint, _hitPoint + _suspensionForce);


            Gizmos.color = Color.antiqueWhite;
            Gizmos.DrawLine(Body.transform.position, Body.transform.position + Body.linearVelocity);

            if (isFrontLeft || isFrontRight)
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);
        }

    }
}