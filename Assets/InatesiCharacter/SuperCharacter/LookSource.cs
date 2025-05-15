using InatesiCharacter.Camera;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter
{
    public class LookSource : MonoBehaviour, ILookSource
    {
        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        public float LookDirectionDistance { get; private set; }

        public float Pitch { get; private set; }

        public CameraMotion CameraMotion { get; set; }


        private void Awake()
        {
            CameraMotion = GetComponent<CameraMotion>();
        }

        public Vector3 LookDirection(bool characterLookDirection = false)
        {
            return transform.forward;
        }

        public Vector3 LookDirection(Vector3 lookPosition, bool characterLookDirection, int layerMask, bool includeRecoil, bool includeMovementSpread)
        {
            return Vector3.zero;
        }

        public Vector3 LookPosition()
        {
            return transform.position;
        }
    }
}