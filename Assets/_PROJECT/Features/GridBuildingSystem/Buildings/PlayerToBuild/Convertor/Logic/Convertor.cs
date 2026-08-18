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

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            conversionTimer = MasterTimer.Create(this, 0, onFinishedEvent: Convert);

            inventory = GetComponent<Inventory.Inventory>();
        }

        public void SetRecipe(in RecipeSO recipe) 
        {
            this.recipe = recipe;
        }

        public void CloseWindow() => window = null;

        public void Interact()
        {
            if (window == null)
            {
                window = ConvertorWindow.Create(Utils.DragWindow.EWindowType.Convertor) as ConvertorWindow;
                window.Initialize(this, recipe);
            }
        }

        private bool CanConvertResources()
        {
            foreach (RecipeSO.SResources resourceRequired in recipe.ResourcesRequiered)
            { 
            }

            return true;
        }

        private void Convert()
        {

        }
    }
}