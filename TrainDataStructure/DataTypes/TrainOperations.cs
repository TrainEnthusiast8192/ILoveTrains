namespace TrainDataStructure.DataTypes;

public readonly record struct TrainOperations(TrainOperations.OPS Operation)
{
    public readonly bool Is(TrainOperations other) => Operation.Equals(other.Operation);

    public static readonly TrainOperations SUCCESS = new TrainOperations(OPS.SUCCESS);
    public static readonly TrainOperations FAILURE = new TrainOperations(OPS.FAILURE);
    public static readonly TrainOperations PASS = new TrainOperations(OPS.PASS);
    public static readonly TrainOperations CANCELLED = new TrainOperations(OPS.CANCELLED);
    public static readonly TrainOperations LOOPED = new TrainOperations(OPS.LOOPED);
    public static readonly TrainOperations CUT_EARLY = new TrainOperations(OPS.CUT_EARLY);

    public enum OPS
    {
        SUCCESS,
        FAILURE,
        PASS,
        CANCELLED,
        LOOPED,
        CUT_EARLY
    };
}