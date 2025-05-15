using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class Attack : AbilityBase
    {
        TimeSince timeSinceAttack;
        Attacks.Carriable currentCarriable;
        private WeaponsTest.IWeapon _currentWeapon;

        internal IWeapon CurrentWeapon { get => _currentWeapon; set => _currentWeapon = value; }

        public override void Start() 
        {
            /* var weaponPoint = _CharacterMotion.transform.GetComponentInChildren<Attacks.WeaponPoint>();
            if (weaponPoint != null)
            {
                CharacterBase.Inventory.WeaponPoint = weaponPoint;
            }


            CharacterBase.Inventory.SetActiveSlot(0, false);*/
        }

        
        public override void Update()
        {
            if (CharacterVars.RiggingTest.Target != null)
            {
                CharacterVars.RiggingTest.Target.position = 
                     CharacterMotion.LookSource.LookPosition() + CharacterMotion.LookSource.LookDirection() * 30f;
            }

            if (CharacterBase.InventoryContainer.TryGetActiveItem(out InventorySystems.InventoryItem inventoryItem))
            {
                if (inventoryItem.TypeItem == InventorySystems.TypeItem.Weapon)
                {
                    if (_currentWeapon != null)
                    {
                        _currentWeapon.UpdateTick();
                    }
                }
            }
        }
         
        public void SetWeapon(WeaponsTest.WeaponBase weapon)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Disable();
            }

            _currentWeapon = weapon;
            _currentWeapon.CharacterMotion = _CharacterMotion;
            _currentWeapon.Init();
            _currentWeapon.Enable();
        }

        public void SetEmpty()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Disable();
                _currentWeapon = null;
            }
        }

        public void Drop()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Drop();
            }

            SetEmpty();
        }

        private void Rigging(bool state = true)
        {
            CharacterVars.RiggingTest.Rig.weight = state == true ? 1 : 0;
            CharacterVars.RiggingTest.SpineRig.transform.rotation =
                Quaternion.LookRotation(
                    CharacterMotion.LookSource.Transform.forward//,
                                                                //CharacterMotion.Up
                );

            CharacterVars.RiggingTest.RHandRig.transform.rotation =
                    Quaternion.LookRotation(
                        CharacterMotion.LookSource.Transform.right
                    //characterComponent.characterMotionTest.Up
                    );

            CharacterVars.RiggingTest.LHandRig.transform.rotation =
                Quaternion.LookRotation(
                    CharacterMotion.LookSource.Transform.right
                //characterComponent.characterMotionTest.Up
                );
        }
    }
}
