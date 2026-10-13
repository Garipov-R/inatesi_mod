namespace InatesiCharacter.Vehicles
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CarController : MonoBehaviour
    {
        public Rigidbody Rigidbody;
        public Pendant[] Wheels;

        public float WheelBase = 2.629f;
        public float RearTrack;
        public float TurnRadius;

        private float _steerInput;

        private float _angleLeft;
        private float _angleRight;

        public float MoveInput {  get; set; }

        InputAction moveAction;

        [SerializeField] private float steerAcceleration = 8f;
        [SerializeField] private float steerDeceleration = 12f;

        private float _steerVelocity;
        private Vector3 _currentDampVelocity;


        void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            _steerInput = GetInput();

            if (_steerInput > 0)
            {
                _angleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * _steerInput;
                _angleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * _steerInput;
            }
            else if (_steerInput < 0)
            {
                _angleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * _steerInput;
                _angleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * _steerInput;
            }
            else
            {
                _angleLeft = 0;
                _angleRight = 0;
            }

            foreach (var wheel in Wheels)
            {
                if (wheel.isFrontLeft) wheel.SteerAngle = _angleLeft;
                if (wheel.isFrontRight) wheel.SteerAngle = _angleRight;
            }

        }

        private void FixedUpdate()
        {
            float minX = -60;
            float maxX = 60;

            // Get current Euler angles
            Vector3 currentAngles = Rigidbody.rotation.eulerAngles;

            // Normalize angles from 0-360 range to -180 to 180 range
            float angleX = currentAngles.x;
            if (angleX > 180f) angleX -= 360f;
            float angleZ = currentAngles.z;
            if (angleZ > 180f) angleZ -= 360f;

            // Clamp the angle
            float clampedX = Mathf.Clamp(angleX, minX, maxX);
            float clampedZ = Mathf.Clamp(angleZ, minX, maxX);

            // Reapply the clamped rotation to the Rigidbody safely
            Quaternion clampedRotation = Quaternion.Euler(clampedX, currentAngles.y, clampedZ);
            Rigidbody.MoveRotation(clampedRotation);

            // OPTIONAL: Kill angular velocity if it hits the limit to prevent bouncing/jittering
            if (angleX <= minX || angleX >= maxX
                || angleZ <= minX || angleZ >= maxX
                )
            {
                Rigidbody.angularVelocity = new Vector3(
                    0f,
                    Rigidbody.angularVelocity.y,
                    0f
                );
            }


            

        }

        public void Move(Vector2 input)
        {
            MoveInput = input.x;

            if (input.y > 0)
            {
                //MoveInput = 
            }

            foreach (var wheel in Wheels)
            {
                wheel.InputMove = input.y;
            }
        }

        private float GetInput()
        {
            float target = MoveInput;

            float accel = Mathf.Abs(target) > 0.01f
                ? steerAcceleration
                : steerDeceleration;

            return Mathf.MoveTowards(
                _steerInput,
                target,
                accel * Time.deltaTime
            );
        }
        private void OnDrawGizmos()
        {
            Vector3 frontCenter = transform.position + transform.forward * (WheelBase / 2);
            Vector3 rearCenter = transform.position - transform.forward * (WheelBase / 2);

            Gizmos.color = Color.sandyBrown;
            Gizmos.DrawLine(rearCenter - transform.right * (RearTrack), frontCenter - transform.right * (RearTrack));

            Gizmos.color = Color.brown;
            Gizmos.DrawLine(rearCenter - transform.right * (RearTrack / 2) - transform.forward * (WheelBase / 3), rearCenter + transform.right * (RearTrack / 2) - transform.forward * (WheelBase / 3f));
        }
    }
}