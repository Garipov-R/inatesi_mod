using InatesiCharacter.SuperCharacter;
using Unity.VisualScripting;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class TeleportPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _TeleportPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (_TeleportPoint == null)
                return;

            if (other.gameObject.TryGetComponent(out CharacterMotionBase component))
            {
                component.Velocity = Vector3.zero;
                component.SetPositionAndRotation(_TeleportPoint.position, _TeleportPoint.rotation);
                //component.LookSource.CameraMotion.
            }
        }
    }
}

