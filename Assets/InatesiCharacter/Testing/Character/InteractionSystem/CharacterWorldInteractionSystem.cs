using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class CharacterWorldInteractionSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _RightHand;
        [SerializeField] private Rig _PistolRig;
        [SerializeField] private Rig _RifleRig;
        [SerializeField] private Rig _RootRig;

        private bool _aim;

        public GameObject RightHand { get => _RightHand; set => _RightHand = value; }
        public GameObject RightHandCurrentChild { get => _rightHandCurrentChild; set => _rightHandCurrentChild = value; }
        public bool Aim { get => _aim; set => _aim = value; }

        private GameObject _rightHandCurrentChild;


        public bool SetRightHandObject(GameObject gameObj) 
        {
            if (_RightHand == null) 
            {
                gameObj.transform.SetParent(transform);
                gameObj.SetActive(false); 
                _rightHandCurrentChild = gameObj;
                return false; 
            }

            _rightHandCurrentChild = gameObj;

            gameObj.transform.SetParent(_RightHand.transform);

            gameObj.transform.localEulerAngles = Vector3.zero;
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localScale = Vector3.one;
            gameObj.layer = LayerMask.NameToLayer("Player");

            foreach (var child in gameObj.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }

            return true;
        }

        public bool DestroyRightHandObject() 
        {
            if (_rightHandCurrentChild == null) return false;

            Destroy(_rightHandCurrentChild);

            return true;
        }

        public void ActiveRig(bool active, bool pistol)
        {
            _aim = active;

            if (_PistolRig != null && pistol)
            {
                _PistolRig.weight = active ? 1 : 0;
            }

            if (_RifleRig != null && !pistol)
            {
                _RifleRig.weight = active ? 1 : 0;
            }

           ActiveRootRig(!active);
        }

        public void ActiveRootRig(bool active)
        {
            if (_RootRig == null) return;

            _RootRig.weight = active ? 1 : 0;
        }
    }
}