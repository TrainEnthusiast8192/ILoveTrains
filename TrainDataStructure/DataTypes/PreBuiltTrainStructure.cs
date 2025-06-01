namespace TrainDataStructure.DataTypes;
public class PreBuiltTrainStructure : OrphanTrainNode
{
    public AbstractTrainNode first;
    public int count;

    public bool hasFork;
    public bool hasExternalType;

    public ITrainCollection? train;

    public PreBuiltTrainStructure(AbstractTrainNode first, int count, bool hasFork, bool hasExternalType)
    {
        this.first = first;
        this.count = count;
        this.hasFork = hasFork;
        this.hasExternalType = hasExternalType;
    }

    public override PreBuiltTrainStructure Clone()
    {
        return new PreBuiltTrainStructure(first, count, hasFork, hasExternalType);
    }

    public override int CompareTo(object? obj)
    {
        return first.CompareTo(obj);
    }

    public override bool Equals(object? obj)
    {
        return first.Equals(obj);
    }

    public override bool EquivalentTo(AbstractTrainNode? node)
    {
        return first.EquivalentTo(node);
    }

    public override int GetHashCode()
    {
        return first.GetHashCode();
    }

    public override string ToString()
    {
        return String.Concat(first.Collapse(new HashSet<AbstractTrainNode>()).Select(o => o.ToString()));
    }
}