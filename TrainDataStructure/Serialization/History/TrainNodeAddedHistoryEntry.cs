namespace TrainDataStructure.Serialization.History;
public class TrainNodeAddedHistoryEntry : ITrainHistoryEntry
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

    private readonly string node;

    public TrainNodeAddedHistoryEntry(ITrainCollection parentTrain, Queue<ITrainHistoryEntry> storingCollection, DateTime timeStamp, AbstractTrainNode node)
    {
        this.parentTrain = parentTrain;
        this.storingCollection = storingCollection;
        this.timeStamp = timeStamp;
        this.node = node.Serialize();
    }
    public TrainNodeAddedHistoryEntry(DateTime timeStamp, AbstractTrainNode node)
    {
        this.parentTrain = null;
        this.storingCollection = null;
        this.timeStamp = timeStamp;
        this.node = node.Serialize();
    }

    public string Serialize()
    {
        return $"AddNode{SERIALIZATION_SEPARATOR}{parentTrain?.GetID() ?? Int32.MaxValue}{SERIALIZATION_SEPARATOR}{timeStamp}{SERIALIZATION_SEPARATOR}{node}";
    }

    public void Execute()
    {
        parentTrain?.Add(NodeDeSerializer.DeSerialize(node));
    }
}