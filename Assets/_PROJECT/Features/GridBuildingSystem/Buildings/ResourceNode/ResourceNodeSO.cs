using ArTiX.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "PlacedObjectDatas", menuName = "Datas/PlacedObject/ResourceNode")]
    public class ResourceNodeSO : PlacedObjectSO
    {
        [Title("ResourceNode datas"), Space(20)]
        [SerializeField] private ItemSO resource;
        public ItemSO Resource => resource;

        [SerializeField] private int quantity;
        public int Quantity => quantity;
    }
}