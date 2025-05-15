using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.InatesiArch.UsableObjects
{
    public class UsableObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent _Event;

        public UnityEvent Event { get => _Event; set => _Event = value; }
    }
}