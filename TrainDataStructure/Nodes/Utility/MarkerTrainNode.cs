
namespace TrainDataStructure.Nodes.Utility;
public class MarkerTrainNode : OrphanTrainNode
{
    protected string message;
    protected enum SpecialMarkers { none, branch_end, fork_end, loop };
    protected readonly SpecialMarkers markerType = SpecialMarkers.none;
    public bool IsSpecialMarker => markerType != SpecialMarkers.none;
    public bool IsEndOfBranch => markerType == SpecialMarkers.branch_end;
    public bool IsEndOfFork => markerType == SpecialMarkers.fork_end;
    public bool IsStartOfLoop => markerType == SpecialMarkers.loop;
    public MarkerTrainNode(string message) { this.message = message; }
    public MarkerTrainNode() { this.message = ""; }
    protected MarkerTrainNode(string message, SpecialMarkers markerType) { this.message = message; this.markerType = markerType; }

    public static class Standards
    {
        public static MarkerTrainNode EndOfBranch(int id, bool isLeft)
        {
            return isLeft ?
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({id})", SpecialMarkers.branch_end) :
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({id})", SpecialMarkers.branch_end);
        }
        public static MarkerTrainNode EndOfBranch(SwitchTrainNode node)
        {
            return node.ForkLeft ?
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({node.GetID()})", SpecialMarkers.branch_end) :
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({node.GetID()})", SpecialMarkers.branch_end);
        }
        public static MarkerTrainNode EndOfFork(int id, bool isLeft)
        {
            return isLeft ?
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({id})", SpecialMarkers.fork_end) :
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({id})", SpecialMarkers.fork_end);
        }
        public static MarkerTrainNode EndOfFork(SwitchTrainNode node)
        {
            return node.ForkLeft ?
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": LEFT ; ID({node.GetID()})", SpecialMarkers.fork_end) :
                    new MarkerTrainNode(SwitchTrainNode.BRANCH_END + $": RIGHT ; ID({node.GetID()})", SpecialMarkers.fork_end);
        }

        public static MarkerTrainNode LoopingStructure(AbstractTrainNode? finalMemberBeforeLoop)
        {
            return new MarkerTrainNode($"LOOP_START_AHEAD : ({finalMemberBeforeLoop?.ToString() ?? ""}) to ({finalMemberBeforeLoop?.GetNext()?.ToString() ?? ""})", SpecialMarkers.loop);
        }

        //public static MarkerTrainNode EndOfGraphBranch(int id, int branch)
        //{
        //    return new MarkerTrainNode(GraphTrainNode.BRANCH_END + $": {branch} ; ID({id})");
        //}
    }

    public override Type? GetStoredTypeOrDefault()
    {
        return default;
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