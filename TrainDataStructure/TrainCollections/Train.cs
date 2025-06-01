namespace TrainDataStructure.TrainCollections;
public class Train<T> : TypedTrainCollection<T, IComparable>, IComparable, IEnumerable<AbstractTrainNode> where T : IComparable
{
    protected AbstractTrainNode? first;
    protected int count;
    protected readonly Guid GUID;
    protected readonly TrainCollectionCache cache;


    public override Guid GetID() => GUID;
    public override int Count => GetBranchLength();
    public override int GetTotalCount() => count;
    public override bool EnforcesTypeSafety => false;
    public override bool IsReadOnly => false;
    public override bool IsCached => true;

    public override bool Equals(object? obj) => obj is ITrainCollection t && t.GetID().Equals(GUID);
    public override int GetHashCode() => GUID.GetHashCode();

    public override AbstractTrainNode? GetFirst() => first;

    public Train() : base()
    {
        first = null;
        count = 0;
        GUID = Guid.NewGuid();
        cache = new TrainCollectionCache(this, true, true, true);
    }
    protected Train(Guid forceID) : base()
    {
        first = null;
        count = 0;
        GUID = forceID;
        cache = new TrainCollectionCache(this, true, true, true);
    }
    public Train(params AbstractTrainNode?[] initNodes) : base(initNodes)
    {
        GUID = Guid.NewGuid();
        cache = new TrainCollectionCache(this, true, true, true);
        foreach (AbstractTrainNode? n in initNodes)
        {
            if (n is not null) { Add(n); }
        }
    }
    public Train(params T?[] initValues) : base(initValues)
    {
        GUID = Guid.NewGuid();
        cache = new TrainCollectionCache(this, true, true, true);
        foreach (T? val in initValues)
        {
            Add(val);
        }
    }
    public Train(params object?[] initValuesAndNodes)
    {
        foreach (object? n in initValuesAndNodes)
        {
            if (n is T value) { Add(value); }
            else if (n is AbstractTrainNode node) { Add(node); }
            else if (n is IComparable comp) { AddExternal(comp); }
            else { throw new ArgumentException($"Invalid addition: Item {n} is not a valid value, node or comparable"); }
        }
    }

    public override IEnumerator<AbstractTrainNode> GetEnumerator()
    {
        AbstractTrainNode? curr = first;

        while (curr is not null)
        {
            yield return curr;
            curr = curr.GetNext();
        }

        yield break;
    }
    public override IEnumerator<AbstractTrainNode> GetEnumerator(params TrainSignal[] signalsToSendBeforeStartingEnumeration)
    {
        Signal(signalsToSendBeforeStartingEnumeration);

        AbstractTrainNode? curr = first;

        while (curr is not null)
        {
            yield return curr;
            curr = curr.GetNext();
        }

        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override Train<M> Cast<M>() where M : default
    {
        Train<M> ret = new Train<M>(GUID);
        ret.count = count;
        ret.first = first;
        return ret;
    }

    public Train<T> Clone() => new Train<T>(first);
    public int CompareTo(object? obj) => obj is Train<T> other ? other.GetFirst()?.CompareTo(GetFirst()) ?? 1 : 1;

    public override bool Add(T? value) => Add(new ValueTrainNode<T>(value));
    protected override bool HandleAddExternal<M>(M? value) where M : default 
    { 
        cache.IsTypeSafe &= typeof(M).Equals(typeof(T));
        return Add(new ValueTrainNode<M>(value));
    }
    public override bool Add(AbstractTrainNode node)
    {
        if (first is null)
        {
            first = node;
            first.ReParent(null);
            first.ReTrain(this);

            first.OnAddedAsFirst();
            first.OnAdded();

            count = 1;

            cache.AddedNodes.Enqueue(node);

            cache.IsLinear &= !node.IsForking;
            cache.IsTypeSafe &= !node.IsValueNode || node is ValueTrainNode<T>;

            return true;
        }

        bool ret = first.AddNode(node, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS);
        if (ret)
        {
            count++;
            cache.AddedNodes.Enqueue(node);

            cache.IsLinear &= !node.IsForking;
            cache.IsTypeSafe &= !node.IsValueNode ||node is ValueTrainNode<T>;
        }
        return ret;
    }

    public override List<AbstractTrainNode> BranchCollapse()
    {
        if (cache.IsPermanent && cache.IsLinear) { return cache.AddedNodes.ToList(); }
        return first?.BranchCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> Collapse()
    {
        if (cache.IsPermanent && cache.IsLinear) { return cache.AddedNodes.ToList(); }
        return first?.Collapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> RawBranchCollapse()
    {
        if (cache.IsPermanent && cache.IsLinear) { return cache.AddedNodes.ToList(); }
        return first?.RawBranchCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> RawCollapse()
    {
        if (cache.IsPermanent && cache.IsLinear) { return cache.AddedNodes.ToList(); }
        return first?.RawCollapse(new HashSet<AbstractTrainNode>()) ?? [];
    }

    public override int GetBranchLength()
    {
        if (cache.IsPermanent && cache.IsLinear) { return count; }
        int counter = 0;
        AbstractTrainNode? current = first;
        while (current is not null)
        {
            current = current.GetNext();
            counter++;
        }

        return counter;
    }

    public override AbstractTrainNode GetNodeAt(int index)
    {
        if (cache.IsLinear && cache.IsPermanent) 
        { 
            return cache.AddedNodes.ElementAtOrDefault(index) ?? throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the branch");
        }

        AbstractTrainNode? current = first;
        
        for (int i = 0; i < index; i++)
        {
            if (current is null) { throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the branch"); }
            current = current.GetNext();
        }

        if (current is null) { throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the branch"); }
        return current;
    }

    public override AbstractTrainNode GetNodeAt(Index index)
    {
        int brnch = GetBranchLength();
        int position = index.GetOffset(brnch);
        return GetNodeAt(position);
    }

    public override List<AbstractTrainNode> GetNodesAt(Range range)
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

    public override List<string> PrintBranch(bool printToConsole = true)
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

    public override List<string> PrintTrain(bool printToConsole = true)
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

    public override List<string> RawPrintBranch(bool printToConsole = true)
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

    public override List<string> RawPrintTrain(bool printToConsole = true)
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

    public override bool Remove(T? value)
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

    public override bool Remove(AbstractTrainNode node)
    {
        AbstractTrainNode? next = node.GetNext();
        bool ret = first?.RemoveNode(node, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
        if (ret)
        {
            if (node.Equals(first))
            {
                first = next;
            }
            count--;

            cache.IsPermanent = false;

            DeCache(node);
        }
        return ret;
    }
    protected bool DeCache(AbstractTrainNode node)
    {
        bool ret = false;
        int finalCacheCnt = cache.AddedNodes.Count;
        for (int i = 0; i < finalCacheCnt; i++)
        {
            AbstractTrainNode n = cache.AddedNodes.Dequeue();
            if (n.Equals(node))
            {
                i++;
                ret = true;
            }
            else
            {
                cache.AddedNodes.Enqueue(n);
            }
        }
        return ret;
    }

    public override bool RemoveAt(int index) => Remove(GetNodeAt(index));
    public override bool RemoveAt(Index index) => Remove(GetNodeAt(index));

    public override bool Clear()
    {
        List<AbstractTrainNode> nodes = RawBranchCollapse();
        bool ret = false;
        for (int i = nodes.Count - 1; i > -1; i--)
        {
            ret |= Remove(nodes[i]);
        }
        cache.IsPermanent |= ret;
        cache.IsLinear |= ret;
        cache.IsTypeSafe |= ret;

        return ret;
    }

    public override bool Insert(int index, T? value)
    {
        AbstractTrainNode prev = GetNodeAt(index - 1);

        AbstractTrainNode? next = prev.ReLink(null);

        ValueTrainNode<T> newNode = new ValueTrainNode<T>(value);

        bool ret = prev.AddNode(newNode, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS);

        if (ret)
        {
            next?.ReParent(newNode);

            cache.AddedNodes.Enqueue(newNode);
            cache.IsLinear &= !newNode.IsForking;
        }
        else
        {
            prev.ReLink(next);
        }

        return ret;
    }

    public override bool Insert(Index index, T? value) => Insert(index.GetOffset(GetBranchLength()) - 1, value);

    public override bool Insert(Range range, params T?[] value)
    {
        int cnt = GetBranchLength();
        int start = range.Start.GetOffset(cnt);
        int end = range.End.GetOffset(cnt);

        int arrCnt = 0;
        bool ret = false;
        for (int i = start; i < end; i++)
        {
            ret |= Insert(i, value[arrCnt]);
            arrCnt++;
        }

        return ret;
    }

    public override bool Insert(int index, AbstractTrainNode value)
    {
        AbstractTrainNode prev = GetNodeAt(index - 1);
        AbstractTrainNode? next = prev.ReLink(null);
        bool ret = prev.AddNode(value, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS);
        if (ret)
        {
            next?.ReParent(value);
            cache.AddedNodes.Enqueue(value);
            cache.IsLinear &= !value.IsForking;
            cache.IsTypeSafe &= !value.IsValueNode || value is ValueTrainNode<T>;
        }
        else
        {
            prev.ReLink(next);
        }

        return ret;
    }

    public override bool Insert(Index index, AbstractTrainNode value) => Insert(index.GetOffset(GetBranchLength()) - 1, value);

    public override bool Insert(Range range, AbstractTrainNode[] value)
    {
        int cnt = GetBranchLength();
        int start = range.Start.GetOffset(cnt);
        int end = range.End.GetOffset(cnt);

        int arrCnt = 0;
        bool ret = false;
        for (int i = start; i < end; i++)
        {
            ret |= Insert(i, value[arrCnt]);
            arrCnt++;
        }

        return ret;
    }

    public override bool Contains(AbstractTrainNode item)
    {
        if (cache.IsLinear && cache.IsPermanent) { return cache.AddedNodes.Contains(item); }

        int cnt = GetBranchLength();
        bool ret = false;
        for (int i = 0; i < cnt; i++)
        {
            ret |= item.Equals(GetNodeAt(i));
        }
        return ret;
    }
    public override bool Contains(T? item)
    {
        int cnt = GetBranchLength();
        bool ret = false;
        ValueTrainNode<T> temp = new ValueTrainNode<T>(item);
        for (int i = 0; i < cnt; i++)
        {
            ret |= temp.EquivalentTo(GetNodeAt(i));
        }
        return ret;
    }
    public override bool Contains<M>(M? item) where M : default
    {
        if (cache.IsTypeSafe && cache.IsPermanent && cache.IsLinear && !typeof(M).Equals(typeof(T))) { return false; }

        int cnt = GetBranchLength();
        bool ret = false;
        ValueTrainNode<M> temp = new ValueTrainNode<M>(item);
        for (int i = 0; i < cnt; i++)
        {
            ret |= temp.EquivalentTo(GetNodeAt(i));
        }
        return ret;
    }

    public override int IndexOf(AbstractTrainNode item)
    {
        int cnt = GetBranchLength();
        int ret = -1;
        for (int i = 0; i < cnt; i++)
        {
            if (item.Equals(GetNodeAt(i)))
            {
                ret = i;
                break;
            }
        }
        return ret;
    }
    public override int IndexOf(T? item)
    {
        int cnt = GetBranchLength();
        int ret = -1;
        ValueTrainNode<T> temp = new ValueTrainNode<T>(item);
        for (int i = 0; i < cnt; i++)
        {
            if (temp.EquivalentTo(GetNodeAt(i)))
            {
                ret = i;
                break;
            }
        }
        return ret;
    }
    public override int IndexOf<M>(M? item) where M : default
    {
        if (cache.IsTypeSafe && cache.IsPermanent && cache.IsLinear && !typeof(M).Equals(typeof(T))) { return -1; }

        int cnt = GetBranchLength();
        int ret = -1;
        ValueTrainNode<M> temp = new ValueTrainNode<M>(item);
        for (int i = 0; i < cnt; i++)
        {
            if (temp.EquivalentTo(GetNodeAt(i)))
            {
                ret = i;
                break;
            }
        }
        return ret;
    }

    public override void CopyTo(AbstractTrainNode[] array, int arrayIndex)
    {
        List<AbstractTrainNode> branch = RawBranchCollapse();

        int cnt = array.Length;
        if (arrayIndex >= cnt || arrayIndex < 0) { throw new IndexOutOfRangeException($"Index {arrayIndex} was outside the bounds of the given array"); }
        if (cnt - arrayIndex < branch.Count) { throw new ArgumentException($"The number of elements in the source {branch.Count} is greater than the available space from {arrayIndex} to the end of the destination array."); }

        int listCounter = 0;
        for (int i = arrayIndex; i < cnt; i++)
        {
            array[i] = branch[listCounter];
            listCounter++;
        }
    }
    public override void CopyTo(AbstractTrainNode[] array) => CopyTo(array, 0);
    public override void CopyTo(AbstractTrainNode[] array, Index arrayIndex) => CopyTo(array, arrayIndex.GetOffset(GetBranchLength()));

    public override bool ReplaceAt(int index, T? newValue)
    {
        AbstractTrainNode? node = GetNodeAt(index);
        if (node is ValueTrainNode<T> vnode)
        {
            vnode.SetValue(newValue);
        }

        return false;
    }

    public override bool ReplaceAt(Index index, T? newValue)
    {
        int brnch = GetBranchLength();
        return ReplaceAt(index.GetOffset(brnch), newValue);
    }

    public override bool ReplaceRange(Range range, params T?[] newValues)
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

    public override bool Signal(TrainSignal signal)
    {
        return first?.Signal(signal, new HashSet<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
    }

    public override bool Signal(params TrainSignal[] signals)
    {
        bool ret = false;
        foreach (TrainSignal s in signals)
        {
            ret |= Signal(s);
        }
        return ret;
    }

    public bool AddStructure(PreBuiltTrainStructure structure)
    {
        cache.AddedNodes.Enqueue(structure); // Enqueue the structure to mark the next "structure.count" nodes to be from the structure
        bool ret = Add(structure.first); // Add the node to the end
        if (ret)
        {
            count += structure.count - 1; // Add the amount of nodes, minus the one we already added

            // Add each node to the cache
            structure.first.RawCollapse(new HashSet<AbstractTrainNode>()).ForEach(o => cache.AddedNodes.Enqueue(o));

            // Update cache
            cache.IsLinear &= !structure.hasFork;
            cache.IsTypeSafe &= !structure.hasExternalType;

            // Update structure metadata
            structure.train = this;
        }
        else
        {
            cache.AddedNodes.Dequeue(); // Remove the structure if adding of the first node failed
        }

        return ret;
    }

    public override T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public override T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public override List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValues(range, []); set => _setValues(value, range, []); }
    public override AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public override AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public override List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodesAt(range); set => _setNodes(value, range, []); }
    public override T? this[int index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public override T? this[Index index, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public override List<T?> this[Range range, IndexerInference.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValues(range, signalsToSendBeforeAccess); set => _setValues(value, range, signalsToSendBeforeAccess); }
    public override AbstractTrainNode? this[int index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public override AbstractTrainNode? this[Index index, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public override List<AbstractTrainNode> this[Range range, IndexerInference.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNodes(range, signalsBeforeAccess); set => _setNodes(value, range, signalsBeforeAccess); }
    public override T? this[int index] { get => this[index, IndexerInference.DIRECT]; set => this[index, IndexerInference.DIRECT] = value; }
    public override T? this[Index index] { get => this[index, IndexerInference.DIRECT]; set => this[index, IndexerInference.DIRECT] = value; }
    public override List<T?> this[Range range] { get => this[range, IndexerInference.DIRECT]; set => this[range, IndexerInference.DIRECT] = value; }
    public override T? this[int index, params TrainSignal[] signalsToSendBeforeAccess] { get => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }
    public override T? this[Index index, params TrainSignal[] signalsToSendBeforeAccess] { get => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[index, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }
    public override List<T?> this[Range range, params TrainSignal[] signalsToSendBeforeAccess] { get => this[range, IndexerInference.DIRECT, signalsToSendBeforeAccess]; set => this[range, IndexerInference.DIRECT, signalsToSendBeforeAccess] = value; }

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