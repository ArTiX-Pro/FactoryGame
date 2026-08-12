using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class ResourceCollector : PlacedObject
    {
        private List<ResourceNode> adjacentResourceNodes = new List<ResourceNode>();

        private ResourceCollectorSO resourceCollectorDatas;
        private float miningTimer;

        private bool active = true;

        private void Awake()
        {
            resourceCollectorDatas = datas as ResourceCollectorSO;
        }

        private void Update()
        {
            if (!active) return;

            miningTimer += Time.deltaTime;

            if (miningTimer >= resourceCollectorDatas.MiningTime)
            {
                miningTimer = 0;

                if (adjacentResourceNodes[0] == null) 
                {
                    adjacentResourceNodes.RemoveAt(0); 
                    if (adjacentResourceNodes.Count == 0)
                    {
                        active = false;
                        return;
                    }
                }

                adjacentResourceNodes[0].TakeResource();
            }
        }

        public override void Initialize(Vector2Int[] occupiedCells)
        {
            base.Initialize(occupiedCells);

            Vector2Int bottomLeftCell = occupiedCells[0], topRightCell = occupiedCells[0];

            // Get extremities
            foreach (Vector2Int cell in occupiedCells)
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

            foreach (ResourceNode nodes in adjacentResourceNodes)
                Debug.Log(nodes);
        }
    }
}