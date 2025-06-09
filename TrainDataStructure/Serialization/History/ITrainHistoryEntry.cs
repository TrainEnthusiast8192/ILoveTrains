namespace TrainDataStructure.Serialization.History;
public interface ITrainHistoryEntry
{
    public const char SERIALIZATION_SEPARATOR = AbstractTrainNode.SERIALIZATION_SEPARATOR;
    public abstract ITrainCollection GetTrain();
    public abstract IEnumerable<ITrainHistoryEntry> GetStoringCollection();
    public abstract void Execute();
    public abstract DateTime GetAbsoluteTimeStamp();
    public abstract int CalculateMillisToDelay(DateTime creationTime);
    public abstract string Serialize();
}