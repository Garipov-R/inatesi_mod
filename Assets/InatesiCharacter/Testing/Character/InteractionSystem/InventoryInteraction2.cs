using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.FPS_Utility;
using InatesiCharacter.Testing.Character.Weapons;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class InventoryInteraction2 
    {
        private InventoryContainer _InventoryContainer;
        private WeaponBase _CurrentWeaponBase; 
        private FirstPersonCameraHelper _FirstPersonHelper;
        private Dictionary<WeaponBase, GameObject> _WeaponsDictionary = new();
        private Dictionary<int, WeaponBase> _WeaponsDictionary2 = new();
        private CharacterMotionBase _characterMotionBase;

        public InventoryContainer InventoryContainer { get => _InventoryContainer; set => _InventoryContainer = value; }
        public CharacterMotionBase CharacterMotionBase { get => _characterMotionBase; set => _characterMotionBase = value; }
        public WeaponBase CurrentWeaponBase { get => _CurrentWeaponBase; set => _CurrentWeaponBase = value; }


        public InventoryInteraction2(CharacterMotionBase characterMotionBase)
        {
            _InventoryContainer = new InventoryContainer();
            _characterMotionBase = characterMotionBase;

            if (characterMotionBase.LookSource != null)
                if (characterMotionBase.LookSource.GameObject.TryGetComponent(out FirstPersonCameraHelper fps))
                    _FirstPersonHelper = fps;
        }

        public void SetActiveInventoryItem(int id)
        {
            var item = _InventoryContainer.GetSlot(id);
            var currentItem = _InventoryContainer.ActiveItem;

            _InventoryContainer.ActiveSlotIndex = id;

            if (item == currentItem)
                return;

            InitializeSelectedItemType(null);

            item = _InventoryContainer.GetSlot(_InventoryContainer.ActiveSlotIndex);

            if (item == null)
                return;

            InitializeSelectedItemType(item);
        }

        public void InitializeWeapons()
        {
            if (_InventoryContainer.InventoryItems == null) return;
            if (_InventoryContainer.InventoryItems.Count == 0) return;

            foreach (var item in _InventoryContainer.InventoryItems)
            {
                InitializeItem(item);
            }
        }

        public void InitializeItem(InventoryItem item)
        {
            if (item == null) return;

            WeaponItemScriptableObject weaponSO = item.ItemScriptableObject as WeaponItemScriptableObject;
            GameObject viewModel = null;
            WeaponBase weaponBase = null;

            if (weaponSO.ViewModel != null && _FirstPersonHelper != null)
            {
                viewModel = GameObject.Instantiate(weaponSO.ViewModel);
            }

            if (weaponSO.Weapon != null)
            {
                weaponBase = GameObject.Instantiate(weaponSO.Weapon, _characterMotionBase.transform);
                //weapon.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            else
            {
                weaponBase = new GameObject("DefaultWeapon").AddComponent<DefaultWeapon>();
            }

            if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(viewModel);
            weaponBase.SwayBob = weaponSO.SwayBobItemSO.SwayBob ?? new InatesiArch.WeaponsTest.SwayBob();
            weaponBase.SpawnedViewModel = viewModel;
            weaponBase.CharacterMotion = _characterMotionBase;
            weaponBase.CameraMotion = _characterMotionBase.LookSource.GameObject.GetComponent<CameraMotion>();
            weaponBase.CarriableObjectData = item.CarriableObjectData;

            if (viewModel != null)
            {
                weaponBase.SwayBob.Init(viewModel);
            }

            weaponBase.Disable();


            _WeaponsDictionary.Add(weaponBase, viewModel);
            _WeaponsDictionary2.Add(item.SlotIndex, weaponBase);
        }

        private void InitializeSelectedItemType(InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
            {
                if (_CurrentWeaponBase != null)
                {
                    _CurrentWeaponBase.Disable();
                    _CurrentWeaponBase = null;
                }

                if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(null);

                return;
            }


            switch (inventoryItem.TypeItem)
            {
                case TypeItem.Weapon:

                    if (_CurrentWeaponBase != null)
                    {
                        _CurrentWeaponBase.Disable();
                        _CurrentWeaponBase = null;
                    }



                    bool hasItem = false;
                    WeaponItemScriptableObject weaponSO = inventoryItem.ItemScriptableObject as WeaponItemScriptableObject;
                    GameObject viewModel = null;
                    WeaponBase weaponBase = null;

                    /*foreach (var item in _WeaponsDictionary)
                    {
                        if (
                            item.Key.name.Replace("(Clone)", "") == weaponSO.Weapon.name 
                            && item.Key.CarriableObjectData.WeaponType == weaponSO.CarriableObjectData.WeaponType
                        )
                        {
                            hasItem = true;
                            weaponBase = item.Key;
                            viewModel = item.Value;
                            //Debug.Log($"{hasItem} {weapon.name}");
                            break;
                        }
                    }*/

                    foreach (var item in _WeaponsDictionary2)
                    {
                        if (item.Key == inventoryItem.SlotIndex)
                        {
                            hasItem = true;
                            weaponBase = item.Value;
                            //Debug.Log($"{hasItem} {weapon.name}");
                            break;
                        }
                    }

                    if (weaponBase != null)
                    {
                        _CurrentWeaponBase = weaponBase;
                        _CurrentWeaponBase.SwayBob.Init(weaponBase.SpawnedViewModel);
                        _CurrentWeaponBase.Enable();
                    }

                    break;
            }
        }

        public InventoryItem AddItem(ItemScriptableObject itemScriptableObject)
        {
            var item = CreateInventoryItem(itemScriptableObject);
            _InventoryContainer.Add(item);
            return item;
        }

        public bool AddItem(CarriableObject carriableObject)
        {
            var item = CreateInventoryItem(carriableObject);
            var canAdd = _InventoryContainer.Add(item);
            InitializeItem(item);
            return canAdd;
        }

        public InventoryItem CreateInventoryItem(CarriableObject carriableObject)
        {
            var inventoryItem = CreateInventoryItem(carriableObject.ItemScriptableObject);
            inventoryItem.CarriableObjectData = carriableObject.CarriableObjectData;
            return inventoryItem;
        }

        public InventoryItem CreateInventoryItem(ItemScriptableObject itemScriptableObject)
        {
            if (itemScriptableObject == null) return null;

            var newItem = new InventoryItem();
            newItem.Setup(itemScriptableObject);

            return newItem;
        }

        public void UpdateEffectTick()
        {
            if (_CurrentWeaponBase != null)
            {
                _CurrentWeaponBase.UpdateEffect();
            }
        }

        public void UpdateTick()
        {
            if (_CurrentWeaponBase != null)
            {
                _CurrentWeaponBase.UpdateTick();
            }
        }

        public void Drop()
        {
            try
            {
                if (_InventoryContainer.TryGetActiveItem(out var item) == true)
                {
                    if (item.ItemScriptableObject.ModelPrefab != null)
                    {
                        var modelPrefab = UnityEngine.Object.Instantiate(item.ItemScriptableObject.ModelPrefab);

                        modelPrefab.transform.SetPositionAndRotation(
                            _characterMotionBase.transform.position + (_characterMotionBase.transform.forward * (_characterMotionBase.Radius + 1f)) + _characterMotionBase.Up * (_characterMotionBase.Height - _characterMotionBase.Radius),
                            Quaternion.identity
                        );

                        if (modelPrefab.TryGetComponent(out CarriableObject carriableObject))
                        {
                            carriableObject.CarriableObjectData = _CurrentWeaponBase.CarriableObjectData;
                        }

                        if (modelPrefab.TryGetComponent(out Rigidbody rb))
                        {
                            rb.AddForce(_characterMotionBase.transform.forward * 1f + _characterMotionBase.Up * 2f, ForceMode.VelocityChange);
                            rb.AddTorque(_characterMotionBase.transform.forward * 5f + _characterMotionBase.transform.up * 5f, ForceMode.VelocityChange);
                        }
                    }


                    _InventoryContainer.Remove(item);
                    _WeaponsDictionary.Remove(_CurrentWeaponBase);
                    _WeaponsDictionary2.Remove(item.SlotIndex);

                    if (_CurrentWeaponBase != null)
                    {
                        _CurrentWeaponBase.Disable();
                        _CurrentWeaponBase.Drop();
                        _CurrentWeaponBase = null;
                    }

                    if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(null);
                }
            }
            catch (Exception e)
            {
                //Debug.LogError(e.ToString());
            }
        }
    }
}