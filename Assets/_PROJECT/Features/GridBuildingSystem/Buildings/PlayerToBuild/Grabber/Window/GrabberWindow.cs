using ArTiX.FactoryGame.Buildings;
using ArTiX.Inventory;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.FactoryGame.UI
{
    public class GrabberWindow : DragWindow
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private ItemButton prefabItemButton;

        private Grabber grabber;

        public void Initialize(in Grabber grabber)
        {
            this.grabber = grabber;

            ItemButton itemButton;
            foreach (ItemSO resource in GameManager.Instance.AvailableResources)
            {
                itemButton = Instantiate(prefabItemButton, buttonContainer);
                itemButton.Initialize(SelectResourceFilter, resource);
            }
        }

        public void SelectEveythingFilter() => grabber.SetFilter(Grabber.EFilter.Everything, null);
        public void SelectNothingFilter() => grabber.SetFilter(Grabber.EFilter.Nothing, null);
        private void SelectResourceFilter(ItemSO item)
        {
            grabber.SetFilter(Grabber.EFilter.ResourceBased, item);
        }
    }
}