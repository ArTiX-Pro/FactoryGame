using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Inventory;

namespace ArTiX.GridBuildingSystem.Buildings
{
    public class ResourceNode : PlacedObject
    {
        private int quantity;

        private void Awake()
        {
            quantity = (datas as ResourceNodeSO).Quantity;
        }

        public ItemSO GiveItem()
        {
            quantity--;
            if (quantity == 0) GridBuildingSystem.Instance.DestroyBuilding(this);

            return (datas as ResourceNodeSO).Resource;
        }
    }
}