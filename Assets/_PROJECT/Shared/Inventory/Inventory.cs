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

        public ItemSO Item
        {
            get
            {
                if (inventory.Count == 0) return null;
                return inventory[0].item;
            }
        }

        public bool CanGiveItem() => Item != null;
        public ItemSO GiveItem()
        {
            inventory[0].quantity--;
            ItemSO item = inventory[0].item;
            if (inventory[0].quantity == 0) inventory.RemoveAt(0);
            return item;
        }

        public bool CanTakeItem(ItemSO item)
        {
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
                    return;
                }
            }

            inventory.Add(new ItemSlot
            {
                item = item,
                quantity = 1
            });

            Debug.Log("took item");
        }
    }
}