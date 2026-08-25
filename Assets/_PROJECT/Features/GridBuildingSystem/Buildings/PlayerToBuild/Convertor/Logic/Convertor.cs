using ArTiX.FactoryGame.Datas;
using ArTiX.FactoryGame.UI;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Interaction;
using ArTiX.Utils;
using System;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    [RequireComponent(typeof(BoxCollider), typeof(Inventory.Inventory))]
    public class Convertor : PlacedObject, IInteractable
    {
        [SerializeField] private RecipeSO recipe;
        public RecipeSO CurrentRecipe => recipe;

        private ConvertorWindow window;
        private MasterTimer.Timer conversionTimer;

        private Inventory.Inventory inventory;

        public bool IsConverting => conversionTimer.IsActive;

        /// <summary>
        /// The parameter is the progress.
        /// </summary>
        public event Action<float> OnConverting;

        protected override void Awake()
        {
            base.Awake();

            GetComponent<BoxCollider>().isTrigger = true;
            conversionTimer = MasterTimer.Create(this, 0, onTimerActiveEvent: UpdateConversionProgress, 
                onFinishedEvent: FinishConverting, name: "ConversionTimer");

            inventory = GetComponent<Inventory.Inventory>();
            inventory.OnAddItem += StartConversion;
        }

        public void SetRecipe(in RecipeSO recipe) 
        {
            this.recipe = recipe;
            if (IsConverting)
            {
                conversionTimer.Stop();
                if (!CanConvertResources())
                {
                    OnConverting?.Invoke(0);
                    inventory.OnAddItem += StartConversion;
                }
                else StartConversion(this, null);
            }
        }

        public void Interact()
        {
            if (window == null)
            {
                window = ConvertorWindow.Create(ConvertorWindow.EWindowType.Convertor, this) as ConvertorWindow;
                window.OnClosingWindow += OnClosingWindow;
            }
        }

        private void UpdateConversionProgress()
        {
            OnConverting?.Invoke(conversionTimer.ElapsedTimeAsPercentage);
        }

        private void StartConversion(object sender, Inventory.Inventory.ItemSlot itemSlot)
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

        private void FinishConverting()
        {
            foreach (RecipeSO.SResources resourcesRequired in recipe.ResourcesRequiered)
            {
                inventory.GiveItem(resourcesRequired.resource, resourcesRequired.quantity);
            }
            Debug.Log($"{recipe.ResourceOutput.resource} crafted !");
            inventory.TakeItem(recipe.ResourceOutput.resource);
            
            if (CanConvertResources()) conversionTimer.Start();
        }

        private void OnClosingWindow() => window = null;
    }
}