using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "ActiveBuildingSO", menuName = "Datas/PlacedObject/ActiveBuildingSO")]
    public class ActiveBuildingSO : PlacedObjectSO
    {
        [SerializeField, Tooltip("Number of ticks between actions.")] private int ticksPerUpdate;
        public int TicksPerUpdate => ticksPerUpdate;
    }
}