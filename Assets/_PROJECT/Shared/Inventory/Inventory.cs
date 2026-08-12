using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Inventory
{
    public class Inventory : MonoBehaviour, IItemGiver, IItemTaker
    {
        [SerializeField] private InventorySO datas;

        private readonly Dictionary<ItemSO, int> inventory = new Dictionary<ItemSO, int>();

        public ItemSO GiveItem()
        {
            foreach (KeyValuePair<ItemSO, int> itemQuantPair in inventory)
            {
                inventory[itemQuantPair.Key]--;
                if (inventory[itemQuantPair.Key] == 0) inventory.Remove(itemQuantPair.Key);
                
                return itemQuantPair.Key;
            }

            return null;
        }

        public ItemSO GiveItem(in ItemSO item, ref int quantity)
        {
            if (inventory.TryGetValue(item, out int currentQuantity))
            {
                quantity = Mathf.Min(quantity, currentQuantity);
                inventory[item] -= quantity;
                if (inventory[item] == 0) inventory.Remove(item);

                return item;
            }
            else return null;
        }

        public void TakeItem(in ItemSO item, ref int quantity)
        {
            if (!inventory.ContainsKey(item)) 
            {
                if (inventory.Keys.Count == datas.NbMaxDifferentItem) return;

                inventory.Add(item, 0); 
            }

            if (inventory[item] + quantity <= item.MaxStackSize)
            {
                inventory[item] += quantity;
                quantity = 0;
            }
            else
            {
                quantity -= item.MaxStackSize - inventory[item];
                inventory[item] = item.MaxStackSize;
            }
        }
    }
}