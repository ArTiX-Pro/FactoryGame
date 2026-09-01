using ArTiX.FactoryGame.UI;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Interaction;
using ArTiX.Inventory;
using ArTiX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    public class Grabber : ActiveBuilding, IInteractable
    {
        [Serializable]
        public enum EFilter
        {
            /// <summary>
            /// Everything can pass
            /// </summary>
            Everything,
            /// <summary>
            /// Nothing can pass
            /// </summary>
            Nothing,
            ResourceBased
        }

        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int inputCell;
        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int outputCell;

        [SerializeField] private Outline outline;

        private IItemHolder inputHolder;
        private IItemHolder outputHolder;

        public EFilter Filter { get; private set; }
        private readonly HashSet<ItemSO> filteredResources = new HashSet<ItemSO>();
        public List<ItemSO> FilteredResources
        {
            get => filteredResources.ToList();
        }

        public override void Initialize(Vector2Int origin)
        {
            base.Initialize(origin);

            CheckForItemHolders();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="resource">Requiered only if filter is set to ResourceBased. 
        /// If resource is already in the filtered resource list, it will remove it from the list.</param>
        public void SetFilter(in EFilter filter, in ItemSO resource = null)
        {
            Filter = filter;
            if (filter == EFilter.ResourceBased)
            {
                if (filteredResources.Contains(resource))
                {
                    filteredResources.Remove(resource);
                    if (filteredResources.Count == 0)
                        Filter = EFilter.Everything;
                }
                else
                    filteredResources.Add(resource);
            }
            else
                filteredResources.Clear();

            Activate(active: filter != EFilter.Nothing);
        }

        protected override void OnTickAction()
        {
            if (inputHolder == null || outputHolder == null)
            {
                CheckForItemHolders();
                if (inputHolder == null || outputHolder == null) return;
            }

            if (Filter == EFilter.ResourceBased)
            {
                foreach (ItemSO resource in filteredResources)
                {
                    if (inputHolder.CanGiveItem(resource) && outputHolder.CanTakeItem(resource))
                    {
                        outputHolder.TakeItem(inputHolder.GiveItem(resource));
                        return;
                    }
                }
            }
            // In this case, Filter == Everything
            else if (inputHolder.CanGiveItem() && outputHolder.CanTakeItem(inputHolder.Item))
            {
                outputHolder.TakeItem(inputHolder.GiveItem());
            }
        }

        private void CheckForItemHolders()
        {
            // Negating the Y angle because we look at it from the top, so in the opposite direction.
            PlacedObject inputBuilding = GridBuildingSystem.GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + inputCell.Rotate(-transform.eulerAngles.y));

            PlacedObject outputBuilding = GridBuildingSystem.GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + outputCell.Rotate(-transform.eulerAngles.y));

            if (inputBuilding != null && inputBuilding.TryGetComponent(out IItemHolder itemHolder))
                inputHolder = itemHolder;
            if (outputBuilding != null && outputBuilding.TryGetComponent(out itemHolder))
                outputHolder = itemHolder;
        }

        public void Interact()
        {
            GrabberWindow window = GrabberWindow.Create(GrabberWindow.EWindowType.Grabber, this) as GrabberWindow;
        }

        public void EnterCanInteract()
        {
            outline.enabled = true;
        }

        public void ExitCanInteract()
        {
            outline.enabled = false;
        }
    }
}