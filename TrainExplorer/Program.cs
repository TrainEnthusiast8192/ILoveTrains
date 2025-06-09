using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;


ValueTrainNode<ValueTrainNode<int>> oldnode = new(new(330));
string serialized = oldnode.Serialize();

FieldInfo f = oldnode.GetType().GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.Instance | BindingFlags.NonPublic)!;

var newnode = (ValueTrainNode<ValueTrainNode<int>>)NodeDeserializer.DeSerialize(serialized);
var test = "ValueTrainNode\u20653b323e05-69b2-43af-af8f-d5eca20db3ee\u2065System.Int32, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\u2065330";
ValueTrainNode<int> testNode = (ValueTrainNode<int>)NodeDeserializer.DeSerialize(test);

Console.WriteLine("NEW NODE  " + newnode);
Console.WriteLine("OLD GUID   " + f.GetValue(oldnode));
Console.WriteLine("NEW GUID   " + f.GetValue(newnode));

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