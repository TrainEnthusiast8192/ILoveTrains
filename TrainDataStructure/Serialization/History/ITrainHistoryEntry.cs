namespace TrainDataStructure.Serialization.History;
public interface ITrainHistoryEntry
{
    public const char SERIALIZATION_SEPARATOR = '\u2000'; // Differs from AbstractTrainNode's separator
    public abstract ITrainCollection? GetTrain();
    public abstract void SetTrain(ITrainCollection? train);
    public abstract IEnumerable<ITrainHistoryEntry>? GetStoringCollection();
    public abstract void SetStoringCollection(IEnumerable<ITrainHistoryEntry>? collection);
    public abstract void Execute();
    public abstract DateTime GetAbsoluteTimeStamp();
    public abstract void SetAbsoluteTimeStamp(DateTime timeStamp);
    public abstract int CalculateMillisToDelay(DateTime creationTime);
    public abstract string Serialize();
}