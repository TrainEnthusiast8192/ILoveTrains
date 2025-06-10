SerializablePredicate<string> s = new(o => o.Length < 5);
var node = new ValueTrainNode<SerializablePredicate<string>>(s);
Console.WriteLine(node.Serialize());

public class TestNode : WaitTrainNode
{
    public TestNode() : base(100) { }
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

        if (signal.IsAwaitable)
        {
            Task.Run(async () =>
            {
                await Task.Delay(millis);
                GetNext()?.Signal(signal, []);
            });

            return TrainOperations.SUCCESS;
        }

        return TrainOperations.PASS;
    }
}