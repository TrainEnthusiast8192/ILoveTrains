namespace TrainDataStructure.Nodes;

public interface IIdentifiableTrainNode
{
    public abstract long GetID();

    private static readonly List<long> TAKEN_IDS = new List<long>();
    private static readonly Random RAND_INST = new Random();

    public static bool IsIDTaken(long ID) => TAKEN_IDS.Contains(ID);
    public static long GetNewID()
    {
        long ret = RAND_INST.NextInt64();

        while (TAKEN_IDS.Contains(ret))
        {
            ret = RAND_INST.NextInt64();
        }

        TAKEN_IDS.Add(ret);
        return ret;
    }
}