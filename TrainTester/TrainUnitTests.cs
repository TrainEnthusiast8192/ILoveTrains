using System.Diagnostics;
using System.Text;
using TUnit.Assertions;

namespace TrainTester;

public class TrainUnitTests
{
    struct TestStruct1;
    struct TestStruct2 : IComparable { public int CompareTo(object? obj) { return 0; } }


    [Test]
    public async Task TestConstructors()
    {
        Train<int> t1 = [];
        await Assert.That(t1.Count).IsEqualTo(0);
        Train<int> t2 = new();
        await Assert.That(t2.Count).IsEqualTo(0);
        Train<int> t3 = new Train<int> { };
        await Assert.That(t3.Count).IsEqualTo(0);
        Train<int> t4 = 3298;
        await Assert.That(((ValueTrainNode<int>)t4.GetFirst()!).GetValue()).IsEqualTo(3298);

        PreBuiltTrainStructure structure = new PreBuiltTrainStructure(new ValueTrainNode<int>(4), new ValueTrainNode<int>(5));
        Train<int> t5 = new Train<int>(structure);

        await Assert.That(((ValueTrainNode<int>)t5.GetFirst()!).GetValue()).IsEqualTo(4);
    }

    [Test]
    public async Task TestAddNode()
    {
        Train<int> train = new();

        Assert.Throws(() => train.Add((AbstractTrainNode)null!));

        var n1 = new ValueTrainNode<int>(9);
        var n2 = new ValueTrainNode<string>("str");
        var n3 = new SwitchTrainNode(536);

        await Assert.That(train.Add(n1)).IsEqualTo(true);
        await Assert.That(train.Add(n2)).IsEqualTo(true);

        n2.ReLink(n1);

        await Assert.That(train.Add(n3)).IsEqualTo(false);

        n2.ReLink(null);

        await Assert.That(train.Add(n3)).IsEqualTo(true);
    }

    
    [Test]
    public async Task TestAddValue()
    {
        Train<string> train = new();

        await Assert.That(train.Add("ureghjklkj234978hgrsdfguop")).IsEqualTo(true);
        await Assert.That(train.Add((string)null!)).IsEqualTo(true); // Adds a node with null as its value

        await Assert.That(train.GetFirst()!.GetNext() is ValueTrainNode<string>).IsEqualTo(true);
    }


    [Test]
    public async Task TestAddExternalValue()
    {
        Train<string> train = new();

        await Assert.That(train.AddExternal(7L)).IsEqualTo(true);
        await Assert.That(train.AddExternal<IComparable>(new TestStruct2())).IsEqualTo(true);
        await Assert.That(train.AddExternal<byte>(99)).IsEqualTo(true);

        Assert.Throws(() => train.AddExternal((IComparable)((object)new TestStruct1())));
    }


    [Test]
    public async Task TestReplaceValue()
    {
        Train<int> train = new(1, 2, 3);

        await Assert.That(train.ReplaceAt(1, 0)).IsEqualTo(true);
        await Assert.That(((ValueTrainNode<int>)train.GetFirst()!.GetNext()!).GetValue()).IsEqualTo(0);

        await Assert.That(train.ReplaceAt(^1, 0)).IsEqualTo(true);
        await Assert.That(((ValueTrainNode<int>)train.GetFirst()!.GetNext()!.GetNext()!).GetValue()).IsEqualTo(0);

        await Assert.That(train.ReplaceRange(0..1, [0])).IsEqualTo(true);
        await Assert.That(((ValueTrainNode<int>)train.GetFirst()!).GetValue()).IsEqualTo(0);
    }
}