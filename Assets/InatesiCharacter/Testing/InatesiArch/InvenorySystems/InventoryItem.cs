using InatesiCharacter.Testing.Character.InteractionSystem;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private string _name;
        [SerializeField] private TypeItem _typeItem;
        [SerializeField] private ItemScriptableObject _itemScriptableObject;
        [SerializeField] private int _slotIndex = 0;

        private CarriableObjectData _carriableObjectData = new(); 

        public string Name { get => _name; set => _name = value; }
        public TypeItem TypeItem { get => _typeItem; set => _typeItem = value; }
        public ItemScriptableObject ItemScriptableObject { get => _itemScriptableObject; set => _itemScriptableObject = value; }
        public int SlotIndex { get => _slotIndex; set => _slotIndex = value; }
        public CarriableObjectData CarriableObjectData { get => _carriableObjectData; set => _carriableObjectData = value; }

        public void Setup(ItemScriptableObject itemScriptableObject)
        {
            Name = itemScriptableObject.Name;
            _typeItem = itemScriptableObject.ItemType;
            _itemScriptableObject = itemScriptableObject;

            if (itemScriptableObject is WeaponItemScriptableObject)
                _carriableObjectData = new CarriableObjectData( (itemScriptableObject as WeaponItemScriptableObject).CarriableObjectData);
        }
    }

    public enum TypeItem
    {
        Basic = 0,
        Weapon = 1
    }
}