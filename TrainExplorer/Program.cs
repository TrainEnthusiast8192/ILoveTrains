using System.Collections;


Train<byte> train = new(new TestNode());
var s = new TrainSignal([], [], false, true);
s.OnSignalSuccess += gunc;
train[0, IndexerInference.NODE]!.ReLink(train[0, IndexerInference.NODE]);
train.Signal(s);

train.PrintTrain();

string? str = Console.ReadLine();

void gunc(TrainSignal s) { Console.WriteLine("SIG"); }


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