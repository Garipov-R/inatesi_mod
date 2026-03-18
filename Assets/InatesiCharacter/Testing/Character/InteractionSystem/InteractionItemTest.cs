using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class InteractionItemTest : MonoBehaviour
    {
        [SerializeField] private string _Name;

        public string Name { get => _Name; set => _Name = value; }
    }
}