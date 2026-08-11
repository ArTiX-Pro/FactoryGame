using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "PlacedObjectDatas", menuName = "Datas/PlacedObject/ResourceNode")]
    public class ResourceNodeDatas : PlacedObjectDatas
    {
        [Title("ResourceNode datas"), Space(20)]
        [SerializeField] private Resource resource;
        public Resource Resource => resource;

        [SerializeField] private int quantity;
        public int Quantity => quantity;
    }
}