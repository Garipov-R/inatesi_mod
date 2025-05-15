using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    public class InventoryContainer
    {
        private int _invetorySize = 5;
        private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
        private int _activeSlotIndex = -1;
        private int _Size = 6;
        private InventoryItem _activeItem;

        public List<InventoryItem> InventoryItems { get => _inventoryItems; set => _inventoryItems = value; }
        public UnityEngine.Events.UnityAction<InventoryItem, bool> OnAdded { get; set; }
        public UnityEngine.Events.UnityAction<InventoryItem> OnRemoved { get; set; }
        public UnityEngine.Events.UnityAction<InventoryItem> OnSelected { get; set; }
        public int Size { get => _Size; set => _Size = value; }
        public int ActiveSlotIndex 
        { 
            get => _activeSlotIndex; 
            set 
            { 
                _activeSlotIndex = value;

                if (_activeSlotIndex >= _Size)
                {
                    _activeSlotIndex = 0;
                }
                else if (_activeSlotIndex < 0)
                {
                    _activeSlotIndex = _Size - 1;
                }

                TryGetActiveItem(out InventoryItem inventoryitem);
                OnSelected?.Invoke(inventoryitem); 
            } 
        }
        public InventoryItem ActiveItem { get => _activeItem; set => _activeItem = value; }


        public InventoryContainer()
        {
            Initialize();
        }

        private void Initialize()
        {
            InventoryItems = new List<InventoryItem>();

            for (int i = 0; i < _Size; i++)
            {
                _inventoryItems.Add(null);
            }

            OnAdded += (item, state) =>
            {

            };
        }



        public bool Add(InventoryItem inventoryItem, bool active = false)
        {
            if (inventoryItem == null)
                return false;

            if (FindFreeSlot(out int slot, inventoryItem) == false)
                return false;

            InventoryItems[slot] = inventoryItem;

            inventoryItem.SlotIndex = slot;

            if (active == true) _activeSlotIndex = slot;

            OnAdded?.Invoke(inventoryItem, active);

            return true;
        }

        public void Remove(InventoryItem item)
        {
            InventoryItems[item.SlotIndex] = null;

            //ActiveSlotIndex = -1;

            _activeItem = null;

            OnRemoved?.Invoke(item);
        }

        public void Clear()
        {
            InventoryItems = new();

            _activeItem = null;
        }

        public InventoryItem Remove(ulong itemId)
        {
            /*if (itemId == 0)
            {
                return null;
            }

            for (ushort i = 0; i < InventoryItems.Count; i++)
            {
                var instance = InventoryItems[i];

                if (instance != null && instance.ItemId == itemId)
                {
                    return ClearSlot(i);
                }
            }*/

            return null;
        }

        /*public InventoryItem ClearSlot(ushort slot, bool clearItemContainer = true)
        {
            if (Game.IsClient)
            {
                return null;
            }

            if (!IsOccupied(slot))
            {
                return null;
            }

            var instance = GetFromSlot(slot);

            if (clearItemContainer)
            {
                if (instance.Parent == this)
                {
                    instance.Parent = null;
                    instance.SlotId = 0;
                }
            }

            ItemList[slot] = null;

            SendTakeEvent(slot, instance);

            return instance;
        }*/

        public bool FindFreeSlot(out int slot, InventoryItem instance = null)
        {
            slot = -1;

            if (InventoryItems == null)
                return false;

            if (InventoryItems.Count == 0)
                return false;


            var slotLimit = Size;

            for (ushort i = 0; i < slotLimit; i++)
            {
                if (InventoryItems[i] == null)
                {
                    //if (instance != null || CanGiveItem(i, instance))
                    if (instance == null || true)
                    {
                        slot = i;
                        return true;
                    }
                }
            }

            slot = 0;
            return false;
        }

        public InventoryItem GetSlot(int  i)
        {
            if (i >= InventoryItems.Count || i <= -1)
                return null;

            return InventoryItems[i];
        }

        public bool TryGetActiveItem(out InventoryItem inventoryitem)
        {
            inventoryitem = null;

            if (_activeItem != null && _activeItem.SlotIndex == _activeSlotIndex)
            {
                inventoryitem = _activeItem;
                return true;
            }

            if (_activeSlotIndex == -1 || _activeSlotIndex >= _inventoryItems.Count)
                return false;

            inventoryitem = _inventoryItems.Find(x => x != null && x.SlotIndex == _activeSlotIndex);

            _activeItem = inventoryitem;

            return inventoryitem != null;
        }

        public bool GetRandomItem(out InventoryItem instance)
        {
            var slotLimit = Size;
            instance = null;

            for (ushort i = 0; i < slotLimit; i++)
            {
                if (InventoryItems[i] != null)
                {
                    instance = InventoryItems[i];

                    return true;
                }
            }

            return false;
        }
    }
}