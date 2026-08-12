using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public abstract class PlacedObject : MonoBehaviour
    {
        [SerializeField] protected PlacedObjectSO datas;
        public PlacedObjectSO Datas => datas;

        public Vector2Int Origin { get; private set; }

        public virtual void Initialize(Vector2Int origin) => Origin = origin;

        public List<Vector2Int> GetOccupiedCells()
        {
            List<Vector2Int> occupiedCells = new List<Vector2Int>();

            foreach (Vector2Int cell in datas.CellArray)
            {
                occupiedCells.Add(Origin + cell.Rotate(transform.eulerAngles.y));
            }

            return occupiedCells;
        }
    }
}