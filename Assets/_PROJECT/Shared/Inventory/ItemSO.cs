using UnityEngine;

namespace ArTiX.Inventory
{
    [CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private Transform prefab;
        public Transform Prefab => prefab;
    }
}