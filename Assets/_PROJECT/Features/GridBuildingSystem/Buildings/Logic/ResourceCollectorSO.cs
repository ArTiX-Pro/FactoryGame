using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceCollectorSO", menuName = "Datas/PlacedObject/ResourceCollectorSO")]
public class ResourceCollectorSO : PlacedObjectSO
{
    [SerializeField] private float miningTime;
    public float MiningTime => miningTime;
}
