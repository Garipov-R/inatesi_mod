using InatesiCharacter.Testing.Character.InteractionSystem;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    [CreateAssetMenu(fileName = "item name", menuName = "Inventory items/Basic", order = 1)]
    public class ItemScriptableObject : ScriptableObject
    {
        [SerializeField] private InventoryItem inventoryItem;
        [Space(10)]

        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        private GameObject _prefab;
        [SerializeField] private GameObject _modelPrefab;
        protected TypeItem _itemType;

        public string Name { get => _name; set => _name = value; }
        public GameObject Prefab { get => _prefab; set => _prefab = value; }
        public TypeItem ItemType { get => _itemType; set => _itemType = value; }
        public Sprite Sprite { get => _sprite; set => _sprite = value; }
        public InventoryItem InventoryItem { get => inventoryItem; set => inventoryItem = value; }
        public GameObject ModelPrefab { get => _modelPrefab; set => _modelPrefab = value; }
    }
}