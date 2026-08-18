using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Inventory;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    public class Grabber : ActiveBuilding
    {
        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int inputCell;
        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int outputCell;

        [SerializeField] private ItemSO itemToGrab;

        private IItemHolder inputHolder;
        private IItemHolder outputHolder;

        public override void Initialize(Vector2Int origin)
        { 
            base.Initialize(origin);

            CheckForHolders();
        }

        protected override void OnTickAction()
        {
            if (inputHolder == null || outputHolder == null) 
            {
                CheckForHolders();
                if (inputHolder == null || outputHolder == null) return; 
            }

            if (itemToGrab != null) Debug.Log(itemToGrab);
            if (inputHolder.CanGiveItem(itemToGrab) && 
                outputHolder.CanTakeItem(itemToGrab == null ? inputHolder.Item : itemToGrab))
            {
                Debug.Log("transfer item");
                outputHolder.TakeItem(inputHolder.GiveItem(itemToGrab));
            }
        }

        private void CheckForHolders()
        {
            PlacedObject inputBuilding = GridBuildingSystem.GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + inputCell.Rotate(-transform.eulerAngles.y));

            PlacedObject outputBuilding = GridBuildingSystem.GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + outputCell.Rotate(-transform.eulerAngles.y));

            if (inputBuilding != null && inputBuilding.TryGetComponent(out IItemHolder itemHolder)) 
                inputHolder = itemHolder;
            if (outputBuilding != null && outputBuilding.TryGetComponent(out itemHolder)) 
                outputHolder = itemHolder;
        }
    }
}