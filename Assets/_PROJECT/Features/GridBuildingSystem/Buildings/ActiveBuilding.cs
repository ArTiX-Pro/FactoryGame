using ArTiX.FactoryGame.Datas;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Utils.TickSystem;
using System;

namespace ArTiX.FactoryGame.Buildings
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
            Activate(true);
        }

        protected void Activate(in bool active)
        {
            if (active)
            {
                switch ((datas as ActiveBuildingSO).TickType)
                {
                    case TickSystem.ETick.First:
                        TickSystem.Instance.OnFirstTick += IncrementTickCounter;
                        break;
                    case TickSystem.ETick.Main:
                        TickSystem.Instance.OnTick += IncrementTickCounter;
                        break;
                    case TickSystem.ETick.Late:
                        TickSystem.Instance.OnLateTick += IncrementTickCounter;
                        break;
                }
            }
            else
            {

                switch ((datas as ActiveBuildingSO).TickType)
                {
                    case TickSystem.ETick.First:
                        TickSystem.Instance.OnFirstTick -= IncrementTickCounter;
                        break;
                    case TickSystem.ETick.Main:
                        TickSystem.Instance.OnTick -= IncrementTickCounter;
                        break;
                    case TickSystem.ETick.Late:
                        TickSystem.Instance.OnLateTick -= IncrementTickCounter;
                        break;
                }
            }
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
