using ArTiX.FactoryGame.Buildings;
using ArTiX.Inventory;
using ArTiX.Utils.Window;
using UnityEngine;

namespace ArTiX.FactoryGame.UI
{
    public class GrabberWindow : ObjectWindow<Grabber>
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private ItemButton prefabItemButton;

        protected override void Initialize()
        {
            ItemButton itemButton;
            foreach (ItemSO resource in GameManager.Instance.AvailableResources)
            {
                itemButton = Instantiate(prefabItemButton, buttonContainer);
                itemButton.AddClickEvent(SelectResourceFilter, resource);
            }
        }

        public void SelectEveythingFilter() => owner.SetFilter(Grabber.EFilter.Everything, null);
        public void SelectNothingFilter() => owner.SetFilter(Grabber.EFilter.Nothing, null);
        private void SelectResourceFilter(ItemSO item)
        {
            owner.SetFilter(Grabber.EFilter.ResourceBased, item);
        }
    }
}