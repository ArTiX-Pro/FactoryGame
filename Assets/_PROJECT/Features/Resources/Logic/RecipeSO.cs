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
            public ResourceSO resource;
            public int quantity;
        }

        [SerializeField] private SResources[] resourcesRequiered;
        public Dictionary<ResourceSO, int> ResourcesQuantityRequiered { get; private set; } = new();

        [SerializeField] private SResources output;
        public KeyValuePair<ResourceSO, int> ResourceOutput
        {
            get => new KeyValuePair<ResourceSO, int>(output.resource, output.quantity);
        }

        private void Awake()
        {
            foreach (SResources resourcesQuantity in  resourcesRequiered)
                ResourcesQuantityRequiered.Add(resourcesQuantity.resource, resourcesQuantity.quantity);
        }
    }
}