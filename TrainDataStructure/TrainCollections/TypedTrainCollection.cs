namespace TrainDataStructure.TrainCollections;
public abstract class TypedTrainCollection<T> : ITrainCollection, IList<AbstractTrainNode>, ICollection<AbstractTrainNode>, IEnumerable<AbstractTrainNode>
{
    // Constructors
    public TypedTrainCollection() { }
    public TypedTrainCollection(params AbstractTrainNode?[] initNodes) { }
    public TypedTrainCollection(params T?[] initValues) { }

    // Standard information
    public abstract int Count { get; }
    public abstract bool EnforcesTypeSafety { get; }
    public abstract bool IsReadOnly { get; }
    public abstract bool IsCached { get; }

    public abstract Guid GetID();
    public abstract AbstractTrainNode? GetFirst();
    public abstract int GetTotalCount();
    public abstract int GetBranchLength();

    // Add value
    public abstract bool Add(T? value);
    // Add node
    public abstract bool Add(AbstractTrainNode node);

    // Insert value
    public abstract bool Insert(int index, T? value);
    public abstract bool Insert(Index index, T? value);
    public abstract bool Insert(Range range, params T?[] value);

    // Insert node
    public abstract bool Insert(int index, AbstractTrainNode value);
    public abstract bool Insert(Index index, AbstractTrainNode value);
    public abstract bool Insert(Range range, AbstractTrainNode[] value);

    // Add external value
    public bool AddExternal<M>(M? value) { return !EnforcesTypeSafety && HandleAddExternal<M>(value); }
    protected abstract bool HandleAddExternal<M>(M? value);

    // Remove
    public abstract bool Remove(T? value);
    public abstract bool Remove(AbstractTrainNode node);
    public abstract bool RemoveAt(int index);
    public abstract bool RemoveAt(Index index);
    public abstract bool Clear();

    // Replace value
    public abstract bool ReplaceAt(int index, T? newValue);
    public abstract bool ReplaceAt(Index index, T? newValue);
    public abstract bool ReplaceRange(Range range, params T?[] newValues);

    // Send signal
    public abstract bool Signal(TrainSignal signal);
    public abstract bool Signal(params TrainSignal[] signals);

    // Collapse to linear list
    public abstract List<AbstractTrainNode> Collapse();
    public abstract List<AbstractTrainNode> BranchCollapse();
    public abstract List<AbstractTrainNode> RawCollapse();
    public abstract List<AbstractTrainNode> RawBranchCollapse();

    // Collapse to strings
    public abstract List<string> PrintTrain(bool printToConsole = true);
    public abstract List<string> PrintBranch(bool printToConsole = true);
    public abstract List<string> RawPrintTrain(bool printToConsole = true);
    public abstract List<string> RawPrintBranch(bool printToConsole = true);

    // Node finding
    public abstract bool Contains(AbstractTrainNode item);
    public abstract int IndexOf(AbstractTrainNode item);
    public abstract bool Contains(T item);
    public abstract int IndexOf(T item);
    public abstract bool Contains<M>(M? item);
    public abstract int IndexOf<M>(M? item);
    public abstract AbstractTrainNode? GetNodeAt(int index);
    public abstract AbstractTrainNode? GetNodeAt(Index index);
    public abstract List<AbstractTrainNode> GetNodesAt(Range range);

    // Collapse to given array
    public abstract void CopyTo(AbstractTrainNode[] array);
    public abstract void CopyTo(AbstractTrainNode[] array, int arrayIndex);
    public abstract void CopyTo(AbstractTrainNode[] array, Index arrayIndex);

    // Collapse to lazy enumerator
    public abstract IEnumerator<AbstractTrainNode> GetEnumerator();

    // Recast to another type
    public abstract TypedTrainCollection<M> Cast<M>();

    #region Indexer Hell
    // Implictly-Inferred Indexers
    public abstract T? this[int index] { get; set; }
    public abstract T? this[Index index] { get; set; }
    public abstract List<T?> this[Range range] { get; set; }
    public abstract T? this[int index, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }

    // Explicitly-Inferred Indexers
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    #endregion

    #region Interface Boilerplate
    void IList<AbstractTrainNode>.Insert(int index, AbstractTrainNode item) => Insert(index, item);
    void ICollection<AbstractTrainNode>.Add(AbstractTrainNode item) => Add(item);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    void IList<AbstractTrainNode>.RemoveAt(int index) => RemoveAt(index);
    void ICollection<AbstractTrainNode>.Clear() => Clear();

    AbstractTrainNode IList<AbstractTrainNode>.this[int index]
    {
        get => this[index, IndexerInference.NODE] ?? throw new ArgumentException($"No node found at index {index}", nameof(index));
        set => this[index, IndexerInference.NODE] = value;
    }
    #endregion
}

//////////////////////////////////////////////////////////////////////////////////////////

public abstract class TypedTrainCollection<T, MExternalTypeConstraint> : ITrainCollection, IList<AbstractTrainNode>, ICollection<AbstractTrainNode>, IEnumerable<AbstractTrainNode>
{
    // Constructors
    public TypedTrainCollection() { }
    public TypedTrainCollection(params AbstractTrainNode?[] initNodes) { }
    public TypedTrainCollection(params T?[] initValues) { }

    // Standard information
    public abstract int Count { get; }
    public abstract bool EnforcesTypeSafety { get; }
    public abstract bool IsReadOnly { get; }
    public abstract bool IsCached { get; }

    public abstract Guid GetID();
    public abstract AbstractTrainNode? GetFirst();
    public abstract int GetTotalCount();
    public abstract int GetBranchLength();

    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();

    // Add value
    public abstract bool Add(T? value);
    // Add node
    public abstract bool Add(AbstractTrainNode node);

    // Insert value
    public abstract bool Insert(int index, T? value);
    public abstract bool Insert(Index index, T? value);
    public abstract bool Insert(Range range, params T?[] value);

    // Insert node
    public abstract bool Insert(int index, AbstractTrainNode value);
    public abstract bool Insert(Index index, AbstractTrainNode value);
    public abstract bool Insert(Range range, AbstractTrainNode[] value);

    // Add external value
    public bool AddExternal<M>(M? value) where M : MExternalTypeConstraint { return !EnforcesTypeSafety && HandleAddExternal<M>(value); }
    protected abstract bool HandleAddExternal<M>(M? value) where M : MExternalTypeConstraint;

    // Remove
    public abstract bool Remove(T? value);
    public abstract bool Remove(AbstractTrainNode node);
    public abstract bool RemoveAt(int index);
    public abstract bool RemoveAt(Index index);
    public abstract bool Clear();

    // Replace value
    public abstract bool ReplaceAt(int index, T? newValue);
    public abstract bool ReplaceAt(Index index, T? newValue);
    public abstract bool ReplaceRange(Range range, params T?[] newValues);

    // Send signal
    public abstract bool Signal(TrainSignal signal);
    public abstract bool Signal(params TrainSignal[] signals);

    // Collapse to linear list
    public abstract List<AbstractTrainNode> Collapse();
    public abstract List<AbstractTrainNode> BranchCollapse();
    public abstract List<AbstractTrainNode> RawCollapse();
    public abstract List<AbstractTrainNode> RawBranchCollapse();

    // Collapse to strings
    public abstract List<string> PrintTrain(bool printToConsole = true);
    public abstract List<string> PrintBranch(bool printToConsole = true);
    public abstract List<string> RawPrintTrain(bool printToConsole = true);
    public abstract List<string> RawPrintBranch(bool printToConsole = true);

    // Node finding
    public abstract bool Contains(AbstractTrainNode item);
    public abstract int IndexOf(AbstractTrainNode item);
    public abstract bool Contains(T? item);
    public abstract int IndexOf(T? item);
    public abstract bool Contains<M>(M? item) where M : MExternalTypeConstraint;
    public abstract int IndexOf<M>(M? item) where M : MExternalTypeConstraint;
    public abstract AbstractTrainNode? GetNodeAt(int index);
    public abstract AbstractTrainNode? GetNodeAt(Index index);
    public abstract List<AbstractTrainNode> GetNodesAt(Range range);

    // Collapse to given array
    public abstract void CopyTo(AbstractTrainNode[] array);
    public abstract void CopyTo(AbstractTrainNode[] array, int arrayIndex);
    public abstract void CopyTo(AbstractTrainNode[] array, Index arrayIndex);

    // Collapse to lazy enumerator
    public abstract IEnumerator<AbstractTrainNode> GetEnumerator();
    public abstract IEnumerator<AbstractTrainNode> GetEnumerator(params TrainSignal[] signalsToSendBeforeStartingEnumeration);

    // Recast to another type
    public abstract TypedTrainCollection<M, MExternalTypeConstraint> Cast<M>() where M : MExternalTypeConstraint;

    #region Indexer Hell
    // Implictly-Inferred Indexers
    public abstract T? this[int index] { get; set; }
    public abstract T? this[Index index] { get; set; }
    public abstract List<T?> this[Range range] { get; set; }
    public abstract T? this[int index, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }

    // Explicitly-Inferred Indexers
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.Node.node] { get; set; }
    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    #endregion

    #region Interface Boilerplate
    void IList<AbstractTrainNode>.Insert(int index, AbstractTrainNode item) => Insert(index, item);

    void ICollection<AbstractTrainNode>.Add(AbstractTrainNode item) => Add(item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void IList<AbstractTrainNode>.RemoveAt(int index) => RemoveAt(index);

    void ICollection<AbstractTrainNode>.Clear() => Clear();

    AbstractTrainNode IList<AbstractTrainNode>.this[int index]
    {
        get => this[index, IndexerInference.NODE] ?? throw new ArgumentException($"No node found at index {index}", nameof(index));
        set => this[index, IndexerInference.NODE] = value;
    }
    #endregion
}