using ArTiX.GridBuildingSystem.Datas;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class ResourceNode : PlacedObject, IResourceHolder
    {
        private ResourceSO resource;
        public ResourceSO Resource => resource;

        private int quantity;

        private void Awake()
        {
            ResourceNodeSO nodeDatas = datas as ResourceNodeSO;
            quantity = nodeDatas.Quantity;
            resource = nodeDatas.Resource;
        }

        public ResourceSO TakeResource()
        {
            if (quantity == 0) return null;

            quantity--;

            if (quantity == 0) GridBuildingSystem.Instance.DestroyBuilding(this);

            return resource;
        }
    }
}