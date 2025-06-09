namespace TrainDataStructure.Nodes.Standard;
public class WaitTrainNode : AbstractTrainNode
{
    protected AbstractTrainNode? next;
    protected AbstractTrainNode? previous;
    protected ITrainCollection? train;

    protected readonly int millis;

    public override bool IsValueNode => false;
    public override Type? GetStoredTypeOrDefault() => default;
    public override bool IsUtilityNode => true;
    public override bool IsOrphanNode => false;
    public override bool IsForking => false;

    public WaitTrainNode()
    {
        this.millis = 0;
        next = null;
        previous = null;
        train = null;
    }
    public WaitTrainNode(int millis)
    {
        this.millis = millis;
        next = null;
        previous = null;
        train = null;
    }
    

    public override int CompareTo(object? obj)
    {
        return obj is WaitTrainNode n ? n.millis.CompareTo(millis) : 1;
    }

    public override bool EquivalentTo(AbstractTrainNode? node)
    {
        return node is WaitTrainNode n && n.millis.Equals(millis);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override string ToString()
    {
        return $"WaitTrainNode : {millis}";
    }
    public override string Serialize()
    {
        return $"WaitTrainNode{SERIALIZATION_SEPARATOR}{millis}{SERIALIZATION_SEPARATOR}{INTERNAL_CONNECTIONS_GUID}";
    }

    public override int GetHashCode()
    {
        return millis;
    }

    public override AbstractTrainNode Clone()
    {
        return new WaitTrainNode(millis);
    }

    public override AbstractTrainNode? GetNext() => next;
    public override AbstractTrainNode? GetPrevious() => previous;
    public override ITrainCollection? GetTrain() => train;

    public override AbstractTrainNode? ReLink(AbstractTrainNode? node)
    {
        AbstractTrainNode? ret = next;
        next = node;
        return ret;
    }

    public override AbstractTrainNode? ReParent(AbstractTrainNode? node)
    {
        AbstractTrainNode? ret = previous;
        previous = node;
        return ret;
    }

    public override ITrainCollection? ReTrain(ITrainCollection? train)
    {
        ITrainCollection? ret = this.train;
        this.train = train;
        return ret;
    }

    protected override List<AbstractTrainNode> HandleCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.Collapse(loopedOver) ?? []];
    }

    protected override List<AbstractTrainNode> HandleBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.BranchCollapse(loopedOver) ?? []];
    }

    protected override List<AbstractTrainNode> HandleRawCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.RawCollapse(loopedOver) ?? []];
    }

    protected override List<AbstractTrainNode> HandleRawBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.RawBranchCollapse(loopedOver) ?? []];
    }


    protected override TrainOperations HandleAddition(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return TrainOperations.LOOPED;
        }

        if (next is null)
        {
            next = node;
            node.ReParent(this);
            node.ReTrain(train);

            return TrainOperations.SUCCESS;
        }

        return TrainOperations.PASS;
    }

    protected override TrainOperations HandleInsertion(AbstractTrainNode node, int skipsRemainingIncludingCurrent, Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return TrainOperations.LOOPED;
        }

        if (skipsRemainingIncludingCurrent != 1)
        {
            return TrainOperations.PASS;
        }

        if (next is null)
        {
            next = node;
            node.ReParent(this);
            node.ReTrain(train);

            return TrainOperations.SUCCESS;
        }
        else
        {
            next.ReParent(node);

            node.ReParent(this);
            node.ReLink(next);
            node.ReTrain(train);

            next = node;
        }

        return TrainOperations.PASS;
    }

    protected override TrainOperations HandleRemoval(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return TrainOperations.LOOPED;
        }

        if (node.Equals(this))
        {
            previous?.ReLink(next);
            next?.ReParent(previous);
            previous = null;
            next = null;

            train = null;

            return TrainOperations.SUCCESS;
        }

        return TrainOperations.PASS;
    }

    protected override TrainOperations HandleSignal(TrainSignal signal, Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return TrainOperations.LOOPED;
        }
        if (signal.IsCancelled)
        {
            return TrainOperations.CANCELLED;
        }
        
        if (signal.IsAwaitable)
        {
            Task.Run(async () =>
            {
                await Task.Delay(millis);
                GetNext()?.Signal(signal, []);
            });

            return TrainOperations.CUT_EARLY;
        }

        return TrainOperations.PASS;
    }
}