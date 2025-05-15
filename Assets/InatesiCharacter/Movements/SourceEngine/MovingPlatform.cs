using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Movements.SourceEngine
{
    [System.Serializable]
    public class MovingPlatform
    {
        private Transform _activePlatform;
        private Vector3 _moveDirection;
        private Vector3 _activeGlobalPlatformPoint;
        private Vector3 _activeLocalPlatformPoint;
        private Quaternion _activeGlobalPlatformRotation;
        private Quaternion _activeLocalPlatformRotation;
        private Transform _transform;

        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }


        public MovingPlatform(Transform transform)
        {
            _transform = transform;
        }

        public void Update()
        {
            if (_activePlatform != null)
            {
                Vector3 newGlobalPlatformPoint = _activePlatform.TransformPoint(_activeLocalPlatformPoint);
                var oldMoveDir = MoveDirection;
                MoveDirection = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
                if (MoveDirection.magnitude > 0.01f)
                {
                    MoveDirection = MoveDirection;
                }
                if (_activePlatform)
                {
                    // Support moving platform rotation
                    Quaternion newGlobalPlatformRotation = _activePlatform.rotation * _activeLocalPlatformRotation;
                    Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(_activeGlobalPlatformRotation);
                    // Prevent rotation of the local up vector
                    rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                    _transform.rotation = rotationDiff * _transform.rotation;
                    _transform.eulerAngles = new Vector3(0, _transform.eulerAngles.y, 0);

                    UpdateMovingPlatform();
                }
            }
            else
            {
                if (MoveDirection.magnitude > 0.01f)
                {
                    MoveDirection = Vector3.Lerp(MoveDirection, Vector3.zero, Time.deltaTime);
                }
            }
        }

        public void OnGroundColliderHit(ControllerColliderHit hit, Transform platform)
        {
            if (platform == null)
            {
                return;
            }

            if (_activePlatform != platform)
            {
                _activePlatform = platform;
                UpdateMovingPlatform();
            }
        }

        private void UpdateMovingPlatform()
        {
            _activeGlobalPlatformPoint = _transform.position;
            _activeLocalPlatformPoint = _activePlatform.InverseTransformPoint(_transform.position);
            // Support moving platform rotation
            _activeGlobalPlatformRotation = _transform.rotation;
            _activeLocalPlatformRotation = Quaternion.Inverse(_activePlatform.rotation) * _transform.rotation;
        }
    }
}
