namespace TrainDataStructure.TrainCollections;

public interface ITypedTrainCollection<T> : ITrainCollection
{
    public abstract bool Add(T? value);
    public abstract bool Remove(T? value);
    public abstract bool EnforcesTypeSafety();

    public abstract bool ReplaceAt(int index, T? newValue);
    public abstract bool ReplaceAt(Index index, T? newValue);
    public abstract bool ReplaceRange(Range range, params T?[] newValues);


    public abstract T? this[int index]
    {
        get;
        set;
    }
    public abstract T? this[Index index]
    {
        get;
        set;
    }
    public abstract List<T?> this[Range range]
    {
        get;
        set;
    }
    public abstract T? this[int index, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }
    public abstract T? this[Index index, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }
    public abstract List<T?> this[Range range, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }



    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT]
    {
        get;
        set;
    }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT]
    {
        get;
        set;
    }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT]
    {
        get;
        set;
    }
    public abstract T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }
    public abstract T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }
    public abstract List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess]
    {
        get;
        set;
    }
}
