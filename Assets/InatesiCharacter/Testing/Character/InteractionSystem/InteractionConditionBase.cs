using GameToolkit.Localization;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class InteractionConditionBase : MonoBehaviour
    {
        [SerializeField] private bool _success = false;

        public virtual bool Check()
        {
            return _success;
        }
    }
}