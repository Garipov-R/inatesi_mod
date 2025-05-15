using InatesiCharacter.SuperCharacter;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public class Carriable
    {
        private Inventory _inventory;

        public Inventory Inventory { get => _inventory; set => _inventory = value; }
        public CharacterMotionBase CharacterMotion { get => _inventory.CharacterBase.CharacterMotion; }
        public CharacterBase CharacterBase { get => _inventory.CharacterBase; }
        public CharacterVars CharacterVars { get => _inventory.CharacterBase.CharacterVars; }
        public Attacks.WeaponPoint WeaponPoint { get => _inventory.WeaponPoint; }


        public void Init(Inventory inventory)
        {
            _inventory = inventory;
        }

        public virtual void PrimaryAttack()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Enable()
        {

        }

        public virtual void Disable()
        {

        }
    }
}