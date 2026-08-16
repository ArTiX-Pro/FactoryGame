using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Datas/PlacedObject/BuildingSO")]
    public class BuildingSO : PlacedObjectSO
    {
        [Title("Building datas")]
        [SerializeField] private string objName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Ghost ghost;

        public string Name => objName;
        public Sprite Icon => icon;
        public Ghost Ghost => ghost;
    }
}