using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class TrainLookupBenchmarks
{
    private Train<int> train = new Train<int>();
    private int[] values = new int[TrainSize];
    private AbstractTrainNode[] nodes = new AbstractTrainNode[TrainSize];
    private const int TrainSize = 1000;

    [GlobalSetup]
    public void Setup()
    {
        // Add random data
        Random r = new Random(DateTime.Now.Millisecond);
        for (int i = 0; i < TrainSize; i++)
        {
            int val = r.Next();
            while (values.Contains(val))
            {
                val = r.Next();
            }
            ValueTrainNode<int> node = new ValueTrainNode<int>(val);
            train.Add(node);
            values[i] = val;
            nodes[i] = node;
        }


        // Train the cache (pun not intended)
        for (int i = 0; i < TrainSize; i++)
        {
            _ = train.GetNodeAt(i);
        }
        foreach (var n in nodes)
        {
            _ = train.IndexOf(n);
        }
        for (int i = 0; i < TrainSize; i++)
        {
            _ = train.IndexOf(values[i]);
        }
        int dummy = 0;
        for (int i = 0; i < 10000; i++)
        {
            dummy += train.GetBranchLength();
        }

        train.cache = null;

        GC.Collect();
    }

    [Benchmark]
    public void LookupUsingGetNodeAt()
    {
        for (int i = 0; i < TrainSize; i++)
        {
            _ = train.GetNodeAt(i);
        }
    }

    [Benchmark]
    public void LookupUsingIndexOfNode()
    {
        foreach (var node in nodes)
        {
            _ = train.IndexOf(node);
        }
    }


    [Benchmark]
    public void LookupUsingIndexOfValue()
    {
        for (int i = 0; i < TrainSize; i++)
        {
            _ = train.IndexOf(values[i]);
        }
    }

    [Benchmark]
    public void RepeatedBranchLength()
    {
        int dummy = 0;
        for (int i = 0; i < 10000; i++)
        {
            dummy += train.GetBranchLength();
        }
    }
}