
namespace ArTiX.Inventory
{
    public interface IItemTaker
    {
        /// <summary>
        /// The object this is called on will gain items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        void TakeItem(in ItemSO item, ref int quantity);
    }
}