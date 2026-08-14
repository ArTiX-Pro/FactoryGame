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

            PlacedObject inputCell = GridBuildingSystem.Instance.GetObjectOnCell(
                cell: origin + Vector2Int.left.Rotate(transform.eulerAngles.y));

            PlacedObject outputCell = GridBuildingSystem.Instance.GetObjectOnCell(
                cell: origin + Vector2Int.right.Rotate(transform.eulerAngles.y));

            if (inputCell != null && inputCell.TryGetComponent(out IItemHolder itemHolder)) inputHolder = itemHolder;
            if (outputCell != null && outputCell.TryGetComponent(out itemHolder)) outputHolder = itemHolder;

            if (inputHolder == null || outputHolder == null)
                Activate(false);
        }

        protected override void OnTickAction()
        {
            if (inputHolder == null || outputHolder == null)
            {
                Activate(false);
                return; 
            }

            if (inputHolder.CanGiveItem() && outputHolder.CanTakeItem(inputHolder.Item))
            {
                outputHolder.TakeItem(inputHolder.GiveItem());
                Debug.Log("Transfer item");
            }
        }
    }
}