namespace TrainDataStructure.TrainCollections;
public interface ITrainCollection
{
    public abstract AbstractTrainNode? GetFirst();
    public abstract int GetTotalCount();
    public abstract int GetBranchLength();

    public abstract List<AbstractTrainNode> Collapse();
    public abstract List<AbstractTrainNode> BranchCollapse();
    public abstract List<AbstractTrainNode> RawCollapse();
    public abstract List<AbstractTrainNode> RawBranchCollapse();

    public abstract List<string> PrintTrain(bool printToConsole = true);
    public abstract List<string> PrintBranch(bool printToConsole = true);
    public abstract List<string> RawPrintTrain(bool printToConsole = true);
    public abstract List<string> RawPrintBranch(bool printToConsole = true);

    public abstract AbstractTrainNode? GetNodeAt(int index);
    public abstract AbstractTrainNode? GetNodeAt(Index index);

    public abstract List<AbstractTrainNode> GetNodesAt(Range range);

    public abstract bool Add(AbstractTrainNode node);
    public abstract bool Remove(AbstractTrainNode node);
    public abstract bool Signal(TrainSignal signal);
    public abstract bool Signal(params TrainSignal[] signals);

    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE]
    {
        get;
        set;
    }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE]
    {
        get;
        set;
    }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE]
    {
        get;
        set;
    }
    public abstract AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess]
    {
        get;
        set;
    }
    public abstract AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess]
    {
        get;
        set;
    }
    public abstract List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess]
    {
        get;
        set;
    }
}