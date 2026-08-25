using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils.TickSystem;
using UnityEngine;

namespace ArTiX.FactoryGame.Datas
{
    [CreateAssetMenu(fileName = "ActiveBuildingSO", menuName = "Datas/PlacedObject/ActiveBuildingSO")]
    public class ActiveBuildingSO : BuildingSO
    {
        [SerializeField, Tooltip("Number of ticks between actions.")] private int ticksPerUpdate;
        public int TicksPerUpdate => ticksPerUpdate;

        [SerializeField] private TickSystem.ETick tickType;
        public TickSystem.ETick TickType => tickType;
    }
}