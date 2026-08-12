using ArTiX.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Datas/Recipe")]
    public class RecipeSO : ScriptableObject
    {
        [Serializable]
        private struct SResources
        {
            public ItemSO resource;
            public int quantity;
        }

        [SerializeField] private SResources[] resourcesRequiered;
        public Dictionary<ItemSO, int> ResourcesQuantityRequiered { get; private set; } = new();

        [SerializeField] private SResources output;
        public KeyValuePair<ItemSO, int> ResourceOutput
        {
            get => new KeyValuePair<ItemSO, int>(output.resource, output.quantity);
        }

        private void Awake()
        {
            foreach (SResources resourcesQuantity in  resourcesRequiered)
                ResourcesQuantityRequiered.Add(resourcesQuantity.resource, resourcesQuantity.quantity);
        }
    }
}