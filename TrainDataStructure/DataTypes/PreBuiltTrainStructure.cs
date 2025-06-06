namespace TrainDataStructure.DataTypes;
public record class PreBuiltTrainStructure
{
    public AbstractTrainNode first;

    public bool IsLinear;
    public bool IsTypeSafe;
    public int Count;

    public PreBuiltTrainStructure(params AbstractTrainNode[] nodes)
    {
        OnConnectionSuccess = PreBuiltStructureEventsConstructor;
        OnConnectionFailure = PreBuiltStructureEventsConstructor;

        Count = 0;
        IsLinear = true;
        IsTypeSafe = true;
        Type? firstSeenType = null;

        if (nodes.Length == 0) { throw new ArgumentException("No nodes supplied to constructor of prebuilt"); }
        AbstractTrainNode firstInNodes = nodes[0];
        first = firstInNodes;
        firstInNodes.ReParent(null);

        int cnt = nodes.Length;
        for (int i = 0; i < cnt; i++)
        {
            AbstractTrainNode n = nodes[i];
            if (first.AddNode(n, new()).Is(TrainOperations.SUCCESS))
            {
                Count++;
                if (n.IsForking)
                {
                    IsLinear = false;
                }
                if (n.IsValueNode)
                {
                    ValueTrainNode<IComparable> vnode = (ValueTrainNode<IComparable>)n;
                    Type? T = vnode.GetValue()?.GetType();
                    firstSeenType ??= T;
                    IsTypeSafe &= Type.Equals(T, firstSeenType);
                }
            }
        }
    }
    public PreBuiltTrainStructure(PreBuiltTrainStructureEvents onConnectionSuccess, PreBuiltTrainStructureEvents onConnectionFailure, params AbstractTrainNode[] nodes)
    {
        OnConnectionSuccess = onConnectionSuccess;
        OnConnectionFailure = onConnectionFailure;

        Count = 0;
        IsLinear = true;
        IsTypeSafe = true;
        Type? firstSeenType = null;

        if (nodes.Length == 0) { throw new ArgumentException("No nodes supplied to constructor of prebuilt"); }
        AbstractTrainNode firstInNodes = nodes[0];
        first = firstInNodes;
        firstInNodes.ReParent(null);

        int cnt = nodes.Length;
        for (int i = 0; i < cnt; i++)
        {
            AbstractTrainNode n = nodes[i];
            if (first.AddNode(n, new()).Is(TrainOperations.SUCCESS))
            {
                Count++;
                if (n.IsForking)
                {
                    IsLinear = false;
                }
                if (n.IsValueNode)
                {
                    ValueTrainNode<IComparable> vnode = (ValueTrainNode<IComparable>)n;
                    Type? T = vnode.GetValue()?.GetType();
                    firstSeenType ??= T;
                    IsTypeSafe &= Type.Equals(T, firstSeenType);
                }
            }
        }
    }
    public PreBuiltTrainStructure(AbstractTrainNode first, int count)
    {
        this.first = first;
        Count = count;
        OnConnectionSuccess = PreBuiltStructureEventsConstructor;
        OnConnectionFailure = PreBuiltStructureEventsConstructor;
    }
    public PreBuiltTrainStructure(AbstractTrainNode first, int count, PreBuiltTrainStructureEvents onConnectionSuccess, PreBuiltTrainStructureEvents onConnectionFailure)
    {
        this.first = first;
        Count = count;
        OnConnectionSuccess = onConnectionSuccess;
        OnConnectionFailure = onConnectionFailure;
    }
    public PreBuiltTrainStructure(AbstractTrainNode first, int count, bool isLinear, bool isTypeSafe)
    {
        this.first = first;
        Count = count;
        OnConnectionSuccess = PreBuiltStructureEventsConstructor;
        OnConnectionFailure = PreBuiltStructureEventsConstructor;

        IsLinear = isLinear;
        IsTypeSafe = isTypeSafe;
    }
    public PreBuiltTrainStructure(AbstractTrainNode first, int count, bool isLinear, bool isTypeSafe, PreBuiltTrainStructureEvents onConnectionSuccess, PreBuiltTrainStructureEvents onConnectionFailure)
    {
        this.first = first;
        Count = count;
        OnConnectionSuccess = onConnectionSuccess;
        OnConnectionFailure = onConnectionFailure;

        IsLinear = isLinear;
        IsTypeSafe = isTypeSafe;
    }

    protected static bool PreBuiltStructureEventsConstructor(ITrainCollection? relevantTrain) => true;
    public delegate bool PreBuiltTrainStructureEvents(ITrainCollection? relevantTrain);
    public PreBuiltTrainStructureEvents OnConnectionSuccess;
    public PreBuiltTrainStructureEvents OnConnectionFailure;
}