using ArTiX.FactoryGame.Datas;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Utils.TickSystem;
using System;
using UnityEngine;

namespace ArTiX.FactoryGame.Buildings
{
    public abstract class ActiveBuilding : PlacedObject
    {
        private int tickCounter;
        private int ticksPerUpdate;

        private bool active;

        protected override void Awake()
        {
            base.Awake ();
            ticksPerUpdate = (datas as ActiveBuildingSO).TicksPerUpdate;
        }

        public override void Initialize(Vector2Int origin) 
        {
            base.Initialize(origin);
            Activate(true);
        }

        protected void Activate(in bool active)
        {
            if (this.active == active) return;

            this.active = active;

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

        public override void Destroy() 
        {
            Activate(false);
            base.Destroy(); 
        }
    }
}
