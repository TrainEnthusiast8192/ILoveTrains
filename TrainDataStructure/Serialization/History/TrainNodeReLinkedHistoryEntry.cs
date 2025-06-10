namespace TrainDataStructure.Serialization.History;
public sealed class TrainNodeReLinkedHistoryEntry : ITrainHistoryEntry
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
    private readonly string nodeTo;

    public TrainNodeReLinkedHistoryEntry(ITrainCollection parentTrain, Queue<ITrainHistoryEntry> storingCollection, DateTime timeStamp, AbstractTrainNode nodeFrom, AbstractTrainNode? nodeTo)
    {
        this.parentTrain = parentTrain;
        this.storingCollection = storingCollection;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.nodeTo = nodeTo?.Serialize() ?? "null";
    }
    public TrainNodeReLinkedHistoryEntry(DateTime timeStamp, AbstractTrainNode nodeFrom, AbstractTrainNode? nodeTo)
    {
        this.parentTrain = null;
        this.storingCollection = null;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.nodeTo = nodeTo?.Serialize() ?? "null";
    }

    public string Serialize()
    {
        return $"ReLinkNode{SERIALIZATION_SEPARATOR}{parentTrain?.GetID() ?? Int32.MaxValue}{SERIALIZATION_SEPARATOR}{timeStamp}{SERIALIZATION_SEPARATOR}{nodeFrom}{SERIALIZATION_SEPARATOR}{nodeTo}";
    }

    public void Execute()
    {
        NodeDeSerializer.DeSerialize(nodeFrom).ReLink(NodeDeSerializer.DeSerialize(nodeTo));
    }
}