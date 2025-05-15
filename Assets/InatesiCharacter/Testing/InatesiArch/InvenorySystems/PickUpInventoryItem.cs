using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public class PickUpInventoryItem : MonoBehaviour
    {
        [SerializeField] private ItemScriptableObject _itemScriptableObject;
        [SerializeField] private bool _activePickUp = false;
        private InventoryItem _inventoryItem;

        public bool ActivePickUp { get => _activePickUp; set => _activePickUp = value; }
        public ItemScriptableObject ItemScriptableObject { get => _itemScriptableObject; set => _itemScriptableObject = value; }
        public InventoryItem InventoryItem { get => _inventoryItem; set => _inventoryItem = value; }


        private void Start()
        {
            if (_itemScriptableObject == null)
                return;

            _inventoryItem = new InventoryItem();
            _inventoryItem.Setup(_itemScriptableObject);
        }

        public InventoryItem Take()
        {
            if (_itemScriptableObject == null )
                return null;

            if (TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }

            if (TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            return _inventoryItem;
        }
    }
}