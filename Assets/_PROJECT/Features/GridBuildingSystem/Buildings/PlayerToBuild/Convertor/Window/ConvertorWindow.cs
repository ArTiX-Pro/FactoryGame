using ArTiX.FactoryGame.Buildings;
using ArTiX.FactoryGame.Datas;
using ArTiX.Inventory.UI;
using ArTiX.Utils;
using ArTiX.Utils.Window;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    public class ConvertorWindow : ObjectWindow<Convertor>
    {
        [Header("References")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Transform resourceNeededContainer;
        [SerializeField] private Image outputResourceImage;
        [SerializeField] private Image conversionProgressImage;

        [Header("Prefab")]
        [SerializeField] private RecipeButton prefabRecipeButton;
        [SerializeField] private ItemSlotDisplay prefabResourceNeeded;

        public event Action OnClosingWindow;

        public override void Initialize(Convertor owner, string title = "Convertor")
        {
            base.Initialize(owner, title);

            RecipeButton recipeButton;
            foreach (RecipeSO recipe in GameManager.Instance.AvailableRecipes)
            {
                recipeButton = Instantiate(prefabRecipeButton, buttonContainer);
                recipeButton.AddClickEvent(OnRecipeButtonClicked, recipe);
            }

            owner.OnConverting += UpdateConversionImage;
            if (!owner.IsConverting) conversionProgressImage.fillAmount = 0;

            UpdateWindow(owner.Recipe);
        }

        private void UpdateConversionImage(float conversionProgress)
        {
            conversionProgressImage.fillAmount = conversionProgress;
        }

        private void OnRecipeButtonClicked(RecipeSO recipe)
        {
            owner.SetRecipe(recipe);
            UpdateWindow(recipe);
        }

        private void UpdateWindow(in RecipeSO recipe)
        {
            resourceNeededContainer.DestroyChildren();
            if (recipe == null)
            {
                outputResourceImage.sprite = null;
                outputResourceImage.enabled = false;
                return;
            }

            outputResourceImage.enabled = true;
            outputResourceImage.sprite = recipe.ResourceOutput.resource.Icon;

            ItemSlotDisplay resourceNeededSlot;
            foreach (RecipeSO.SResources resourceQuantityRequired in recipe.ResourcesRequiered)
            {
                resourceNeededSlot = Instantiate(prefabResourceNeeded, resourceNeededContainer);
                resourceNeededSlot.SetItemSlot(new Inventory.Inventory.ItemSlot(resourceQuantityRequired.resource, resourceQuantityRequired.quantity));
            }
        }

        public override void CloseWindow()
        {
            owner.OnConverting -= UpdateConversionImage;
            OnClosingWindow?.Invoke();
            base.CloseWindow();
        }
    }
}