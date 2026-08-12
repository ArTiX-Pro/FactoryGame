using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public abstract class ActiveBuilding : PlacedObject
    {
        protected bool active = true;

        protected ActiveBuildingSO activeDatas;
        protected float timer;

        protected virtual void Awake()
        {
            activeDatas = datas as ActiveBuildingSO;
        }

        private void Update()
        {
            if (!active) return;

            timer += Time.deltaTime;

            if (timer >= activeDatas.Duration)
            {
                timer = 0;

                OnTimerFinished();
            }
        }

        protected abstract void OnTimerFinished();
    }
}
