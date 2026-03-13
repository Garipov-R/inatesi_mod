using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.InteractionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct CharacterComponent
    {
        public GameObject gameObject;
        public Transform transform;
        public CharacterMotionBase characterMotion;
        public InventoryInteraction2 InventoryInteraction2;
        public CharacterSO CharacterSO;
        public bool isDead;
    }
}
