using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceCollectorSO", menuName = "Scriptable Objects/ResourceCollectorSO")]
public class ResourceCollectorSO : PlacedObjectSO
{
    [SerializeField] private float miningTime;
    public float MiningTime => miningTime;
}
