using ArTiX.GridBuildingSystem.Datas;

public interface IResourceHolder
{
    ResourceSO Resource { get; }
    ResourceSO TakeResource();
}
