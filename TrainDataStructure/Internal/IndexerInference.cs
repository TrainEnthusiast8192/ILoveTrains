namespace TrainDataStructure.Internal;
public static class IndexerInference
{
    public static readonly Enum[] Strategies = [DIRECT, NODE];
    public enum Direct { direct };
    public const Direct DIRECT = Direct.direct;
    public enum Node { node };
    public const Node NODE = Node.node;

    public static bool IsValid(Enum strategy) => Strategies.Contains(strategy);
}