using InatesiCharacter.Testing.Character.UI;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Components
{
    public struct PlayerComponent
    {
        public bool moveInputEnabled;
        public bool cameraInputEnabled;
        public bool fpc;
        public bool noclip;
        public bool canAttack;


        public InventoryUI InventoryUI;
        public HealthUI HealthUI;
    }
}