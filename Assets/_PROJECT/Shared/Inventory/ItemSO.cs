using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.Inventory
{
    [CreateAssetMenu(fileName = "ItemSO", menuName = "Datas/Inventory/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private Transform prefab;
        public Transform Prefab => prefab;

        [SerializeField] private bool infiniteStack;
        [SerializeField, HideIf("infiniteStack")] private int maxStackSize;
        public int MaxStackSize
        {
            get
            {
                return infiniteStack ? int.MaxValue : maxStackSize;
            }
        }
    }
}