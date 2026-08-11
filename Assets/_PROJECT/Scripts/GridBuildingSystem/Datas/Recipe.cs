using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Datas/Recipe")]
    public class Recipe : ScriptableObject
    {
        [Serializable]
        private struct SResources
        {
            public Resource resource;
            public int quantity;
        }

        [SerializeField] private SResources[] resourcesRequiered;
        public Dictionary<Resource, int> ResourcesQuantityRequiered { get; private set; } = new();

        [SerializeField] private SResources output;
        public KeyValuePair<Resource, int> ResourceOutput
        {
            get => new KeyValuePair<Resource, int>(output.resource, output.quantity);
        }

        private void Awake()
        {
            foreach (SResources resourcesQuantity in  resourcesRequiered)
                ResourcesQuantityRequiered.Add(resourcesQuantity.resource, resourcesQuantity.quantity);
        }
    }
}