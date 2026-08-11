using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class ResourceCollector : PlacedObject
    {
        private List<ResourceNode> adjacentResourceNodes;

        public override void Initialize(Vector2Int[] occupiedCells)
        {
            base.Initialize(occupiedCells);

        }
    }
}