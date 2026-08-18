using ArTiX.FactoryGame.Datas;
using ArTiX.FactoryGame.UI;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Interaction;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    [RequireComponent(typeof(BoxCollider), typeof(Inventory.Inventory))]
    public class Convertor : PlacedObject, IInteractable
    {
        [SerializeField] private RecipeSO recipe;

        private ConvertorWindow window;
        private MasterTimer.Timer conversionTimer;

        private Inventory.Inventory inventory;

        private bool IsConverting => conversionTimer.IsActive;

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            conversionTimer = MasterTimer.Create(this, 0, onFinishedEvent: Convert, name: "ConversionTimer");

            inventory = GetComponent<Inventory.Inventory>();
            inventory.OnAddItem += StartConversion;
        }

        public void SetRecipe(in RecipeSO recipe) 
        {
            this.recipe = recipe;
        }

        public void Interact()
        {
            if (window == null)
            {
                window = ConvertorWindow.Create(DragWindow.EWindowType.Convertor) as ConvertorWindow;
                window.Initialize(this, recipe);
                window.OnClosingWindow += OnClosingWindow;
            }
        }

        private void OnClosingWindow() => window = null;

        private void StartConversion()
        {
            if (CanConvertResources() && !IsConverting)
            {
                Debug.Log("start conversion");
                conversionTimer.Duration = recipe.ConversionDuration;
                conversionTimer.Start();
                inventory.OnAddItem -= StartConversion;
            }
        }

        private bool CanConvertResources()
        {
            if (recipe == null) return false;

            foreach (RecipeSO.SResources resourceRequired in recipe.ResourcesRequiered)
            {
                if (!inventory.CanGiveItem(resourceRequired.resource, resourceRequired.quantity)) return false;
            }

            return true;
        }

        private void Convert()
        {
            foreach (RecipeSO.SResources resourcesRequired in recipe.ResourcesRequiered)
            {
                inventory.GiveItem(resourcesRequired.resource, resourcesRequired.quantity);
            }
            Debug.Log($"{recipe.ResourceOutput.resource} crafted !");
            inventory.TakeItem(recipe.ResourceOutput.resource);
            
            if (CanConvertResources()) conversionTimer.Start();
        }
    }
}