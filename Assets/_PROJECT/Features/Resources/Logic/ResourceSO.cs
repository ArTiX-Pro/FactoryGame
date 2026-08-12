using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Datas/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private Transform prefab;
        public Transform Prefab => prefab;
    }
}