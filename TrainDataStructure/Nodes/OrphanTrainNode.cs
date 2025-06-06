namespace TrainDataStructure.Nodes;
public abstract class OrphanTrainNode : AbstractTrainNode
{
    public override bool IsValueNode => true;
    public override bool IsUtilityNode => false;
    public sealed override bool IsOrphanNode => true;
    public override bool IsForking => false;


    protected sealed override List<AbstractTrainNode> HandleCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this];
    }
    protected sealed override List<AbstractTrainNode> HandleBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this];
    }
    protected sealed override List<AbstractTrainNode> HandleRawCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [];
    }
    protected sealed override List<AbstractTrainNode> HandleRawBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [];
    }

    public sealed override AbstractTrainNode? GetNext()
    {
        return null;
    }

    public sealed override AbstractTrainNode? GetPrevious()
    {
        return null;
    }

    public sealed override ITrainCollection? GetTrain()
    {
        return null;
    }

    public sealed override AbstractTrainNode? ReLink(AbstractTrainNode? node)
    {
        return null;
    }

    public sealed override AbstractTrainNode? ReParent(AbstractTrainNode? node)
    {
        return null;
    }

    public sealed override ITrainCollection? ReTrain(ITrainCollection? train)
    {
        return null;
    }

    protected sealed override TrainOperations HandleAddition(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        return TrainOperations.CUT_EARLY;
    }

    protected sealed override TrainOperations HandleInsertion(AbstractTrainNode node, int skipsRemainingIncludingCurrent, Stack<AbstractTrainNode> loopedOver)
    {
        return TrainOperations.CUT_EARLY;
    }

    protected sealed override TrainOperations HandleRemoval(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        return TrainOperations.CUT_EARLY;
    }

    protected sealed override TrainOperations HandleSignal(TrainSignal signal, Stack<AbstractTrainNode> loopedOver)
    {
        return TrainOperations.CUT_EARLY;
    }
}