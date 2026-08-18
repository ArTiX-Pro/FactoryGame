using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Inventory
{
    public class Inventory : MonoBehaviour, IItemHolder
    {
        private class ItemSlot
        {
            public ItemSO item;
            public int quantity;
        }

        [SerializeField] private InventorySO datas;

        private readonly List<ItemSlot> inventory = new List<ItemSlot>();

        public event Action OnAddItem;

        public ItemSO Item
        {
            get
            {
                if (inventory.Count == 0) return null;
                return inventory[0].item;
            }
        }

        public bool CanGiveItem(ItemSO item = null, int quantity = 1) 
        {
            if (item == null) return Item != null;

            foreach (ItemSlot slot in inventory)
            {
                if (slot.item == item && slot.quantity >= quantity) return true;
            }

            return false;
        }
        public ItemSO GiveItem(ItemSO item = null, int quantity = 1)
        {
            ItemSlot slot = null;

            if (item == null) slot = inventory[0];
            else
            {
                foreach (ItemSlot itemSlot in inventory)
                {
                    if (itemSlot.item == item)
                    {
                        slot = itemSlot;
                        break;
                    }
                }
            }

            slot.quantity -= quantity;
            item = slot.item;
            if (slot.quantity == 0) inventory.Remove(slot);
            return item;
        }

        public bool CanTakeItem(ItemSO item)
        {
            if (item == null) return false;

            foreach (ItemSlot slot in inventory)
            {
                if (slot.item == item && slot.quantity < item.MaxStackSize) return true;
            }

            if (inventory.Count < datas.NbMaxDifferentItem) return true;

            return false;
        }

        public void TakeItem(ItemSO item)
        {
            int inventorySize = inventory.Count;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i].item == item)
                {
                    inventory[i].quantity++;
                    OnAddItem?.Invoke();
                    return;
                }
            }

            inventory.Add(new ItemSlot
            {
                item = item,
                quantity = 1
            });

            OnAddItem?.Invoke();
        }
    }
}