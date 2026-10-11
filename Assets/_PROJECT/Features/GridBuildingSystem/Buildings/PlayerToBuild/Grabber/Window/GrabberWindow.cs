using ArTiX.FactoryGame.Buildings;
using ArTiX.Inventory;
using ArTiX.Utils;
using ArTiX.Utils.Window;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame.UI
{
    public class GrabberWindow : ObjectWindow<Grabber>
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private ItemButton prefabItemButton;

        [SerializeField] private SpriteToggle everythingToggle;
        [SerializeField] private SpriteToggle nothingToggle;

        private Dictionary<ItemSO, SpriteToggle> itemButtonDictionary = new Dictionary<ItemSO, SpriteToggle>();

        public override void Initialize(Grabber owner)
        {
            base.Initialize(owner);

            ItemButton itemButton;
            foreach (ItemSO resource in GameManager.Instance.AvailableResources)
            {
                itemButton = Instantiate(prefabItemButton, buttonContainer);
                itemButton.AddClickEvent(SelectResourceFilter, resource);
                itemButtonDictionary.Add(resource, itemButton.GetComponent<SpriteToggle>());
            }

            UpdateToggles();
            switch (owner.Filter)
            {
                case Grabber.EFilter.Everything:
                    everythingToggle.IsOn = true;
                    break;
                case Grabber.EFilter.Nothing:
                    nothingToggle.IsOn = true;
                    break;
                case Grabber.EFilter.ResourceBased:
                    List<ItemSO> filteredResources = owner.FilteredResources;
                    foreach (KeyValuePair<ItemSO, SpriteToggle> itemTogglePair in itemButtonDictionary)
                    {
                        itemTogglePair.Value.IsOn = filteredResources.Contains(itemTogglePair.Key);
                    }
                    break;
            }
        }

        public void SelectEveythingFilter()
        {
            owner.SetFilter(Grabber.EFilter.Everything, null);
            UpdateToggles();
        }

        public void SelectNothingFilter()
        {
            owner.SetFilter(Grabber.EFilter.Nothing, null);
            UpdateToggles();
        }

        private void SelectResourceFilter(ItemSO item)
        {
            owner.SetFilter(Grabber.EFilter.ResourceBased, item);
            UpdateToggles();
        }

        private void ClearToggles()
        {
            foreach (Transform buttonTransform in buttonContainer.transform)
            {
                buttonTransform.GetComponent<SpriteToggle>().IsOn = false;
            }
        }

        private void UpdateToggles()
        {
            switch (owner.Filter)
            {
                case Grabber.EFilter.Everything:
                    ClearToggles();
                    break;
                case Grabber.EFilter.Nothing:
                    ClearToggles();
                    break;
                case Grabber.EFilter.ResourceBased:
                    everythingToggle.IsOn = nothingToggle.IsOn = false;

                    List<ItemSO> filteredResources = owner.FilteredResources;
                    foreach (KeyValuePair<ItemSO, SpriteToggle> itemTogglePair in itemButtonDictionary)
                    {
                        itemTogglePair.Value.IsOn = filteredResources.Contains(itemTogglePair.Key);
                    }
                    break;
            }
        }
    }
}