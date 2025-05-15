using InatesiCharacter.SuperCharacter;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public abstract class CarriableBase : MonoBehaviour
    {
        public virtual CharacterMotionBase CharacterMotion { get; set; }
    }
}