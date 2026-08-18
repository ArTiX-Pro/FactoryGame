using ArTiX.Inventory;
using System;
using UnityEngine;

namespace ArTiX.FactoryGame.Datas
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Datas/Recipe")]
    public class RecipeSO : ScriptableObject
    {
        [Serializable]
        public struct SResources
        {
            public ItemSO resource;
            public int quantity;
        }

        [SerializeField] private SResources[] resourcesRequiered;
        public SResources[] ResourcesRequiered => resourcesRequiered;

        [SerializeField] private SResources output;
        public SResources ResourceOutput => output;

        [SerializeField] private float conversionDuration;
        public float ConversionDuration => conversionDuration;
    }
}