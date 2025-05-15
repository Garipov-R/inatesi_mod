using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public class Inventory : IInventory
    {
        private List<Carriable> _List = new List<Carriable>();
        private int _activeSlotIndex = -1;
        private CharacterBase _characterBase;
        private WeaponPoint _weaponPoint;

        public List<Carriable> List { get => _List; set => _List = value; }
        public CharacterBase CharacterBase { get => _characterBase; set => _characterBase = value; }
        public WeaponPoint WeaponPoint { get => _weaponPoint; set => _weaponPoint = value; }

        public Inventory(CharacterBase characterBase)
        {
            _characterBase = characterBase;
            _List = new List<Carriable>();
            _activeSlotIndex = -1;
        }

        //public bool Add(GameObject ent, bool makeactive = false)
        //{

        //    _List.Add(ent);

        //    return true;
        //}

        public bool Add(Carriable carriable, bool makeactive = false)
        {
            _List.Add(carriable);

            carriable.Init(this);

            if (makeactive == true)
            {
                _activeSlotIndex = _List.Count - 1;
                //carriable.Enable();
            }
            
            return true;
        }

        public bool Contains(GameObject ent)
        {
            throw new System.NotImplementedException();
        }

        public int Count()
        {
            return _List.Count;
        }

        public void DeleteContents()
        {
            throw new System.NotImplementedException();
        }

        public bool Drop(GameObject ent)
        {
            throw new System.NotImplementedException();
        }

        public GameObject DropActive()
        {
            throw new System.NotImplementedException();
        }

        public int GetActiveSlot()
        {
            if (_activeSlotIndex == -1) { return 0; }

            return _activeSlotIndex;
        }

        public Carriable GetSlot(int i)
        {
            return _List[i];
        }

        public void OnChildAdded(GameObject child)
        {
            throw new System.NotImplementedException();
        }

        public void OnChildRemoved(GameObject child)
        {
            throw new System.NotImplementedException();
        }

        public bool SetActive(GameObject ent)
        {
            throw new System.NotImplementedException();
        }

        public bool SetActiveSlot(int i, bool allowempty)
        {
            if (_List.Count == 0) { return false; }

            _activeSlotIndex = i;
            _List[i].Enable();

            return false;
        }

        public bool SwitchActiveSlot(int idelta, bool loop)
        {
            throw new System.NotImplementedException();
        }
    }
}