using ArTiX.FactoryGame.UI;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Interaction;
using ArTiX.Inventory.UI;
using QuickOutline;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    [RequireComponent(typeof(Inventory.Inventory))]
    public class Warehouse : PlacedObject, IInteractable
    {
        [SerializeField] private Outline outline;
        [SerializeField] private InventoryWindow prefabWindow;

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
                window = Instantiate(prefabWindow, HUD.Instance.transform);
                window.Initialize(inventory);
            }
            else
                window.Focus();
        }

        public void EnterCanInteract() => outline.enabled = true;

        public void ExitCanInteract() => outline.enabled = false;
    }
}