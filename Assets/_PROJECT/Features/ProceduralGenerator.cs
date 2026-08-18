using ArTiX.FactoryGame.Datas;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame
{
    public class ProceduralGenerator : MonoBehaviour
    {
        [SerializeField] private GridSO gridDatas;
        [SerializeField] private List<ResourceNodeSO> resourceNodes;

        private void Start()
        {
            List<Vector2Int> possibleCells = new List<Vector2Int>();

            for (int x = 0; x < gridDatas.Width; x++)
            {
                for (int y = 0; y < gridDatas.Height; y++)
                    possibleCells.Add(new Vector2Int(x, y));
            }

            Vector2Int selectedCell;
            while (resourceNodes.Count > 0)
            {
                selectedCell = possibleCells.GetRandomElement();
                possibleCells.Remove(selectedCell);
                GridBuildingSystem.GridBuildingSystem.Instance.CreateBuilding(resourceNodes[0], selectedCell);
                resourceNodes.RemoveAt(0);
            }

            Destroy(gameObject);
        }
    }
}