namespace TrainDataStructure.Nodes.Standard;
public class SwitchTrainNode : AbstractTrainNode, IUniquelyIdentifiableTrainObject, ITrainSignalReceiver, IDisposable
{
    public const string BRANCH_END = "END_OF_BRANCH";

    public static string GetSignalTemplate() => SIGNAL_TEMPLATE;
    private const string SIGNAL_TEMPLATE = @"SWITCHTRAINNODE:(\d+):(left|right|toggle|pass)";
    public enum OPCODES { left, right, toggle, pass };

    public static string? FillTemplate(params object[] parameters)
    {
        if (parameters.Length != 2) { return null; }

        bool firstIsID = parameters[0] is int;
        bool secondIsOpcode = (parameters[1] is string opcode && Enum.TryParse<OPCODES>(opcode, out _)) || parameters[1] is OPCODES;
        if (firstIsID && secondIsOpcode)
        {
            return $"SWITCHTRAINNODE:{(int)parameters[0]}:{(string)parameters[1]}";
        }

        return null;
    }

    public int GetID() => id;
    protected int id;
    
    public bool ForkLeft = false;

    protected AbstractTrainNode? previous;
    protected AbstractTrainNode? next;
    protected AbstractTrainNode? left = null;
    protected ITrainCollection? train;

    public override bool IsValueNode => false;
    public override Type? GetStoredTypeOrDefault() => default;
    public override bool IsUtilityNode => true;
    public override bool IsOrphanNode => false;
    public override bool IsForking => true;

    public SwitchTrainNode()
    {
        this.id = IUniquelyIdentifiableTrainObject.GetNewID();
        this.previous = null;
        this.next = null;
        this.train = null;
    }
    public SwitchTrainNode(int forceID)
    {
        this.id = forceID;
        this.previous = null;
        this.next = null;
        this.train = null;
        IUniquelyIdentifiableTrainObject.AddForcedID(forceID);
    }
    public SwitchTrainNode(AbstractTrainNode? previous, AbstractTrainNode? next, ITrainCollection? train)
    {
        this.id = IUniquelyIdentifiableTrainObject.GetNewID();
        this.previous = previous;
        this.next = next;
        this.train = train;
    }
    public SwitchTrainNode(int forceID, AbstractTrainNode? previous, AbstractTrainNode? next, ITrainCollection? train)
    {
        this.id = forceID;
        this.previous = previous;
        this.next = next;
        this.train = train;
        IUniquelyIdentifiableTrainObject.AddForcedID(forceID);
    }
    ~SwitchTrainNode()
    {
        IUniquelyIdentifiableTrainObject.ReturnID(this);
    }
    public void Dispose()
    {
        IUniquelyIdentifiableTrainObject.ReturnID(this);
        GC.SuppressFinalize(this);
    }

    public override int CompareTo(object? obj)
    {
        return obj is SwitchTrainNode n ? n.id.CompareTo(id) : 1;
    }

    public override bool EquivalentTo(AbstractTrainNode? node)
    {
        return node is SwitchTrainNode n && n.id == id;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override string ToString()
    {
        return $"SwitchTrainNode ID({id}) : " + (ForkLeft ? "Pointing LEFT" : "Pointing RIGHT");
    }

    public override int GetHashCode()
    {
        return id;
    }

    public override SwitchTrainNode Clone()
    {
        return new SwitchTrainNode(id);
    }

    public override AbstractTrainNode? GetNext()
    {
        return ForkLeft ? left : next;
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
        if (ForkLeft)
        {
            AbstractTrainNode? ret = left;
            left = node;
            return ret;
        }
        else
        {
            AbstractTrainNode? ret = next;
            next = node;
            return ret;
        }
    }

    public override AbstractTrainNode? ReParent(AbstractTrainNode? node)
    {
        AbstractTrainNode? ret = previous;
        previous = node;
        return ret;
    }

    public override ITrainCollection? ReTrain(ITrainCollection? train)
    {
        ITrainCollection? ret = train;
        this.train = train;
        return ret;
    }

    protected override List<AbstractTrainNode> HandleCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        List<AbstractTrainNode> LeftCollapse = left?.Collapse(loopedOver) ?? [];
        List<AbstractTrainNode> RightCollapse = next?.Collapse(loopedOver) ?? [];

        List<AbstractTrainNode> ret = [this,
                        ..LeftCollapse,
                        MarkerTrainNode.Standards.EndOfBranch(id, true),
                        ..RightCollapse,
                        MarkerTrainNode.Standards.EndOfFork(id, false)
                        ];

        return ret;
    }

    protected override List<AbstractTrainNode> HandleBranchCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.BranchCollapse(loopedOver) ?? [], MarkerTrainNode.Standards.EndOfFork(this)];
    }

    protected override List<AbstractTrainNode> HandleRawCollapse(Stack<AbstractTrainNode> loopedOver)
    {
        List<AbstractTrainNode> ret = [this,
                        ..left?.RawCollapse(loopedOver) ?? [],
                        ..next?.RawCollapse(loopedOver) ?? [],
                        ];

        return ret;
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

        if (ForkLeft)
        {
            if (left is null)
            {
                left = node;
                node.ReParent(this);
                node.ReTrain(train);

                return TrainOperations.SUCCESS;
            }

            return TrainOperations.PASS;
        }
        else
        {
            if (next is null)
            {
                next = node;
                node.ReParent(this);
                node.ReTrain(train);

                return TrainOperations.SUCCESS;
            }

            return TrainOperations.PASS;
        }
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

        if (ForkLeft)
        {
            if (left is null)
            {
                left = node;
                node.ReParent(this);
                node.ReTrain(train);

                return TrainOperations.SUCCESS;
            }
            else
            {
                left.ReParent(node);

                node.ReParent(this);
                node.ReLink(next);
                node.ReTrain(train);

                left = node;
            }
        }
        else
        {
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
            previous?.ReLink(next ?? null);
            GetNext()?.ReParent(previous ?? null);
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

        string[] match = signal.FindCodeAndGetGroups(GetSignalTemplate());
        if (match.Length != 2) { return TrainOperations.PASS; }

        int incomingID = Int32.Parse(match[0]);
        if (incomingID != id) { return TrainOperations.PASS; }

        bool parsed = Enum.TryParse<OPCODES>(match[1], out OPCODES op);
        if (!parsed) { throw new ArgumentException($"Regex matched incorrect literal: {op} for signal: {signal}"); }

        switch (op)
        {
            default:
                throw new ArgumentException($"Regex matched incorrect literal: {op} for signal: {signal}");
            case OPCODES.pass:
                return TrainOperations.SUCCESS;
            case OPCODES.left:
                ForkLeft = true;
                return TrainOperations.SUCCESS;
            case OPCODES.right:
                ForkLeft = false;
                return TrainOperations.SUCCESS;
            case OPCODES.toggle:
                ForkLeft = !ForkLeft;
                return TrainOperations.SUCCESS;
        }
    }
}