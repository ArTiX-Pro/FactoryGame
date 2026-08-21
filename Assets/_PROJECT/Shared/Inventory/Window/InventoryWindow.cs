using ArTiX.Utils.Window;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.Inventory
{
    public class InventoryWindow : ObjectWindow<Inventory>
    {
        [SerializeField] private Transform resourceDisplayContainer;
        [SerializeField] private Image prefabResourceDisplay;

        private readonly Dictionary<ItemSO, Image> resourceDisplayDictionary = new Dictionary<ItemSO, Image>();

        protected override void Initialize() 
        {
            owner.OnAddItem += DisplayAddedItem;
            owner.OnRemoveItem += OnRemoveItemFromInventory;
            foreach (KeyValuePair<ItemSO, int> slot in owner.GetInventory())
            {
                DisplayAddedItem(new Inventory.ItemSlot(slot.Key, slot.Value));
            }
        }

        private void DisplayAddedItem(Inventory.ItemSlot itemSlot)
        {
            if (!resourceDisplayDictionary.ContainsKey(itemSlot.Item))
            {
                resourceDisplayDictionary.Add(
                    key: itemSlot.Item, 
                    value: Instantiate(prefabResourceDisplay, resourceDisplayContainer));
                resourceDisplayDictionary[itemSlot.Item].sprite = itemSlot.Item.Icon;
            }

            resourceDisplayDictionary[itemSlot.Item].GetComponentInChildren<TextMeshProUGUI>().text =
                itemSlot.Quantity.ToString();
        }
        
        private void OnRemoveItemFromInventory(Inventory.ItemSlot itemSlot)
        {
            if (itemSlot.Quantity <= 0)
            {
                Destroy(resourceDisplayDictionary[itemSlot.Item].gameObject);
                resourceDisplayDictionary.Remove(itemSlot.Item);
            }
            else
            {
                resourceDisplayDictionary[itemSlot.Item].GetComponentInChildren<TextMeshProUGUI>().text =
                    itemSlot.Quantity.ToString();
            }
        }

        public override void CloseWindow()
        {
            owner.OnAddItem -= DisplayAddedItem;
            owner.OnRemoveItem -= OnRemoveItemFromInventory;
            base.CloseWindow(); 
        }
    }
}