using System.Collections;
using UnityEngine;
 
namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public class WeaponPoint : MonoBehaviour
    {
        [SerializeField] private GameObject[] _List;

        public GameObject[] List { get => _List; set => _List = value; }
    }
}