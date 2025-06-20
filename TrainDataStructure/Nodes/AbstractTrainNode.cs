﻿namespace TrainDataStructure.Nodes;
public abstract class AbstractTrainNode : IComparable, ICloneable
{
    protected readonly Guid INTERNAL_CONNECTIONS_GUID = Guid.NewGuid();
    public const char SERIALIZATION_SEPARATOR = '\u2065'; // U+2065 is "Invisible operators - undefined", which seems to have no use
    protected static void NodeEventsConstructor() { }
    public delegate void NodeEvents();
    public NodeEvents OnAdded = NodeEventsConstructor;
    public NodeEvents OnRemoved = NodeEventsConstructor;
    public NodeEvents OnAddedAsFirst = NodeEventsConstructor;
    public NodeEvents OnInserted = NodeEventsConstructor;
    public NodeEvents OnReplaced = NodeEventsConstructor;

    public NodeEvents OnCollapse = NodeEventsConstructor;
    public NodeEvents OnRawCollapse = NodeEventsConstructor;
    public NodeEvents OnBranchCollapse = NodeEventsConstructor;
    public NodeEvents OnRawBranchCollapse = NodeEventsConstructor;

    public NodeEvents OnAnyCollapse = NodeEventsConstructor;
    public NodeEvents OnAnyFullCollapse = NodeEventsConstructor;
    public NodeEvents OnAnyRawCollapse = NodeEventsConstructor;
    public NodeEvents OnAnyMarkedCollapse = NodeEventsConstructor;
    public NodeEvents OnAnyBranchCollapse = NodeEventsConstructor;

    public abstract bool IsValueNode { get; }
    public abstract Type? GetStoredTypeOrDefault();
    public abstract bool IsUtilityNode { get; }
    public abstract bool IsOrphanNode { get; }
    public abstract bool IsForking { get; }

    public abstract int CompareTo(object? obj);
    public abstract bool EquivalentTo(AbstractTrainNode? node);
    public abstract override bool Equals(object? obj);
    public abstract override string ToString();
    public abstract string Serialize();
    public static AbstractTrainNode Parse(string serializedNode) => NodeDeSerializer.DeSerialize(serializedNode);
    public abstract override int GetHashCode();
    public abstract AbstractTrainNode Clone();
    object ICloneable.Clone() { return Clone(); }

    public abstract AbstractTrainNode? GetNext();
    public abstract AbstractTrainNode? GetPrevious();
    public abstract ITrainCollection? GetTrain();

    /// <summary>
    /// Re-connects this node to another one (or makes it end of branch).
    /// Implement without side effects and idempotently; it will be called often!
    /// </summary>
    /// <param name="node">Other node to connect to (or null for end of branch)</param>
    /// <returns>Previous link before starting operation</returns>
    protected abstract AbstractTrainNode? HandleReLink(AbstractTrainNode? node);
    public AbstractTrainNode? ReLink(AbstractTrainNode? node)
    {
        AbstractTrainNode? ret = HandleReLink(node);
        GetTrain()?.Log(new TrainNodeReLinkedHistoryEntry(DateTime.Now, this, node));
        return ret;
    }
    /// <summary>
    /// Re-parents this node to another one (or makes it non-reversable/first).
    /// Implement without side effects and idempotently; it will be called often!
    /// Nodes that have no explicit parent may still have nodes linked towards it. Keep it that way!
    /// </summary>
    /// <param name="node">Other node to come from (or null for no explicit parent)</param>
    /// <returns>Previous parent before starting operation</returns>
    protected abstract AbstractTrainNode? HandleReParent(AbstractTrainNode? node);
    public AbstractTrainNode? ReParent(AbstractTrainNode? node)
    {
        AbstractTrainNode? ret = HandleReParent(node);
        GetTrain()?.Log(new TrainNodeReParentHistoryEntry(DateTime.Now, this, node));
        return ret;
    }
    /// <summary>
    /// Replaces the node's connection to its parent train with another instance
    /// Implement without side effects and idempotently; it will be called often!
    /// </summary>
    /// <param name="train">Other train to connect to (or null for unconnected node)</param>
    /// <returns>Previous link before starting operation</returns>
    protected abstract ITrainCollection? HandleReTrain(ITrainCollection? train);
    public ITrainCollection? ReTrain(ITrainCollection? train)
    {
        ITrainCollection? ret = HandleReTrain(train);

        // Do both so moving, detaching and adding nodes always get logged
        // If both are null, this was a no-op, so we don't care about logging it
        ret?.Log(new TrainNodeReTrainedHistoryEntry(DateTime.Now, this, train));
        GetTrain()?.Log(new TrainNodeReTrainedHistoryEntry(DateTime.Now, this, train));
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleCollapse(Stack<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> Collapse(Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [MarkerTrainNode.Standards.LoopingStructure(FindFinalMemberBeforeLoop(loopedOver))];
        }

        loopedOver.Push(this);

        List<AbstractTrainNode> ret = HandleCollapse(loopedOver);

        loopedOver.Pop();

        OnAnyCollapse();
        OnCollapse();
        OnAnyFullCollapse();
        OnAnyMarkedCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleBranchCollapse(Stack<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> BranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [MarkerTrainNode.Standards.LoopingStructure(FindFinalMemberBeforeLoop(loopedOver))];
        }

        loopedOver.Push(this);

        List<AbstractTrainNode> ret = HandleBranchCollapse(loopedOver);

        loopedOver.Pop();

        OnAnyCollapse();
        OnBranchCollapse();
        OnAnyBranchCollapse();
        OnAnyMarkedCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleRawCollapse(Stack<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> RawCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [];
        }

        loopedOver.Push(this);

        List<AbstractTrainNode> ret = HandleRawCollapse(loopedOver);

        loopedOver.Pop();

        OnAnyCollapse();
        OnRawCollapse();
        OnAnyFullCollapse();
        OnAnyRawCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleRawBranchCollapse(Stack<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> RawBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [];
        }

        loopedOver.Push(this);

        List<AbstractTrainNode> ret = HandleBranchCollapse(loopedOver);

        loopedOver.Pop();

        OnAnyCollapse();
        OnRawBranchCollapse();
        OnAnyBranchCollapse();
        OnAnyRawCollapse();
        return ret;
    }


    protected abstract TrainOperations HandleAddition(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver);
    public TrainOperations AddNode(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleAddition(node, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Push(this);

        if (ret.Is(TrainOperations.PASS))
        {
            return GetNext()?.AddNode(node, loopedOver) ?? TrainOperations.PASS;
        }
        else if (ret.Is(TrainOperations.SUCCESS))
        {
            node.OnAdded();
        }

        return ret;
    }
    protected static AbstractTrainNode? FindFinalMemberBeforeLoop(Stack<AbstractTrainNode> loopedOver)
    {
        return loopedOver.Peek();
    }



    protected abstract TrainOperations HandleInsertion(AbstractTrainNode node, int skipsRemainingIncludingCurrent, Stack<AbstractTrainNode> loopedOver);
    public TrainOperations InsertNode(AbstractTrainNode node, int skipsRemaining, Stack<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleInsertion(node, skipsRemaining, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Push(this);

        if (ret.Is(TrainOperations.PASS))
        {
            return GetNext()?.InsertNode(node, skipsRemaining - 1, loopedOver) ?? TrainOperations.PASS;
        }
        if (ret.Is(TrainOperations.SUCCESS))
        {
            node.OnInserted();
        }

        return ret;
    }

    protected abstract TrainOperations HandleRemoval(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver);
    public TrainOperations RemoveNode(AbstractTrainNode node, Stack<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleRemoval(node, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Push(this);

        if (ret.Is(TrainOperations.PASS))
        {
            return GetNext()?.RemoveNode(node, loopedOver) ?? TrainOperations.PASS;
        }
        else if (ret.Is(TrainOperations.SUCCESS))
        {
            node.OnRemoved();
        }

        return ret;
    }

    protected abstract TrainOperations HandleSignal(TrainSignal signal, Stack<AbstractTrainNode> loopedOver);
    public TrainOperations Signal(TrainSignal signal, Stack<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleSignal(signal, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Push(this);

        if (ret.Is(TrainOperations.PASS))
        {
            return GetNext()?.Signal(signal, loopedOver) ?? TrainOperations.PASS;
        }
        else if (ret.Is(TrainOperations.SUCCESS))
        {
            signal.OnSignalSuccess(signal);
        }

        if (signal.IsTransitive)
        {
            GetNext()?.Signal(signal, loopedOver);
        }

        return ret;
    }
}