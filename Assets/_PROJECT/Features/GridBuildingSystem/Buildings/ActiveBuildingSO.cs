using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "ActiveBuildingSO", menuName = "Datas/PlacedObject/ActiveBuildingSO")]
    public class ActiveBuildingSO : PlacedObjectSO
    {
        [SerializeField] private float duration;
        public float Duration => duration;
    }
}