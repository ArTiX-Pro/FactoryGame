using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Interaction;
using ArTiX.Inventory;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    [RequireComponent(typeof(Inventory.Inventory))]
    public class Warehouse : PlacedObject, IInteractable
    {
        private Inventory.Inventory inventory;
        private InventoryWindow window;

        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory.Inventory>();
        }

        public void Interact()
        {
            if (window == null)
            {
                window = InventoryWindow.Create(InventoryWindow.EWindowType.Inventory, inventory,
                    title: "Warehouse") as InventoryWindow;
            }
            else
                window.Focus();
        }
    }
}