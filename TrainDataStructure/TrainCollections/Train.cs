namespace TrainDataStructure.TrainCollections;
public class Train<T> : ITypedTrainCollection<T>, IComparable, ICloneable, IEnumerable<AbstractTrainNode> where T : IComparable
{
    protected AbstractTrainNode? first;
    protected int count;
    public bool EnforcesTypeSafety() => false;
    public int GetTotalCount() => count;
    public AbstractTrainNode? GetFirst() => first;

    public Train()
    {

    }
    public Train(params AbstractTrainNode?[] initNodes)
    {
        foreach (AbstractTrainNode? n in initNodes)
        {
            if (n is not null) { Add(n); }
        }
    }
    public Train(params T?[] initNodes)
    {
        foreach (T? val in initNodes)
        {
            Add(val);
        }
    }

    public IEnumerator<AbstractTrainNode> GetEnumerator()
    {
        AbstractTrainNode? curr = first;

        while (curr is not null)
        {
            yield return curr;
            curr = curr.GetNext();
        }

        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    object ICloneable.Clone()
    {
        return Clone();
    }
    public Train<T> Clone()
    {
        return new Train<T>(first);
    }
    public int CompareTo(object? obj)
    {
        return obj is Train<T> other ? other.GetFirst()?.CompareTo(GetFirst()) ?? 1 : 1;
    }

    public bool Add(T? value)
    {
        return Add(new ValueTrainNode<T>((T?)value));
    }

    public bool Add(AbstractTrainNode node)
    {
        if (first is null)
        {
            first = node;
            first.ReParent(null);
            first.ReTrain(this);

            first.OnAddedAsFirst();
            first.OnAdded();

            count = 1;

            return true;
        }

        bool ret = first.AddNode(node, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS);
        if (ret) { count++; }
        return ret;
    }

    public List<AbstractTrainNode> BranchCollapse()
    {
        return first?.BranchCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public List<AbstractTrainNode> Collapse()
    {
        return first?.Collapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public List<AbstractTrainNode> RawBranchCollapse()
    {
        return first?.RawBranchCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public List<AbstractTrainNode> RawCollapse()
    {
        return first?.RawCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }

    public int GetBranchLength()
    {
        int counter = 0;
        AbstractTrainNode? current = first;
        while (current is not null)
        {
            current = current.GetNext();
        }

        return counter;
    }

    public AbstractTrainNode GetNodeAt(int index)
    {
        AbstractTrainNode? current = first;
        
        for (int i = 0; i < index; i++)
        {
            if (current is null) { throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the branch"); }
            current = current.GetNext();
        }

        if (current is null) { throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the branch"); }
        return current;
    }

    public AbstractTrainNode GetNodeAt(Index index)
    {
        int brnch = GetBranchLength();
        int position = index.GetOffset(brnch);
        return GetNodeAt(position);
    }

    public List<AbstractTrainNode> GetNodesAt(Range range)
    {
        int brnch = GetBranchLength();
        int start = range.Start.GetOffset(brnch);
        int end = range.End.GetOffset(brnch);

        List<AbstractTrainNode> ret = new List<AbstractTrainNode>(end - start);

        for (int i = start; i < end; i++)
        {
            ret.Add(GetNodeAt(i));
        }

        return ret;
    }

    public List<string> PrintBranch(bool printToConsole = true)
    {
        List<string> ret = new List<string>();
        foreach (AbstractTrainNode n in BranchCollapse())
        {
            string ns = n.ToString();
            if (printToConsole) { Console.WriteLine(ns); }
            ret.Add(ns);
        }

        return ret;
    }

    public List<string> PrintTrain(bool printToConsole = true)
    {
        List<string> ret = new List<string>();
        foreach (AbstractTrainNode n in Collapse())
        {
            string ns = n.ToString();
            if (printToConsole) { Console.WriteLine(ns); }
            ret.Add(ns);
        }

        return ret;
    }
    public List<string> PrintTrainWithIndent(bool printToConsole = true, int indentSpaceAmount = 4)
    {
        List<string> ret = new List<string>();
        int currentIndentation = 0;

        foreach (AbstractTrainNode n in Collapse())
        {
            bool IsFinalMarker = n is MarkerTrainNode m && m.IsEndOfFork;
            bool IsBranchMarker = n is MarkerTrainNode l && l.IsEndOfBranch;

            if (IsBranchMarker) { currentIndentation--; }
            if (IsFinalMarker) { currentIndentation--; }

            string indent = String.Concat(Enumerable.Repeat(" ", currentIndentation * indentSpaceAmount));
            string retS = indent + n.ToString();
            if (printToConsole) { Console.WriteLine(retS); }
            ret.Add(retS);

            if (IsBranchMarker) { currentIndentation++; }
            if (n.IsForking) { currentIndentation++; }
        }

        return ret;
    }

    public List<string> RawPrintBranch(bool printToConsole = true)
    {
        List<string> ret = new List<string>();
        foreach (AbstractTrainNode n in RawBranchCollapse())
        {
            string ns = n.ToString();
            if (printToConsole) { Console.WriteLine(ns); }
            ret.Add(ns);
        }

        return ret;
    }

    public List<string> RawPrintTrain(bool printToConsole = true)
    {
        List<string> ret = new List<string>();
        foreach (AbstractTrainNode n in RawCollapse())
        {
            string ns = n.ToString();
            if (printToConsole) { Console.WriteLine(ns); }
            ret.Add(ns);
        }

        return ret;
    }

    public bool Remove(T? value)
    {
        AbstractTrainNode? current = first;
        ValueTrainNode<T> compNode = new ValueTrainNode<T>(value);

        while (current is not null)
        {
            if (current.IsValueNode && current.EquivalentTo(compNode))
            {
                return Remove(current);
            }

            current = current.GetNext();
        }

        return false;
    }

    public bool Remove(AbstractTrainNode node)
    {
        AbstractTrainNode? next = node.GetNext();
        bool ret = first?.RemoveNode(node, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
        if (ret && node.Equals(first))
        {
            first = next;
        }
        return ret;
    }

    public bool ReplaceAt(int index, T? newValue)
    {
        AbstractTrainNode? node = GetNodeAt(index);
        if (node is ValueTrainNode<T> vnode)
        {
            vnode.SetValue(newValue);
        }

        return false;
    }

    public bool ReplaceAt(Index index, T? newValue)
    {
        int brnch = GetBranchLength();
        return ReplaceAt(index.GetOffset(brnch), newValue);
    }

    public bool ReplaceRange(Range range, params T?[] newValues)
    {
        int brnch = GetBranchLength();
        int start = range.Start.GetOffset(brnch);
        int end = range.End.GetOffset(brnch);

        int arrcnt = 0;
        bool ret = false;
        for (int i = start; i < end; i++)
        {
            ret |= ReplaceAt(i, newValues[arrcnt]);
            arrcnt++;
        }

        return ret;
    }

    public bool Signal(TrainSignal signal)
    {
        return first?.Signal(signal, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
    }

    public bool Signal(params TrainSignal[] signals)
    {
        bool ret = false;
        foreach (TrainSignal s in signals)
        {
            ret |= Signal(s);
        }
        return ret;
    }

    public T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValues(range, []); set => _setValues(value, range, []); }
    public AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodesAt(range); set => _setNodes(value, range, []); }
    public T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValues(range, signalsToSendBeforeAccess); set => _setValues(value, range, signalsToSendBeforeAccess); }
    public AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNodes(range, signalsBeforeAccess); set => _setNodes(value, range, signalsBeforeAccess); }
    public T? this[int index] { get => this[index, IndexerInference.DIRECT]; set => this[index, IndexerInference.DIRECT] = value; }
    public T? this[Index index] { get => this[index, IndexerInference.DIRECT]; set => this[index, IndexerInference.DIRECT] = value; }
    public List<T?> this[Range range] { get => this[range, IndexerInference.DIRECT]; set => this[range, IndexerInference.DIRECT] = value; }
    public T? this[int index, params TrainSignal[] signalsToSendBeforeAccess] { get => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }
    public T? this[Index index, params TrainSignal[] signalsToSendBeforeAccess] { get => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }
    public List<T?> this[Range range, params TrainSignal[] signalsToSendBeforeAccess] { get => this[range, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[range, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }

    protected AbstractTrainNode? _getNode(int index, TrainSignal[] signals)
    {
        Signal(signals);
        return GetNodeAt(index);
    }
    protected AbstractTrainNode? _getNode(Index index, TrainSignal[] signals)
    {
        Signal(signals);
        return GetNodeAt(index);
    }
    protected List<AbstractTrainNode> _getNodes(Range range, TrainSignal[] signals)
    {
        Signal(signals);
        return GetNodesAt(range);
    }

    protected void _setNode(AbstractTrainNode? value, int index, params TrainSignal[] signals)
    {
        Signal(signals);
        AbstractTrainNode node = GetNodeAt(index);
        node.GetPrevious()?.ReLink(value);
        node.ReParent(value);
        value?.ReTrain(this);
    }
    protected void _setNode(AbstractTrainNode? value, Index index, params TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        _setNode(value, index.GetOffset(brnch), []);
    }
    protected void _setNodes(List<AbstractTrainNode> values, Range range, params TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        int start = range.Start.GetOffset(brnch);
        int end = range.End.GetOffset(brnch);

        int arrcnt = 0;
        for (int i = start; i < end; i++)
        {
            _setNode(values[arrcnt], i, []);
            arrcnt++;
        }
    }

    protected T? _getValue(int index, TrainSignal[] signals)
    {
        Signal(signals);
        AbstractTrainNode node = GetNodeAt(index);
        if (node.IsValueNode)
        {
            var vnode = (ValueTrainNode<T>)node;
            return vnode.GetValue();
        }

        return default;
    }
    protected T? _getValue(Index index, TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        return _getValue(index.GetOffset(brnch), []);
    }
    protected List<T?> _getValues(Range range, TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        int start = range.Start.GetOffset(brnch);
        int end = range.End.GetOffset(brnch);

        int arrcnt = 0;
        List<T?> ret = new List<T?>();
        for (int i = start; i < end; i++)
        {
            ret.Add(_getValue(i, []));
            arrcnt++;
        }

        return ret;
    }

    protected void _setValue(T? value, int index, params TrainSignal[] signals)
    {
        Signal(signals);
        AbstractTrainNode node = GetNodeAt(index);
        if (node.IsValueNode)
        {
            var vnode = (ValueTrainNode<T>)node;
            vnode.SetValue(value);
        }
    }
    protected void _setValue(T? value, Index index, params TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        _setValue(value, index.GetOffset(brnch), []);
    }
    protected void _setValues(List<T?> values, Range range, params TrainSignal[] signals)
    {
        Signal(signals);
        int brnch = GetBranchLength();
        int start = range.Start.GetOffset(brnch);
        int end = range.End.GetOffset(brnch);

        int arrcnt = 0;
        for (int i = start; i < end; i++)
        {
            _setValue(values[arrcnt], i, []);
            arrcnt++;
        }
    }
}