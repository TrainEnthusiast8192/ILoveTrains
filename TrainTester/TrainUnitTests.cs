using System.Diagnostics;
using TUnit.Assertions;

namespace TrainTester;

public class TrainUnitTests
{
    [Test]
    public async Task TestAdd()
    {
        Train<int> train = new();

        var n1 = new ValueTrainNode<int>(9);
        var n2 = new ValueTrainNode<string>("str");
        var n3 = new SwitchTrainNode(536);

        await Assert.That(train.Add(n1)).IsEqualTo(true);
        await Assert.That(train.Add(n2)).IsEqualTo(true);

        n2.ReLink(n1);

        await Assert.That(train.Add(n3)).IsEqualTo(false);
    }
}