﻿namespace TrainDataStructure.Nodes;
public abstract class AbstractTrainNode : IComparable, ICloneable
{
    protected readonly Guid INTERNAL_ID = Guid.NewGuid();
    protected static void NodeEventsConstructor() { }
    public delegate void NodeEvents();
    public NodeEvents OnAdded = NodeEventsConstructor;
    public NodeEvents OnRemoved = NodeEventsConstructor;
    public NodeEvents OnAddedAsFirst = NodeEventsConstructor;
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
    public abstract bool IsUtilityNode { get; }
    public abstract bool IsOrphanNode { get; }
    public abstract bool IsForking { get; }

    public abstract int CompareTo(object? obj);
    public abstract bool EquivalentTo(AbstractTrainNode? node);
    public abstract override bool Equals(object? obj);
    public abstract override string ToString();
    public abstract override int GetHashCode();
    public abstract AbstractTrainNode Clone();
    object ICloneable.Clone() { return Clone(); }

    public abstract AbstractTrainNode? GetNext();
    public abstract AbstractTrainNode? GetPrevious();
    public abstract ITrainCollection? GetTrain();

    public abstract AbstractTrainNode? ReLink(AbstractTrainNode? node);
    public abstract AbstractTrainNode? ReParent(AbstractTrainNode? node);
    public abstract ITrainCollection? ReTrain(ITrainCollection? train);

    protected abstract List<AbstractTrainNode> HandleCollapse(HashSet<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> Collapse(HashSet<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [MarkerTrainNode.Standards.LoopingStructure(loopedOver.LastOrDefault())];
        }

        loopedOver.Add(this);

        List<AbstractTrainNode> ret = HandleCollapse(loopedOver);
        OnAnyCollapse();
        OnCollapse();
        OnAnyFullCollapse();
        OnAnyMarkedCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleBranchCollapse(HashSet<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> BranchCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [MarkerTrainNode.Standards.LoopingStructure(loopedOver.LastOrDefault())];
        }

        loopedOver.Add(this);

        List<AbstractTrainNode> ret = HandleBranchCollapse(loopedOver);
        OnAnyCollapse();
        OnBranchCollapse();
        OnAnyBranchCollapse();
        OnAnyMarkedCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleRawCollapse(HashSet<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> RawCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [];
        }

        loopedOver.Add(this);

        List<AbstractTrainNode> ret = HandleRawCollapse(loopedOver);
        OnAnyCollapse();
        OnRawCollapse();
        OnAnyFullCollapse();
        OnAnyRawCollapse();
        return ret;
    }

    protected abstract List<AbstractTrainNode> HandleRawBranchCollapse(HashSet<AbstractTrainNode> loopedOver);
    public List<AbstractTrainNode> RawBranchCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return [];
        }

        loopedOver.Add(this);

        List<AbstractTrainNode> ret = HandleBranchCollapse(loopedOver);
        OnAnyCollapse();
        OnRawBranchCollapse();
        OnAnyBranchCollapse();
        OnAnyRawCollapse();
        return ret;
    }


    protected abstract TrainOperations HandleAddition(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver);
    public TrainOperations AddNode(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleAddition(node, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Add(this);

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

    protected abstract TrainOperations HandleRemoval(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver);
    public TrainOperations RemoveNode(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleRemoval(node, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Add(this);

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

    protected abstract TrainOperations HandleSignal(TrainSignal signal, HashSet<AbstractTrainNode> loopedOver);
    public TrainOperations Signal(TrainSignal signal, HashSet<AbstractTrainNode> loopedOver)
    {
        TrainOperations ret = HandleSignal(signal, loopedOver);

        if (ret.Is(TrainOperations.CUT_EARLY) || ret.Is(TrainOperations.CANCELLED))
        {
            return ret;
        }

        loopedOver.Add(this);

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