using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public abstract class PlacedObject : MonoBehaviour
    {
        [SerializeField] protected PlacedObjectSO datas;
        public PlacedObjectSO Datas => datas;

        public Vector2Int[] OccupiedCells { get; private set; }

        public virtual void Initialize(Vector2Int[] occupiedCells) => OccupiedCells = occupiedCells;
    }
}