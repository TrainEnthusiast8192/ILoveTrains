namespace TrainDataStructure.Serialization.History;
public readonly struct TrainNodeAddedHistoryEntry(ITrainCollection parentTrain, Queue<ITrainHistoryEntry> storingCollection, DateTime timeStamp, AbstractTrainNode node) : ITrainHistoryEntry
{
    public const char SERIALIZATION_SEPARATOR = ITrainHistoryEntry.SERIALIZATION_SEPARATOR;
    private readonly ITrainCollection parentTrain = parentTrain;
    public readonly ITrainCollection GetTrain() => parentTrain;

    private readonly Queue<ITrainHistoryEntry> storingCollection = storingCollection;
    public readonly IEnumerable<ITrainHistoryEntry> GetStoringCollection() => storingCollection;

    private readonly DateTime timeStamp = timeStamp;
    public readonly DateTime GetAbsoluteTimeStamp() => timeStamp;
    public readonly int CalculateMillisToDelay(DateTime creationTime) => (timeStamp - creationTime).Milliseconds;

    private readonly string node = node.Serialize();

    public readonly string Serialize()
    {
        return $"AddNode{SERIALIZATION_SEPARATOR}{parentTrain.GetID()}{SERIALIZATION_SEPARATOR}{timeStamp}{SERIALIZATION_SEPARATOR}{node}";
    }

    public readonly void Execute()
    {
        parentTrain.Add(NodeDeSerializer.DeSerialize(node));
    }
}