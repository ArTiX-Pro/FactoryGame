using ArTiX.FactoryGame.Buildings;
using ArTiX.FactoryGame.Datas;
using ArTiX.Utils;
using ArTiX.Utils.Window;
using System;
using TMPro;
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
        [SerializeField] private Image prefabResourceNeeded;

        public event Action OnClosingWindow;

        public override void Initialize(Convertor owner)
        {
            base.Initialize(owner);

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

            Image resourceNeededImage;
            foreach (RecipeSO.SResources resourcesQuantityRequired in recipe.ResourcesRequiered)
            {
                resourceNeededImage = Instantiate(prefabResourceNeeded, resourceNeededContainer);
                resourceNeededImage.sprite = resourcesQuantityRequired.resource.Icon;

                resourceNeededImage.GetComponentInChildren<TextMeshProUGUI>().text =
                    resourcesQuantityRequired.quantity.ToString();
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