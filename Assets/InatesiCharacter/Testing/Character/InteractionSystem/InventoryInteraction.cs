using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.FPS_Utility;
using InatesiCharacter.Testing.Character.Weapons;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using SwayBob = InatesiCharacter.Testing.InatesiArch.WeaponsTest.SwayBob;


namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class InventoryInteraction
    {
        private InventoryContainer _inventoryContainer;
        private CharacterMotionBase _characterMotionBase;
        private FirstPersonCameraHelper _FirstPersonHelper;
        private IWeapon _CurrentIWeaponBase;
        private WeaponBase _CurrentWeaponBase;
        private Dictionary<WeaponBase, GameObject> _WeaponsDictionary = new();

        public InventoryContainer InventoryContainer { get => _inventoryContainer; set => _inventoryContainer = value; }
        public IWeapon IWeaponBase { get => _CurrentIWeaponBase; set => _CurrentIWeaponBase = value; }
        public WeaponBase WeaponBase { get => _CurrentWeaponBase; set => _CurrentWeaponBase = value; }

        public InventoryInteraction(InventoryContainer inventoryContainer, CharacterMotionBase characterMotionBase)
        {
            _inventoryContainer = inventoryContainer;
            _characterMotionBase = characterMotionBase;
            _inventoryContainer.ActiveSlotIndex = 0;

            if (characterMotionBase.LookSource != null)
                if (characterMotionBase.LookSource.GameObject.TryGetComponent(out FirstPersonCameraHelper fps)) 
                    _FirstPersonHelper = fps;
        }

        public void SetActiveInventoryitem(int id)
        {
            var item = _inventoryContainer.GetSlot(id);
            var currentItem = _inventoryContainer.ActiveItem;

            InventoryContainer.ActiveSlotIndex = id;

            if (item == currentItem)
                return;

            InitializeItemType(null);

            item = _inventoryContainer.GetSlot(InventoryContainer.ActiveSlotIndex);

            if (item == null)
                return;

            InitializeItemType(item);
        }

        public void DropInventoryItem()
        {
            try
            {
                if (InventoryContainer.TryGetActiveItem(out var item) == true)
                {
                    //CharacterVars.CharacterWorldInteractionSystem.DestroyRightHandObject();

                    //UnityEngine.Object.Destroy(_currentInventoryItem);
                    /* var g = UnityEngine.Object.Instantiate(item.ItemScriptableObject.Prefab);
                     g.transform.SetPositionAndRotation(
                         CharacterMotion.transform.position + CharacterMotion.transform.forward * 1f + CharacterMotion.transform.up * 2f,
                         Quaternion.identity
                     );

                     if (g.TryGetComponent(out Rigidbody rb))
                     {
                         rb.AddForce(CharacterMotion.transform.forward * 5f + CharacterMotion.transform.up * 10f, ForceMode.VelocityChange);
                     }

                     switch (item.TypeItem)
                     {
                         case TypeItem.Weapon:
                             var attackAbility = CharacterBase.GetAbility<Attack>() as Attack;
                             attackAbility.Drop();
                             break;
                     }*/


                    if (item.ItemScriptableObject.ModelPrefab != null)
                    {
                        var modelPrefab = UnityEngine.Object.Instantiate(item.ItemScriptableObject.ModelPrefab);

                        if (modelPrefab != null)
                        {
                            modelPrefab.transform.SetPositionAndRotation(
                                _characterMotionBase.transform.position + _characterMotionBase.transform.forward * 1f + _characterMotionBase.transform.up * 1f,
                                Quaternion.identity
                            );

                            if (modelPrefab.TryGetComponent(out CarriableObject carriableObject))
                            {
                                carriableObject.CarriableObjectData = item.CarriableObjectData;
                            }

                            if (modelPrefab.TryGetComponent(out Rigidbody rb))
                            {
                                rb.AddForce(_characterMotionBase.transform.forward * 2f + _characterMotionBase.transform.up * 5f, ForceMode.VelocityChange);
                            }
                        }
                    }


                    InventoryContainer.Remove(item);

                    if (_CurrentIWeaponBase != null)
                    {
                        _CurrentIWeaponBase.Disable();
                        _CurrentIWeaponBase.Drop();
                        _CurrentIWeaponBase = null;
                    }

                    if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(null);
                }
            }
            catch (Exception e)
            {
                //Debug.LogError(e.ToString());
            }

            /*if (InventoryContainer.GetRandomItem(out InventorySystems.InventoryItem inventoryItem) == true)
            {
                SetActiveInventoryitem(inventoryItem.SlotIndex);
            }*/
        }

        public void DropAllInventroyItem()
        {
            if (InventoryContainer == null)
                return;

            if (_inventoryContainer.InventoryItems == null) return;

            foreach (var item in _inventoryContainer.InventoryItems.ToArray())
            {
                if (item == null) continue;

                if (item.ItemScriptableObject.ModelPrefab != null)
                {
                    var modelPrefab = UnityEngine.Object.Instantiate(item.ItemScriptableObject.ModelPrefab);

                    if (modelPrefab != null)
                    {
                        modelPrefab.transform.SetPositionAndRotation(
                            _characterMotionBase.transform.position + _characterMotionBase.transform.forward * 1f + _characterMotionBase.transform.up * 1f,
                            Quaternion.identity
                        );

                        if (modelPrefab.TryGetComponent(out Rigidbody rb))
                        {
                            rb.AddForce(_characterMotionBase.transform.forward * 2f + _characterMotionBase.transform.up * 5f, ForceMode.VelocityChange);
                        }
                    }
                }


                InventoryContainer.Remove(item);

                if (_CurrentIWeaponBase != null)
                {
                    _CurrentIWeaponBase.Disable();
                    _CurrentIWeaponBase.Drop();
                    _CurrentIWeaponBase = null;
                }

                if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(null);
            }
        }

        public void InitializeItemType(InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
            {
                if (_CurrentIWeaponBase != null)
                {
                    _CurrentIWeaponBase.Disable();
                    _CurrentIWeaponBase = null;
                }

                if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(null);

                return;
            }

            

            switch (inventoryItem.TypeItem)
            {
                case TypeItem.Weapon:

                    if (_CurrentIWeaponBase != null)
                    {
                        _CurrentIWeaponBase.Disable();
                        _CurrentIWeaponBase = null;
                    }



                    bool hasItem = false;
                    WeaponItemScriptableObject weaponSO = inventoryItem.ItemScriptableObject as WeaponItemScriptableObject;
                    GameObject viewModel = null;
                    WeaponBase weapon = null;

                    foreach (var item in _WeaponsDictionary)
                    {
                        if (item.Key.name.Replace("(Clone)", "") == weaponSO.Weapon.name)
                        {
                            hasItem = true;
                            weapon = item.Key;
                            viewModel = item.Value;
                            Debug.Log($"{hasItem} {weapon.name}");
                            break;
                        }
                    }

                    if (weapon != null)
                    {
                        _CurrentWeaponBase = weapon;
                        _CurrentIWeaponBase = weapon;
                        _CurrentWeaponBase.SwayBob.Init(viewModel);
                        _CurrentIWeaponBase.Enable();
                    }

                break;
            }
        }

        public void UpdateTick()
        {
            if (_CurrentIWeaponBase != null)
            {
                _CurrentIWeaponBase.UpdateTick();
            }
        }

        public void UpdateWeaponEffect()
        {
            if (_CurrentIWeaponBase != null)
            {
                _CurrentIWeaponBase.UpdateEffect();
            }
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







        public void SelectWeapon(int id)
        {
            var item = _inventoryContainer.GetSlot(id);
            var currentItem = _inventoryContainer.ActiveItem;

            InventoryContainer.ActiveSlotIndex = id;

            if (item == currentItem)
                return;

            InitializeItemType(null);

            item = _inventoryContainer.GetSlot(InventoryContainer.ActiveSlotIndex);

            if (item == null)
                return;

            InitializeItemType(item);
        }

        public void InitializeWeapons()
        {
            if (_inventoryContainer.InventoryItems == null) return;
            if (_inventoryContainer.InventoryItems.Count == 0) return;  

            foreach (var item in _inventoryContainer.InventoryItems)
            {
                if (item == null) continue; 

                WeaponItemScriptableObject weaponSO = item.ItemScriptableObject as WeaponItemScriptableObject;
                GameObject viewModel = null;
                WeaponBase weapon = null;

                if (weaponSO.ViewModel != null)
                {
                    viewModel = GameObject.Instantiate(weaponSO.ViewModel);
                }

                if (weaponSO.Weapon != null)
                {
                    weapon = GameObject.Instantiate(weaponSO.Weapon, _characterMotionBase.transform);
                    weapon.gameObject.layer = LayerMask.NameToLayer("Player");
                }
                else
                {
                    weapon = new GameObject("DefaultWeapon").AddComponent<DefaultWeapon>();
                }

                _WeaponsDictionary.Add(weapon, viewModel);

                if (_FirstPersonHelper != null) _FirstPersonHelper.SetRightHandObject(viewModel);
                _CurrentIWeaponBase = weapon;
                _CurrentWeaponBase = weapon; 
                _CurrentWeaponBase.SwayBob = weaponSO.SwayBobItemSO.SwayBob ?? new SwayBob();
                _CurrentWeaponBase.SpawnedViewModel = viewModel;
                _CurrentWeaponBase.CharacterMotion = _characterMotionBase;
                _CurrentWeaponBase.CameraMotion = _characterMotionBase.LookSource.GameObject.GetComponent<CameraMotion>();

                if (viewModel != null)
                {
                    _CurrentWeaponBase.SwayBob.Init(viewModel);
                }


                _CurrentIWeaponBase.Disable();
            }
        }
    }
}