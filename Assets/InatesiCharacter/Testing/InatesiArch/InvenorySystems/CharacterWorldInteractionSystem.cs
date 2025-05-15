using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public class CharacterWorldInteractionSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _RightHand;

        public GameObject RightHand { get => _RightHand; set => _RightHand = value; }
        public GameObject RightHandCurrentChild { get => _rightHandCurrentChild; set => _rightHandCurrentChild = value; }

        private GameObject _rightHandCurrentChild;


        public bool SetRightHandObject(GameObject gameObj) 
        {
            if (_RightHand == null) return false ;

            _rightHandCurrentChild = gameObj;

            gameObj.transform.SetParent(_RightHand.transform);

            gameObj.transform.localEulerAngles = Vector3.zero;
            gameObj.transform.localPosition = Vector3.zero;

            return true;
        }

        public bool DestroyRightHandObject() 
        {
            if (_rightHandCurrentChild == null) return false;

            Destroy(_rightHandCurrentChild);

            return true;
        }
    }
}