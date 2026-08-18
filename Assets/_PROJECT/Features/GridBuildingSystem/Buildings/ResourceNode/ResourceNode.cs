using ArTiX.FactoryGame.Datas;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Inventory;

namespace ArTiX.FactoryGame.Buildings
{
    public class ResourceNode : PlacedObject
    {
        private int quantity;
        public ItemSO Item => (datas as ResourceNodeSO).Resource;

        private void Awake()
        {
            quantity = (datas as ResourceNodeSO).Quantity;
        }

        public ItemSO GiveItem()
        {
            quantity--;
            if (quantity == 0) GridBuildingSystem.GridBuildingSystem.Instance.DestroyBuilding(this);

            return (datas as ResourceNodeSO).Resource;
        }
    }
}