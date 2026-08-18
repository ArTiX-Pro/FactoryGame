using ArTiX.FactoryGame.Buildings;
using ArTiX.FactoryGame.Datas;
using ArTiX.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    public class ConvertorWindow : DragWindow
    {
        [Header("References")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Transform resourceNeededContainer;
        [SerializeField] private Image outputResourceImage;

        [Header("Prefab")]
        [SerializeField] private RecipeButton prefabRecipeButton;
        [SerializeField] private Image prefabResourceNeeded;

        private Convertor convertor;

        public event Action OnClosingWindow;

        public void Initialize(in Convertor convertor, in RecipeSO currentRecipe)
        {
            this.convertor = convertor;

            RecipeButton recipeButton;
            foreach (RecipeSO recipe in GameManager.Instance.AvailableRecipes)
            {
                recipeButton = Instantiate(prefabRecipeButton, buttonContainer);
                recipeButton.Initialize(OnRecipeButtonClicked, recipe);
            }

            UpdateWindow(currentRecipe);
        }

        private void OnRecipeButtonClicked(RecipeSO recipe)
        {
            convertor.SetRecipe(recipe);
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
            OnClosingWindow?.Invoke();
            base.CloseWindow(); 
        }
    }
}