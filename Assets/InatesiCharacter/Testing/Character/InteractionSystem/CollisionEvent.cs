using InatesiCharacter.Testing.LeoEcs;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class CollisionEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _OnCollisionEnter;
        [SerializeField] private UnityEvent _OnCollisionExit;
        [SerializeField] private UnityEvent _OnCollisionStay;
        [SerializeField] private UnityEvent _OnTriggerEnter;
        [SerializeField] private UnityEvent _OnTriggerExit;
        [SerializeField] private UnityEvent _OnTriggerStay;
        [SerializeField] private UnityEvent _OnAnyEvent;
        [SerializeField] private UnityEvent _OnDamage;
        [SerializeField] private UnityEvent _OnUse;

        protected SetupLeoEcs _SetupLeoEcs;



        [Inject]
        protected virtual void Construct(SetupLeoEcs setupLeoEcs)
        {
            _SetupLeoEcs = setupLeoEcs;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _OnCollisionEnter?.Invoke();
            _OnAnyEvent?.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            _OnCollisionExit?.Invoke();
            _OnAnyEvent?.Invoke();
        }

        private void OnCollisionStay(Collision collision)
        {
            _OnCollisionStay?.Invoke();
            _OnAnyEvent?.Invoke();
        }


        private void OnTriggerEnter(Collider other)
        {
            _OnTriggerEnter?.Invoke();
            _OnAnyEvent?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            _OnTriggerExit?.Invoke();
            _OnAnyEvent?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            _OnTriggerStay?.Invoke();
            _OnAnyEvent?.Invoke();
        }


        public void Damage()
        {
            _OnDamage?.Invoke();
        }

        public void Use()
        {
            _OnUse?.Invoke();
        }
    }
}