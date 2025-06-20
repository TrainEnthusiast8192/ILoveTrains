﻿namespace TrainDataStructure.TrainCollections;
public abstract class TypedTrainCollection<T> : ITrainCollection, IList<AbstractTrainNode>, ICollection<AbstractTrainNode>, IEnumerable<AbstractTrainNode>
{
    // Constructors
    #pragma warning disable IDE0060
    public TypedTrainCollection() { }
    public TypedTrainCollection(params AbstractTrainNode?[] initNodes) { }
    public TypedTrainCollection(IEnumerable<AbstractTrainNode> initNodes) { }
    public TypedTrainCollection(Span<AbstractTrainNode> initNodes) { }
    public TypedTrainCollection(params T?[] initValues) { }
    public TypedTrainCollection(IEnumerable<T?> initValues) { }
    public TypedTrainCollection(Span<T> initValues) { }
    public TypedTrainCollection(PreBuiltTrainStructure initStructure) { }
    #pragma warning restore IDE0060
    ~TypedTrainCollection() { IUniquelyIdentifiableTrainItem.ReturnID(this); }
    public abstract void Dispose();

    // Standard information
    public abstract int Count { get; }
    public abstract bool EnforcesTypeSafety { get; }
    public abstract bool IsReadOnly { get; }
    public abstract bool IsCached { get; }
    public abstract ITrainCollectionCache? GetCacheView();

    public abstract int GetID();
    public abstract AbstractTrainNode? GetFirst();
    public abstract int GetTotalCount();
    public abstract int GetBranchLength();

    // Make node for value operations
    protected abstract AbstractTrainNode MakeValueNode(T? value);
    protected abstract AbstractTrainNode MakeValueNode<M>(M? value);

    // Add value
    public abstract bool Add(T? value);
    // Add node
    public abstract bool Add(AbstractTrainNode node);
    // Add structure
    public abstract bool Add(PreBuiltTrainStructure structure);

    // Insert value
    public abstract bool Insert(int index, T? value);
    public abstract bool Insert(Index index, T? value);
    public abstract bool Insert(Range range, params T?[] values);
    public abstract bool Insert<M>(int index, M? value);
    public abstract bool Insert<M>(Index index, M? value);
    public abstract bool Insert<M>(Range range, params M?[] values);

    // Insert node
    public abstract bool Insert(int index, AbstractTrainNode node);
    public abstract bool Insert(Index index, AbstractTrainNode node);
    public abstract bool Insert(Range range, AbstractTrainNode[] nodes);

    // Add external value
    public bool AddExternal<M>(M? value) { return !EnforcesTypeSafety && HandleAddExternal<M>(value); }
    protected abstract bool HandleAddExternal<M>(M? value);

    // Remove
    public abstract bool Remove(T? value);
    public abstract bool Remove(AbstractTrainNode node);
    public abstract bool Remove(params AbstractTrainNode[] nodes);
    public abstract bool RemoveAt(int index);
    public abstract bool RemoveAt(Index index);
    public abstract bool RemoveRange(Range range);
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

    // Serialization
    public abstract List<string> SerializeHistory();
    public abstract Task<ITrainCollection> DeSerializeHistory(string[] serializedHistory);
    public abstract ITrainCollection DeSerializeHistoryInstant(string[] serializedHistory);
    public abstract void Log(ITrainHistoryEntry entry);

    // Node finding
    public abstract bool Contains(AbstractTrainNode node);
    public abstract int IndexOf(AbstractTrainNode node);
    public abstract bool Contains(T? value);
    public abstract int IndexOf(T? value);
    public abstract bool Contains<M>(M? value);
    public abstract int IndexOf<M>(M? value);
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
    public abstract IEnumerator<T?> GetValues();
    public abstract IEnumerator<T?> GetValues(params TrainSignal[] signalsToSendBeforeStartingEnumeration);

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
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
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
    #pragma warning disable IDE0060
    public TypedTrainCollection() { }
    public TypedTrainCollection(PreBuiltTrainStructure initStructure) { }
    public TypedTrainCollection(params AbstractTrainNode?[] initNodes) { }
    public TypedTrainCollection(IEnumerable<AbstractTrainNode> initNodes) { }
    public TypedTrainCollection(Span<AbstractTrainNode> initNodes) { }
    public TypedTrainCollection(params T?[] initValues) { }
    public TypedTrainCollection(IEnumerable<T?> initValues) { }
    public TypedTrainCollection(Span<T> initValues) { }
    #pragma warning restore IDE0060

    ~TypedTrainCollection() { IUniquelyIdentifiableTrainItem.ReturnID(this); }
    public abstract void Dispose();

    // Standard information
    public abstract int Count { get; }
    public abstract bool EnforcesTypeSafety { get; }
    public abstract bool IsReadOnly { get; }
    public abstract bool IsCached { get; }
    public abstract ITrainCollectionCache? GetCacheView();

    public abstract int GetID();
    public abstract AbstractTrainNode? GetFirst();
    public abstract int GetTotalCount();
    public abstract int GetBranchLength();

    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();

    // Make node for value operations
    protected abstract AbstractTrainNode MakeValueNode(T? value);
    protected abstract AbstractTrainNode MakeValueNode<M>(M? value) where M : MExternalTypeConstraint;

    // Add value
    public abstract bool Add(T? value);
    // Add node
    public abstract bool Add(AbstractTrainNode node);
    // Add structure
    public abstract bool Add(PreBuiltTrainStructure structure);

    // Insert value
    public abstract bool Insert(int index, T? value);
    public abstract bool Insert(Index index, T? value);
    public abstract bool Insert(Range range, params T?[] values);

    // Insert External
    public abstract bool Insert<M>(int index, M? value) where M : IComparable;
    public abstract bool Insert<M>(Index index, M? value) where M : IComparable;
    public abstract bool Insert<M>(Range range, params M?[] values) where M : IComparable;

    // Insert node
    public abstract bool Insert(int index, AbstractTrainNode node);
    public abstract bool Insert(Index index, AbstractTrainNode node);
    public abstract bool Insert(Range range, AbstractTrainNode[] nodes);

    // Add external value
    public bool AddExternal<M>(M? value) where M : MExternalTypeConstraint { return !EnforcesTypeSafety && HandleAddExternal<M>(value); }
    protected abstract bool HandleAddExternal<M>(M? value) where M : MExternalTypeConstraint;

    // Remove
    public abstract bool Remove(T? value);
    public abstract bool Remove(AbstractTrainNode node);
    public abstract bool Remove(params AbstractTrainNode[] nodes);
    public abstract bool RemoveAt(int index);
    public abstract bool RemoveAt(Index index);
    public abstract bool RemoveRange(Range range);
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

    // Serialization
    public abstract List<string> SerializeHistory();
    public abstract Task<ITrainCollection> DeSerializeHistory(string[] serializedHistory);
    public abstract ITrainCollection DeSerializeHistoryInstant(string[] serializedHistory);
    public abstract void Log(ITrainHistoryEntry entry);

    // Node finding
    public abstract bool Contains(AbstractTrainNode node);
    public abstract int IndexOf(AbstractTrainNode node);
    public abstract bool Contains(T? value);
    public abstract int IndexOf(T? value);
    public abstract bool Contains<M>(M? value) where M : MExternalTypeConstraint;
    public abstract int IndexOf<M>(M? value) where M : MExternalTypeConstraint;
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
    public abstract IEnumerator<T?> GetValues();
    public abstract IEnumerator<T?> GetValues(params TrainSignal[] signalsToSendBeforeStartingEnumeration);

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
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get; set; }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get; set; }
    public abstract T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get; set; }
    public abstract T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
    public abstract List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get; set; }
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