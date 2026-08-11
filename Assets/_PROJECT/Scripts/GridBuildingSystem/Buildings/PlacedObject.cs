using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public abstract class PlacedObject : MonoBehaviour
    {
        [SerializeField] protected PlacedObjectDatas datas;
        public PlacedObjectDatas Datas => datas;

        public Vector2Int[] OccupiedCells { get; private set; }

        public virtual void Initialize(Vector2Int[] occupiedCells) => OccupiedCells = occupiedCells;
    }
}