namespace TrainDataStructure.DataTypes;
public record class PreBuiltTrainStructure(AbstractTrainNode first, bool IsLinear, bool IsTypeSafe, int Count)
{
    public AbstractTrainNode first = first;

    public bool IsLinear = IsLinear;
    public bool IsTypeSafe = IsTypeSafe;
    public int Count = Count;

    protected static bool PreBuiltStructureEventsConstructor(ITrainCollection? connectedTrain) => true;
    public delegate bool PreBuiltTrainStructureEvents(ITrainCollection? connectedTrain);
    public PreBuiltTrainStructureEvents OnConnectionSuccess = PreBuiltStructureEventsConstructor;
    public PreBuiltTrainStructureEvents OnConnectionFailure = PreBuiltStructureEventsConstructor;
}