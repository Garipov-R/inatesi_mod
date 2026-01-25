using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    //[RequireComponent(typeof( BoxCollider))]
    public class TriggerTouch : MonoBehaviour
    {
        [SerializeField] private UnityEvent _OnTouch;

        public UnityEvent OnTouch { get => _OnTouch; set => _OnTouch = value; }


        private void Start()
        {
            //UnityEngine.Pool.
        }
    }

    public class TriggerTouchSetting
    {
        [SerializeField] private float _Delay;
        [SerializeField] private UnityEvent _OnTouch;
    }
}