namespace TrainDataStructure.Nodes.Utility;
public class MarkerTrainNode : OrphanTrainNode
{
    protected string message;
    public MarkerTrainNode(string message) { this.message = message; }
    public MarkerTrainNode() { this.message = ""; }

    public static class Standards
    {
        //public static MarkerTrainNode EndOfBranch(int id, bool isLeft)
        //{
        //    return isLeft ?
        //            new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({id})") :
        //            new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({id})");
        //}
        //public static MarkerTrainNode EndOfBranch(SwitchTrainNode node)
        //{
        //    return node.ForkLeft ?
        //            new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({node.GetId()})") :
        //            new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({node.GetId()})");
        //}

        public static MarkerTrainNode LoopingStructure(AbstractTrainNode? finalMemberBeforeLoop)
        {
            return new MarkerTrainNode($"LOOP_START_AHEAD : ({finalMemberBeforeLoop?.ToString() ?? ""}) to ({finalMemberBeforeLoop?.GetNext()?.ToString() ?? ""})");
        }

        //public static MarkerTrainNode EndOfGraphBranch(int id, int branch)
        //{
        //    return new MarkerTrainNode(GraphTrainNode.BRANCH_END + $": {branch} ; ID({id})");
        //}
    }

    public override AbstractTrainNode Clone()
    {
        return new MarkerTrainNode(message);
    }

    public override int CompareTo(object? obj)
    {
        return obj is MarkerTrainNode n ? n.message.CompareTo(message) : 1;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override bool EquivalentTo(AbstractTrainNode? node)
    {
        return node is MarkerTrainNode n && n.message.Equals(message);
    }

    public override int GetHashCode()
    {
        return message.GetHashCode();
    }

    public override string ToString()
    {
        return "MARKER: " + message;
    }
}