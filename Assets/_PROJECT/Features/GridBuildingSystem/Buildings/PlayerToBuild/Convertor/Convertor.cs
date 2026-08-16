using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{

    public class Convertor : ActiveBuilding, IItemHolder
    {
        [SerializeField] private RecipeSO recipe;

        private Dictionary<ItemSO, int> resourcesQuantityDic = new Dictionary<ItemSO, int>();

        private RecipeSO.SResources resourceQuantity;
        public ItemSO Item
        {
            get
            {
                if (resourceQuantity.quantity == 0) return null;
                return resourceQuantity.resource;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            resourceQuantity = new RecipeSO.SResources()
            {
                resource = recipe.ResourceOutput.Key,
                quantity = 0
            };
        }

        public bool CanGiveItem() => Item != null;
        public ItemSO GiveItem()
        {
            resourceQuantity.quantity--;
            return resourceQuantity.resource;
        }

        public bool CanTakeItem(ItemSO item) => recipe.ResourcesQuantityRequiered.ContainsKey(item);
        public void TakeItem(ItemSO item)
        {
            if (!resourcesQuantityDic.ContainsKey(item)) resourcesQuantityDic.Add(item, 0);

            resourcesQuantityDic[item]++;
        }

        protected override void OnTickAction()
        {

        }
    }
}