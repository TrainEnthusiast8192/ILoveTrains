namespace TrainDataStructure.Internal;
public static class IndexerInference
{
    public static readonly List<Enum> ValidStrategies = [DIRECT, NODE];

    public const Strategies.Direct DIRECT = default;
    public const Strategies.Node NODE = default;
    public static class Strategies
    {
        public enum Direct;
        public enum Node;
    }

    public static bool IsValidStrategy(Enum strategy) => ValidStrategies.Contains(strategy);
}