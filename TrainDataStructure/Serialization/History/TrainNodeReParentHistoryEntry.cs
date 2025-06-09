namespace TrainDataStructure.Serialization.History;
public sealed class TrainNodeReParentHistoryEntry : ITrainHistoryEntry
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

    private string nodeFrom;
    private string nodeTo;

    public TrainNodeReParentHistoryEntry(ITrainCollection parentTrain, Queue<ITrainHistoryEntry> storingCollection, DateTime timeStamp, AbstractTrainNode nodeFrom, AbstractTrainNode? nodeTo)
    {
        this.parentTrain = parentTrain;
        this.storingCollection = storingCollection;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.nodeTo = nodeTo?.Serialize() ?? "null";
    }
    public TrainNodeReParentHistoryEntry(DateTime timeStamp, AbstractTrainNode nodeFrom, AbstractTrainNode? nodeTo)
    {
        this.parentTrain = null;
        this.storingCollection = null;
        this.timeStamp = timeStamp;
        this.nodeFrom = nodeFrom.Serialize();
        this.nodeTo = nodeTo?.Serialize() ?? "null";
    }

    public string Serialize()
    {
        return $"ReLinkNode{SERIALIZATION_SEPARATOR}{timeStamp}{SERIALIZATION_SEPARATOR}{nodeFrom}{SERIALIZATION_SEPARATOR}{nodeTo}";
    }

    public void Execute()
    {
        NodeDeSerializer.DeSerialize(nodeFrom).ReParent(NodeDeSerializer.DeSerialize(nodeTo));
    }
}