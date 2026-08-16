using ArTiX.Inventory;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class Grabber : ActiveBuilding
    {
        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int inputCell;
        [SerializeField, Tooltip("The cell is relative to the building.")] private Vector2Int outputCell;

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

            if (inputHolder.CanGiveItem() && outputHolder.CanTakeItem(inputHolder.Item))
            {
                outputHolder.TakeItem(inputHolder.GiveItem());
                Debug.Log("Transfer item");
            }
            else
            {
                Debug.Log("can't transfer item");
                if (!inputHolder.CanGiveItem()) Debug.Log("Input holder can't give item");
                if (!outputHolder.CanTakeItem(inputHolder.Item)) Debug.Log("Output holder can't take item");
            }
        }

        private void CheckForHolders()
        {
            PlacedObject inputBuilding = GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + inputCell.Rotate(-transform.eulerAngles.y));

            PlacedObject outputBuilding = GridBuildingSystem.Instance.GetObjectOnCell(
                cell: Origin + outputCell.Rotate(-transform.eulerAngles.y));

            if (inputBuilding != null && inputBuilding.TryGetComponent(out IItemHolder itemHolder)) 
                inputHolder = itemHolder;
            if (outputBuilding != null && outputBuilding.TryGetComponent(out itemHolder)) 
                outputHolder = itemHolder;
        }
    }
}