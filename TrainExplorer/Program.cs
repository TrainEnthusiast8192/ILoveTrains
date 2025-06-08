using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;


Train<int> train = new(40, 66, 0, 4);
var sorted = train.Contains(0);
Console.WriteLine(sorted.ToInt());

StandardTrainCacheView<int> cache = (StandardTrainCacheView<int>)train.GetCacheView()!;
cache.Deconstruct(out int cnt, out bool cntValid, out var Dic1, out var Dic2, out var Dic3);

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