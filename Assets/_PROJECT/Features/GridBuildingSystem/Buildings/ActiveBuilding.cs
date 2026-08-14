using ArTiX.GridBuildingSystem.Datas;
using System;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public abstract class ActiveBuilding : PlacedObject
    {
        private int tickCounter;
        private int ticksPerUpdate;

        protected virtual void Awake()
        {
            ticksPerUpdate = (datas as ActiveBuildingSO).TicksPerUpdate;
        }

        private void Start()
        {
            TickSystem.Instance.OnTick += IncrementTickCounter;
        }

        protected void Activate(in bool active)
        {
            if (active) TickSystem.Instance.OnTick += IncrementTickCounter;
            else TickSystem.Instance.OnTick -= IncrementTickCounter;
        }

        protected abstract void OnTickAction();

        private void IncrementTickCounter(object sender, EventArgs args)
        {
            tickCounter++;
            if (tickCounter == ticksPerUpdate)
            {
                tickCounter = 0;
                OnTickAction();
            }
        }
    }
}
