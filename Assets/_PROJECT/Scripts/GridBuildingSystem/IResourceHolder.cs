using ArTiX.GridBuildingSystem.Datas;

public interface IResourceHolder
{
    Resource Resource { get; }
    Resource TakeResource();
}
