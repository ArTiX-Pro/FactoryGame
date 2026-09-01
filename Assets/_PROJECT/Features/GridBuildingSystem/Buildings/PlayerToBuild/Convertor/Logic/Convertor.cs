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
        [SerializeField] private Outline outline;

        public RecipeSO Recipe { get; private set; }

        private ConvertorWindow window;
        private STimerHandle conversionTimer;

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
            conversionTimer = Timer.Create(this, 0, onTimerActiveEvent: UpdateConversionProgress,
                onFinishedEvent: FinishConverting, name: "ConversionTimer");

            inventory = GetComponent<Inventory.Inventory>();
            inventory.OnAddItem += StartConversion;
        }

        public void SetRecipe(in RecipeSO recipe)
        {
            Recipe = recipe;
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

        #region Interaction

        public void Interact()
        {
            if (window == null)
            {
                window = ConvertorWindow.Create(ConvertorWindow.EWindowType.Convertor, this) as ConvertorWindow;
                window.OnClosingWindow += OnClosingWindow;
            }
        }

        public void EnterCanInteract() => outline.enabled = true;

        public void ExitCanInteract() => outline.enabled = false;

        #endregion

        #region Conversion

        private void UpdateConversionProgress()
        {
            OnConverting?.Invoke(conversionTimer.ElapsedTimeAsPercentage);
        }

        private void StartConversion(object sender, Inventory.Inventory.ItemSlot itemSlot)
        {
            if (CanConvertResources() && !IsConverting)
            {
                Debug.Log("start conversion");
                conversionTimer.SetDuration(Recipe.ConversionDuration);
                conversionTimer.Play();
                inventory.OnAddItem -= StartConversion;
            }
        }

        private bool CanConvertResources()
        {
            if (Recipe == null) return false;

            foreach (RecipeSO.SResources resourceRequired in Recipe.ResourcesRequiered)
            {
                if (!inventory.CanGiveItem(resourceRequired.resource, resourceRequired.quantity)) return false;
            }

            return true;
        }

        private void FinishConverting()
        {
            foreach (RecipeSO.SResources resourcesRequired in Recipe.ResourcesRequiered)
            {
                inventory.GiveItem(resourcesRequired.resource, resourcesRequired.quantity);
            }
            Debug.Log($"{Recipe.ResourceOutput.resource} crafted !");
            inventory.TakeItem(Recipe.ResourceOutput.resource);

            if (CanConvertResources()) conversionTimer.Play();
        }

        #endregion

        private void OnClosingWindow() => window = null;
    }
}