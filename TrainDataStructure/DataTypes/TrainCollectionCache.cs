namespace TrainDataStructure.DataTypes;
public class TrainCollectionCache(ITrainCollection? Owner, bool IsTypeSafe, bool IsLinear, bool IsPermanent)
{
    public ITrainCollection? Owner = Owner;
    public bool IsTypeSafe = IsTypeSafe;
    public bool IsLinear = IsLinear;
    public bool IsPermanent = IsPermanent;

    public readonly Queue<AbstractTrainNode> AddedNodes = new Queue<AbstractTrainNode>();

    public TrainCollectionCacheState GetState()
    {
        return new TrainCollectionCacheState(Owner, IsTypeSafe, IsLinear, IsPermanent, AddedNodes);
    }
}
public readonly struct TrainCollectionCacheState(ITrainCollection? Owner, bool IsTypeSafe, bool IsLinear, bool IsPermanent, Queue<AbstractTrainNode> AddedNodes)
{
    public readonly ITrainCollection? Owner = Owner;
    public readonly bool IsTypeSafe = IsTypeSafe;
    public readonly bool IsLinear = IsLinear;
    public readonly bool IsPermanent = IsPermanent;

    public readonly ImmutableQueue<AbstractTrainNode> AddedNodes = ImmutableQueue.Create(AddedNodes.ToArray());
}