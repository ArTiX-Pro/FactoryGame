using ArTiX.GridBuildingSystem.Datas;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class ResourceNode : PlacedObject, IResourceHolder
    {
        private Resource resource;
        public Resource Resource => resource;

        private int quantity;

        private void Awake()
        {
            ResourceNodeDatas nodeDatas = datas as ResourceNodeDatas;
            quantity = nodeDatas.Quantity;
            resource = nodeDatas.Resource;
        }

        public Resource TakeResource()
        {
            if (quantity == 0) return null;

            quantity--;

            if (quantity == 0) GridBuildingSystem.Instance.DestroyBuilding(this);

            return resource;
        }
    }
}