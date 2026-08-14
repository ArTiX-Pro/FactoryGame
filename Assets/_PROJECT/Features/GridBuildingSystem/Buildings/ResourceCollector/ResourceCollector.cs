using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    [RequireComponent(typeof(Inventory.Inventory))]
    public class ResourceCollector : ActiveBuilding
    {
        private Inventory.Inventory inventory;

        private readonly List<ResourceNode> adjacentResourceNodes = new List<ResourceNode>();

        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory.Inventory>();
        }

        public override void Initialize(Vector2Int origin)
        {
            base.Initialize(origin);

            Vector2Int bottomLeftCell = Origin, topRightCell = Origin;

            // Get extremities
            foreach (Vector2Int cell in GetOccupiedCells())
            {
                bottomLeftCell.x = Mathf.Min(bottomLeftCell.x, cell.x);
                bottomLeftCell.y = Mathf.Min(bottomLeftCell.y, cell.y);
                topRightCell.x = Mathf.Max(topRightCell.x, cell.x);
                topRightCell.y = Mathf.Max(topRightCell.y, cell.y);
            }

            int width = topRightCell.x - bottomLeftCell.x + 1;
            int height = topRightCell.y - bottomLeftCell.y + 1;

            GridBuildingSystem grid = GridBuildingSystem.Instance;
            // Bottom line
            Vector2Int startCell = bottomLeftCell + Vector2Int.down;
            int i;
            ResourceNode resourceNode;

            void AddResourceNodeToList(in Vector2Int cell)
            {
                resourceNode = grid.GetObjectOnCell(cell) as ResourceNode;
                if (resourceNode != null && !adjacentResourceNodes.Contains(resourceNode))
                    adjacentResourceNodes.Add(resourceNode);
            }

            for (i = 0; i < width; i++)
                AddResourceNodeToList(startCell + (Vector2Int.right * i));

            // Left line
            startCell = bottomLeftCell + Vector2Int.left;
            for (i = 0; i < height; i++)
                AddResourceNodeToList(startCell + (Vector2Int.up * i));

            // Top line
            startCell = new Vector2Int(bottomLeftCell.x, topRightCell.y + 1);
            for (i = 0; i < width; i++)
                AddResourceNodeToList(startCell + (Vector2Int.right * i));

            // Right line
            startCell = topRightCell + Vector2Int.right;
            for (i = 0; i < height; i++)
                AddResourceNodeToList(startCell + (Vector2Int.down * i));

            if (adjacentResourceNodes.Count == 0) Activate(false);
        }

        protected override void OnTickAction()
        {
            if (adjacentResourceNodes[0] == null)
            {
                adjacentResourceNodes.RemoveAt(0);
                if (adjacentResourceNodes.Count == 0)
                {
                    Activate(false);
                    return;
                }
            }

            if (inventory.CanTakeItem(adjacentResourceNodes[0].Item))
            {
                inventory.TakeItem(adjacentResourceNodes[0].GiveItem());
                Debug.Log("took item");
            }
            else
                Debug.Log("Inventory full");
        }
    }
}