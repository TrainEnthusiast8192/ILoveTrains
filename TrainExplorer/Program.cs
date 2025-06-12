SerializablePredicate<TrainSignal> s = new(o => "CODE".Equals(o.FindCode("CODE")), ["TrainDataStructure.DataTypes"], [typeof(TrainSignal).Assembly]);
var node = new ValueTrainNode<SerializablePredicate<TrainSignal>>(s);
Console.WriteLine(((ValueTrainNode<SerializablePredicate<TrainSignal>>)NodeDeSerializer.DeSerialize(node.Serialize()))!.GetValue().AsPredicate(TrainSignal.Simple("CODE")));



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