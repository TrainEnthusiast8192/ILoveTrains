using System.Collections.Frozen;

namespace TrainDataStructure.DataTypes;
public class StandardTrainCache<T> : ITrainCollectionCache where T : notnull
{
    public int BranchCount = 0;
    public bool BranchCountCached = false;
    protected static readonly EqualityComparer<T> REF_EQ = EqualityComparer<T>.Create((a, b) => a is ValueType ? Equals(a, b) : ReferenceEquals(a, b), o => o.GetHashCode());

    public Dictionary<AbstractTrainNode, int> IndexOf = new(ReferenceEqualityComparer.Instance);
    public Dictionary<T, int> IndexOfValue = new(REF_EQ);
    public Dictionary<int, AbstractTrainNode> GetNodeAt = new();

    public void Invalidate()
    {
        BranchCount = 0;
        BranchCountCached = false;
        IndexOf = new(ReferenceEqualityComparer.Instance);
        IndexOfValue = new(REF_EQ);
        GetNodeAt = new();
    }

    public ITrainCollectionCache GetView() => new StandardTrainCacheView<T>(this);
}

public readonly struct StandardTrainCacheView<T>(StandardTrainCache<T> cache) : ITrainCollectionCache where T : notnull
{
    private static readonly EqualityComparer<T> REF_EQ = EqualityComparer<T>.Create((a, b) => a is ValueType ? Equals(a, b) : ReferenceEquals(a, b), o => o.GetHashCode());

    public readonly int BranchCount = cache.BranchCount;
    public readonly bool BranchCountCached = cache.BranchCountCached;

    public readonly FrozenDictionary<AbstractTrainNode, int> IndexOf = cache.IndexOf.ToFrozenDictionary(ReferenceEqualityComparer.Instance);
    public readonly FrozenDictionary<T, int> IndexOfValue = cache.IndexOfValue.ToFrozenDictionary(REF_EQ);
    public readonly FrozenDictionary<int, AbstractTrainNode> GetNodeAt = cache.GetNodeAt.ToFrozenDictionary();

    public readonly ITrainCollectionCache GetView() => this;
}