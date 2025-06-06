namespace TrainDataStructure.Nodes.Standard;
public class ValueTrainNode<T> : AbstractTrainNode where T : IComparable
{
    public override bool IsValueNode => true;
    public override bool IsUtilityNode => false;
    public override bool IsForking => false;
    public override bool IsOrphanNode => false;

    protected T? value;
    public override Type GetStoredTypeOrDefault() => typeof(T);
    public T? GetValue()
    {
        return value;
    }
    public T? SetValue(T? newValue)
    {
        T? ret = value;
        value = newValue;
        return ret;
    }

    protected AbstractTrainNode? previous;
    protected AbstractTrainNode? next;
    protected ITrainCollection? train;

    public ValueTrainNode(T? value, AbstractTrainNode? previous, AbstractTrainNode? next, ITrainCollection? train)
    {
        this.value = value;
        this.previous = previous;
        this.next = next;
        this.train = train;
    }
    public ValueTrainNode(T? value)
    {
        this.value = value;
        this.previous = null;
        this.next = null;
        this.train = null;
    }
    public ValueTrainNode(AbstractTrainNode? previous, AbstractTrainNode? next, ITrainCollection? train)
    {
        this.value = default;
        this.previous = previous;
        this.next = next;
        this.train = train;
    }
    public ValueTrainNode()
    {
        this.value = default;
        this.previous = null;
        this.next = null;
        this.train = null;
    }

    public override ValueTrainNode<T> Clone()
    {
        if (value is ICloneable c && value is not AbstractTrainNode)
        {
            return new ValueTrainNode<T>((T)c.Clone(), previous, next, train);
        }

        return new ValueTrainNode<T>(value, previous, next, train);
    }

    public override int CompareTo(object? obj)
    {
        return obj is ValueTrainNode<T> vnode && vnode.value is not null ? vnode.value.CompareTo(value) : 1;
    }

    public override bool EquivalentTo(AbstractTrainNode? node)
    {
        return node is ValueTrainNode<T> vnode && EqualityComparer<T>.Default.Equals(value, vnode.value);
    }
    public override bool Equals(object? node)
    {
        return node is ValueTrainNode<T> vnode && EqualityComparer<T>.Default.Equals(value, vnode.value);
    }
    public static bool operator ==(ValueTrainNode<T> nodeA, ValueTrainNode<T> nodeB) => nodeA.value?.Equals(nodeB.value) ?? false;
    public static bool operator !=(ValueTrainNode<T> nodeA, ValueTrainNode<T> nodeB) => !(nodeA == nodeB);

    public override int GetHashCode()
    {
        return value?.GetHashCode() ?? 0;
    }

    public override AbstractTrainNode? GetNext()
    {
        return next;
    }

    public override AbstractTrainNode? GetPrevious()
    {
        return previous;
    }

    public override ITrainCollection? GetTrain()
    {
        return train;
    }

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

    public override string ToString()
    {
        return $"ValueTrainNode<{typeof(T)}> : {value}";
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

        return TrainOperations.PASS;
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
        return [this, ..GetNext()?.RawCollapse(loopedOver) ?? []];
    }

    protected override List<AbstractTrainNode> HandleRawBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.RawBranchCollapse(loopedOver) ?? []];
    }
}