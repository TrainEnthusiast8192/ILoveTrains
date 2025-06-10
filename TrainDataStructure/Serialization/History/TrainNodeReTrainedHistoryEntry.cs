namespace TrainDataStructure.Serialization.History;
public sealed class TrainNodeReTrainedHistoryEntry : ITrainHistoryEntry
{
    public const char SERIALIZATION_SEPARATOR = ITrainHistoryEntry.SERIALIZATION_SEPARATOR;
    private ITrainCollection? parentTrain;
    public ITrainCollection? GetTrain() => parentTrain;
    public void SetTrain(ITrainCollection? train) => parentTrain = train;

    private IEnumerable<ITrainHistoryEntry>? storingCollection;
    public IEnumerable<ITrainHistoryEntry>? GetStoringCollection() => storingCollection;
    public void SetStoringCollection(IEnumerable<ITrainHistoryEntry>? collection) => storingCollection = collection;

    private DateTime timeStamp;
    public DateTime GetAbsoluteTimeStamp() => timeStamp;
    public void SetAbsoluteTimeStamp(DateTime timeStamp) => this.timeStamp = timeStamp;
    public int CalculateMillisToDelay(DateTime creationTime) => (timeStamp - creationTime).Milliseconds;

    private readonly string nodeFrom;
    private readonly int trainTo;

    public TrainNodeReTrainedHistoryEntry(ITrainCollection oldParentTrain, Queue<ITrainHistoryEntry> storingCollection, DateTime timeStamp, AbstractTrainNode nodeFrom, ITrainCollection? trainTo)
    {
        this.parentTrain = oldParentTrain;
        this.storingCollection = storingCollection;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.trainTo = trainTo?.GetID() ?? Int32.MaxValue;
    }
    public TrainNodeReTrainedHistoryEntry(DateTime timeStamp, AbstractTrainNode nodeFrom, ITrainCollection? trainTo)
    {
        this.parentTrain = null;
        this.storingCollection = null;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.trainTo = trainTo?.GetID() ?? Int32.MaxValue;
    }

    public string Serialize()
    {
        return $"ReTrainNode{SERIALIZATION_SEPARATOR}{parentTrain?.GetID() ?? Int32.MaxValue}{SERIALIZATION_SEPARATOR}{timeStamp}{SERIALIZATION_SEPARATOR}{nodeFrom}{SERIALIZATION_SEPARATOR}{trainTo}";
    }

    public void Execute()
    {
        NodeDeSerializer.DeSerialize(nodeFrom).ReTrain(null); // TODO: Find train by ID and put here
    }
}