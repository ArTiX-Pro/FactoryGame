using ArTiX.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArTiX.Inventory.Inventory;

namespace ArTiX.Inventory.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemSlotDisplay : MonoBehaviour, IMouseTooltip
    {
        private string itemName = "";
        public string Tooltip => itemName;

        public void SetItemSlot(ItemSlot slot)
        {
            itemName = slot.Item.Name;
            GetComponent<Image>().sprite = slot.Item.Icon;
            GetComponentInChildren<TextMeshProUGUI>().text = slot.Quantity.ToString();
        }
    }
}