namespace ArTiX.Inventory
{
    public interface IItemGiver
    {
        /// <summary>
        /// The object this is called on will lose items.
        /// </summary>
        /// <returns></returns>
        ItemSO GiveItem();
        /// <summary>
        /// The object this is called on will lose items.
        /// </summary>
        /// <returns></returns>
        ItemSO GiveItem(in ItemSO item, ref int quantity);
    }
}