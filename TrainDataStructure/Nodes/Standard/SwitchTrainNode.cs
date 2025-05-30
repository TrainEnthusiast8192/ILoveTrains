namespace TrainDataStructure.Nodes.Standard;
public class SwitchTrainNode : AbstractTrainNode, IIdentifiableTrainNode, ITrainSignalReceiver
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

    public override bool IsUtilityNode => true;

    public override bool IsOrphanNode => false;

    public override bool IsForking => true;

    public SwitchTrainNode()
    {
        this.id = IIdentifiableTrainNode.GetNewID();
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
    }
    public SwitchTrainNode(AbstractTrainNode? previous, AbstractTrainNode? next, ITrainCollection? train)
    {
        this.id = IIdentifiableTrainNode.GetNewID();
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

    protected override List<AbstractTrainNode> HandleCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        List<AbstractTrainNode> ret = [this,
                        ..left?.Collapse(loopedOver) ?? [],
                        MarkerTrainNode.Standards.EndOfBranch(id, true),
                        ..next?.Collapse(loopedOver) ?? [],
                        MarkerTrainNode.Standards.EndOfFork(id, false)
                        ];

        return ret;
    }

    protected override List<AbstractTrainNode> HandleBranchCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.RawBranchCollapse(loopedOver) ?? [], MarkerTrainNode.Standards.EndOfFork(this)];
    }

    protected override List<AbstractTrainNode> HandleRawCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        List<AbstractTrainNode> ret = [this,
                        ..left?.Collapse(loopedOver) ?? [],
                        ..next?.Collapse(loopedOver) ?? [],
                        ];

        return ret;
    }

    protected override List<AbstractTrainNode> HandleRawBranchCollapse(HashSet<AbstractTrainNode> loopedOver)
    {
        return [this, .. GetNext()?.RawBranchCollapse(loopedOver) ?? []];
    }

    protected override TrainOperations HandleAddition(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver)
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

    protected override TrainOperations HandleRemoval(AbstractTrainNode node, HashSet<AbstractTrainNode> loopedOver)
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

    protected override TrainOperations HandleSignal(TrainSignal signal, HashSet<AbstractTrainNode> loopedOver)
    {
        if (loopedOver.Contains(this))
        {
            return TrainOperations.LOOPED;
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