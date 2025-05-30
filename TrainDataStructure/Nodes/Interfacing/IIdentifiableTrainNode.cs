namespace TrainDataStructure.Nodes.Interfacing;

public interface IIdentifiableTrainNode
{
    public abstract int GetID();

    private static readonly List<int> TAKEN_IDS = new List<int>();
    private static readonly Random RAND_INST = new Random();

    public static bool IsIDTaken(int ID) => TAKEN_IDS.Contains(ID);
    public static int GetNewID()
    {
        int ret = RAND_INST.Next();

        while (TAKEN_IDS.Contains(ret))
        {
            ret = RAND_INST.Next();
        }

        TAKEN_IDS.Add(ret);
        return ret;
    }
}