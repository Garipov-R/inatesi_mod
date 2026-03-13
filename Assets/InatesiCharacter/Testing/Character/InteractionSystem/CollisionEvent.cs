using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs5;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static UnityEditor.VersionControl.Asset;

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
        [SerializeField] private bool _IsOneObjectEntered = false;

        protected SetupLeoEcs _SetupLeoEcs;
        private bool _Entered;
        private GameObject _firstObjectEntered;
        private List<GameObject> _Objects = new List<GameObject>();




        [Inject]
        protected virtual void Construct(StartEcs startEcs)
        {
            Debug.Log(startEcs.gameObject.name);
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
            _Objects.Add(other.gameObject);
            ClearEmptyObjects();

            if (_IsOneObjectEntered == true && _Entered == true)
            {
                return;
            }

            _Entered = true;

            _OnTriggerEnter?.Invoke();
            _OnAnyEvent?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            _Objects.Remove(other.gameObject); 
            ClearEmptyObjects();

            if (_IsOneObjectEntered == true && _Objects.Count > 0)
            {
                foreach (var item in _Objects)
                {
                    if (item != null)
                    {
                        return;
                    }
                }
            }

            _Entered = false;

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


        private void ClearEmptyObjects()
        {
            int index = 0;
            foreach (var item in _Objects)
            {
                if (item == null)
                {
                    _Objects.RemoveAt(index);

                }

                index++;
            }
        }
    }
}