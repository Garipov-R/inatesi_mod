using GameToolkit.Localization;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class CollisionEvent : MonoBehaviour
    {
        [SerializeField] private InteractionConditionBase _InteractionConditionBase;
        [SerializeField] private UnityEvent _OnCollisionEnter;
        [SerializeField] private UnityEvent _OnCollisionExit;
        [SerializeField] private UnityEvent _OnCollisionStay;
        [SerializeField] private UnityEvent _OnTriggerEnter;
        [SerializeField] private UnityEvent _OnTriggerExit;
        [SerializeField] private UnityEvent _OnTriggerStay;
        [SerializeField] private UnityEvent _OnAnyEvent;
        [SerializeField] private UnityEvent _OnDamage;
        [SerializeField] private UnityEvent _OnUse;
        [SerializeField] private UnityEvent _OnSuccess;
        [SerializeField] private UnityEvent _OnError;
        [SerializeField] private bool _IsOneObjectEntered = false;
        [SerializeField] private LayerMask _EnterLayer;

        protected SetupLeoEcs _SetupLeoEcs;
        private bool _Entered;
        private GameObject _firstObjectEntered;
        private List<GameObject> _Objects = new List<GameObject>();
        [Inject] private StartEcs _StartEcs;



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


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (IsIncludeLayer(other) == false) return;
            if (CheckInteractionCondition() == false) 
            { 
                return; 
            }
            else
            {

            }

            _Objects.Add(other.gameObject);
            ClearEmptyObjects();

            if (_IsOneObjectEntered == true && _Entered == true)
            {
                return;
            }

            _Entered = true;

            _OnTriggerEnter?.Invoke();
            _OnAnyEvent?.Invoke();

            if (_StartEcs)
            {
                ref var collisionEvent = ref ECSHelper.Create<InatesiCharacter.Testing.LeoEcs5.Components.CollisionComponentEvent>(_StartEcs.EcsWorld);
                collisionEvent.collideGameObject = other.gameObject;
                collisionEvent.gameObject = gameObject;
            }
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

        private bool IsIncludeLayer(Collider other)
        {
            if ((_EnterLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckInteractionCondition()
        {
            if (_InteractionConditionBase == null)
            {
                _OnSuccess?.Invoke();
                return true;
            }

            if (_InteractionConditionBase != null)
            {
                if (_InteractionConditionBase.Check() == false)
                {
                    _OnError?.Invoke();
                    return false;
                }
                else
                {
                    _OnSuccess?.Invoke();
                    return true;
                }
            }
            
            

            return true;
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