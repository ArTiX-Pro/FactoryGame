namespace ArTiX.Inventory
{
    public interface IItemHolder
    {
        ItemSO Item { get; }
        ItemSO GiveItem();
        bool CanGiveItem();
        void TakeItem(ItemSO item);
        bool CanTakeItem(ItemSO item);
    }
}