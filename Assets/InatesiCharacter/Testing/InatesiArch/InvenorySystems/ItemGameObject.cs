using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public class ItemGameObject : MonoBehaviour
    {
        [SerializeField] private ItemScriptableObject _itemScriptableObject;

        private InventoryItem _item;

        public ItemScriptableObject ItemScriptableObject { get => _itemScriptableObject; set => _itemScriptableObject = value; }


        public void SetItem(InventoryItem inventoryItem)
        {
            _item = inventoryItem;
        }

        public void Take()
        {
            
        }
    }
}