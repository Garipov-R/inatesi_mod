using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.ui
{
    public class UIObserver : MonoBehaviour
    {
        [SerializeField] private GameObject _VirtualInputGameObject;

        private void Awake()
        {

#if UNITY_STANDALONE || UNITY_EDITOR
            if (_VirtualInputGameObject != null) {_VirtualInputGameObject.SetActive(false);}

#elif UNITY_ANDROID
            if (_VirtualInputGameObject != null) { _VirtualInputGameObject.SetActive(true); }
#endif

        }
    }
}