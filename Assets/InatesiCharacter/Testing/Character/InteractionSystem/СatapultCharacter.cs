using InatesiCharacter.SuperCharacter;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class СatapultCharacter : MonoBehaviour
    {
        [SerializeField] private Vector3 _Force = new Vector3(0, 27, 5);

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CharacterMotionBase motionBase))
            {
                motionBase.InputDirection = Vector3.zero;
                motionBase.InputVector = Vector3.zero;
                motionBase.Velocity = Vector3.zero;
                motionBase.IsInputDisabled = false;
                motionBase.AddForce(transform.up * _Force.y + transform.forward * _Force.z);
            }
        }
    }
}