using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.InteractionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.LeoEcs4.Components
{
    public struct CharacterComponent
    {
        public CharacterMotionBase CharacterMotionBase;
        public GameObject GameObject;
        public InventoryInteraction InventoryInteraction;
        public InventoryInteraction2 InventoryInteraction2;
        public CharacterWorldInteractionSystem CharacterWorldInteractionSystem;
        public CharacterSO CharacterSO;
        public ICharacter ICharacter;

        public bool Dead;
        public float Health;
        public bool Crouched;
        public float TimeOfDeath;
        public Action<float> OnHealthChanged;
        public float stamina;


        // bot
        public NavMeshPath NavMeshPath;
        public NavMeshAgent NavMeshAgent;
        public NavMeshObstacle NavMeshObstacle;
    }
}
