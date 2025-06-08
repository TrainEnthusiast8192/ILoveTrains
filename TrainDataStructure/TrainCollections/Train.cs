namespace TrainDataStructure.TrainCollections;
public class Train<T> : TypedTrainCollection<T, IComparable>, IComparable where T : IComparable
{
    protected AbstractTrainNode? first;
    protected int count;
    protected readonly int SUUID;

    public override int GetID() => SUUID;
    public override int Count => GetBranchLength();
    public override int GetTotalCount() => count;
    public override bool EnforcesTypeSafety => false;
    public override bool IsReadOnly => false;

    public override bool IsCached => cache is not null;
    public StandardTrainCache<T>? cache = new();
    public override ITrainCollectionCache? GetCacheView() => cache?.GetView() ?? null;

    public override bool Equals(object? obj) => obj is ITrainCollection t && t.GetID().Equals(SUUID);
    public override int GetHashCode() => SUUID;

    public override AbstractTrainNode? GetFirst() => first;

    // STANDARD CONSTRUCTORS
    /// <summary>
    /// Initializes a new Train object with no nodes and an auto-generated ID
    /// </summary>
    public Train() : base()
    {
        first = null;
        count = 0;
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
         
    }
    /// <summary>
    /// Initializes a new Train object with no nodes and a given ID
    /// </summary>
    /// <param name="forceID">Unique integer ID</param>
    protected Train(int forceID) : base()
    {
        first = null;
        count = 0;
        SUUID = forceID;
        IUniquelyIdentifiableTrainObject.AddForcedID(forceID);
         
    }

    // NODE CONSTRUCTORS
    /// <summary>
    /// Initializes a new Train object with the given nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initNodes">Collection of existing node instances</param>
    public Train(params AbstractTrainNode[] initNodes) : base(initNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (AbstractTrainNode n in initNodes)
        {
            Add(n);
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initNodes">Collection of existing node instances</param>
    public Train(IEnumerable<AbstractTrainNode> initNodes) : base(initNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (AbstractTrainNode n in initNodes)
        {
            Add(n);
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initNodes">Collection of existing node instances</param>
    public Train(Span<AbstractTrainNode> initNodes) : base(initNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (AbstractTrainNode val in initNodes)
        {
            Add(val);
        }
         
    }

    // VALUE CONSTRUCTORS
    /// <summary>
    /// Initializes a new Train object with the given values wrapped in value nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initValues">Collection of values</param>
    public Train(params T?[] initValues) : base(initValues)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (T? val in initValues)
        {
            Add(val);
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given values wrapped in value nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initValues">Collection of values</param>
    public Train(IEnumerable<T?> initValues) : base(initValues)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (T? val in initValues)
        {
            Add(val);
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given values wrapped in value nodes added in order and an auto-generated ID
    /// </summary>
    /// <param name="initValues">Collection of values</param>
    public Train(Span<T> initValues) : base(initValues)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (T val in initValues)
        {
            Add(val);
        }
         
    }

    // DYNAMIC CONSTRUCTORS
    /// <summary>
    /// Initializes a new Train object with the given objects, which may be existing nodes or values to be wrapped into new nodes
    /// </summary>
    /// <param name="initValuesAndNodes">Collection of values and nodes</param>
    public Train(params object?[] initValuesAndNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (object? n in initValuesAndNodes)
        {
            if (n is T value) { Add(value); }
            else if (n is AbstractTrainNode node) { Add(node); }
            else if (n is IComparable comp) { AddExternal(comp); }
            else { throw new ArgumentException($"Invalid addition: Item {n} is not a valid value, node or comparable"); }
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given objects, which may be existing nodes or values to be wrapped into new nodes
    /// </summary>
    /// <param name="initValuesAndNodes">Collection of values and nodes</param>
    public Train(IEnumerable<object?> initValuesAndNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (object? n in initValuesAndNodes)
        {
            if (n is T value) { Add(value); }
            else if (n is AbstractTrainNode node) { Add(node); }
            else if (n is IComparable comp) { AddExternal(comp); }
            else { throw new ArgumentException($"Invalid addition: Item {n} is not a valid value, node or comparable"); }
        }
         
    }
    /// <summary>
    /// Initializes a new Train object with the given objects, which may be existing nodes or values to be wrapped into new nodes
    /// </summary>
    /// <param name="initValuesAndNodes">Collection of values and nodes</param>
    public Train(Span<object?> initValuesAndNodes)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        foreach (object? n in initValuesAndNodes)
        {
            if (n is T value) { Add(value); }
            else if (n is AbstractTrainNode node) { Add(node); }
            else if (n is IComparable comp) { AddExternal(comp); }
            else { throw new ArgumentException($"Invalid addition: Item {n} is not a valid value, node or comparable"); }
        }
         
    }

    // OTHER CONSTRUCTORS
    /// <summary>
    /// Initializes a new Train object with the given structure using a pre-built substructure and an auto-generated ID
    /// </summary>
    /// <param name="initStructure">Structure to add</param>
    public Train(PreBuiltTrainStructure initStructure) : base(initStructure)
    {
        SUUID = IUniquelyIdentifiableTrainObject.GetNewID();
        Add(initStructure);
         
    }

    /// <summary>
    /// Initializes a new Train object with the given value wrapped in a value node and an auto-generated ID
    /// </summary>
    /// <param name="initValue">First value</param>
    public static implicit operator Train<T>(T? initValue) => new(initValues: initValue);
    /// <summary>
    /// Initializes a new Train object with the given pre-existing node and an auto-generated ID
    /// </summary>
    /// <param name="initNode">First node</param>
    public static implicit operator Train<T>(AbstractTrainNode initNode) => new(initNodes: initNode);
    /// <summary>
    /// Initializes a new Train object with the given pre-built structure and an auto-generated ID
    /// </summary>
    /// <param name="initStructure">Structure to add</param>
    public static implicit operator Train<T>(PreBuiltTrainStructure initStructure) => new(initStructure);

    /// <summary>
    /// Returns the Train's ID to the set of possible IDs
    /// </summary>
    ~Train()
    {
        IUniquelyIdentifiableTrainObject.ReturnID(this);
        cache = null;
    }
    public override void Dispose()
    {
        IUniquelyIdentifiableTrainObject.ReturnID(this);
        cache = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Lazily evaluates the nodes in the currently active branch
    /// </summary>
    /// <returns>Enumerator of nodes</returns>
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
    /// <summary>
    /// Sends signals and then lazily evaluates the currently active branch
    /// </summary>
    /// <param name="signalsToSendBeforeStartingEnumeration">Sent in the given order, all at the beginning</param>
    /// <returns>Enumerator of nodes</returns>
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

    /// <summary>
    /// Lazily evaluates the currently active branch as values, skipping utility nodes
    /// </summary>
    /// <returns>Enumerator of T? values</returns>
    public override IEnumerator<T?> GetValues()
    {
        AbstractTrainNode? curr = first;

        while (curr is not null)
        {
            if (curr is ValueTrainNode<T> vnode)
            {
                yield return vnode.GetValue();
            }
            curr = curr.GetNext();
        }

        yield break;
    }
    /// <summary>
    /// Sends signals and then lazily evaluates the currently active branch as values, skipping utility nodes
    /// </summary>
    /// <param name="signalsToSendBeforeStartingEnumeration">Sent in the given order, all at the beginning</param>
    /// <returns>Enumerator of T? values</returns>
    public override IEnumerator<T?> GetValues(params TrainSignal[] signalsToSendBeforeStartingEnumeration)
    {
        Signal(signalsToSendBeforeStartingEnumeration);

        AbstractTrainNode? curr = first;

        while (curr is not null)
        {
            if (curr is ValueTrainNode<T> vnode)
            {
                yield return vnode.GetValue();
            }
            curr = curr.GetNext();
        }

        yield break;
    }

    /// <summary>
    /// Creates a new view of the train under the new type
    /// Does not copy any nodes
    /// </summary>
    /// <typeparam name="M">New view type</typeparam>
    /// <returns>New Train object under type <typeparamref name="M"/> with the same underlying node structure </returns>
    public override Train<M> Cast<M>() where M : default
    {
        Train<M> ret = new Train<M>(SUUID);
        ret.count = count;
        ret.first = first;
        return ret;
    }

    /// <summary>
    /// Creates a shallow copy of the train pointing to the same head node
    /// </summary>
    /// <returns>Shallow clone of this Train</returns>
    public Train<T> Clone()
    {
        Train<T> ret = new Train<T>(SUUID);
        ret.count = count;
        ret.first = first;
        return ret;
    }
    /// <summary>
    /// Compares the first node of each Train
    /// </summary>
    /// <param name="obj">Non-Empty Train</param>
    /// <returns>1 if obj is not a Train or it is empty; else, comparison between first nodes</returns>
    public int CompareTo(object? obj) => obj is Train<T> other ? GetFirst()?.CompareTo(other.GetFirst()) ?? 1 : 1;

    /// <summary>
    /// Wraps the value in a value node and attempts to add it
    /// </summary>
    /// <param name="value">Value to add</param>
    /// <returns>Success / Failure</returns>
    public override bool Add(T? value) => Add(new ValueTrainNode<T>(value));
    /// <summary>
    /// Handles external addition called from the base abstract class member AddExternal of <typeparamref name="M"/>.
    /// Wraps the value in a value and attempts to add it
    /// </summary>
    /// <typeparam name="M">Type being added</typeparam>
    /// <param name="value">Value to be added</param>
    /// <returns>Succes / Failure</returns>
    protected override bool HandleAddExternal<M>(M? value) where M : default 
    {
        return Add(new ValueTrainNode<M>(value));
    }
    /// <summary>
    /// Adds a node at the end of the currently active branch.
    /// Activates OnAdded and OnAddedAsFirst (where appropriate)
    /// </summary>
    /// <param name="node">Pre-existing node to be added</param>
    /// <returns>Success / Failure</returns>
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

            if (cache is not null)
            {
                cache.IndexOf.Add(node, 0);
                cache.BranchCount = 1;
                cache.BranchCountCached = true;
            }

            return true;
        }

        bool ret = first.AddNode(node, new Stack<AbstractTrainNode>()).Is(TrainOperations.SUCCESS);
        if (ret)
        {
            count++;
            cache?.Invalidate();
        }
        return ret;
    }
    /// <summary>
    /// Adds the first node to the end of the Train, which drags previous connections forward
    /// </summary>
    /// <param name="structure">Structure to add</param>
    /// <returns>Success / Failure</returns>
    public override bool Add(PreBuiltTrainStructure structure)
    {
        bool ret = Add(structure.first);
        
        if (ret)
        {
            count += structure.Count - 1;
        }

        return ret;
    }

    public override List<AbstractTrainNode> BranchCollapse()
    {
        return first?.BranchCollapse(new Stack<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> Collapse()
    {
        return first?.Collapse(new Stack<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> RawBranchCollapse()
    {
        return first?.RawBranchCollapse(new Stack<AbstractTrainNode>()) ?? [];
    }
    public override List<AbstractTrainNode> RawCollapse()
    {
        return first?.RawCollapse(new Stack<AbstractTrainNode>()) ?? [];
    }

    public override int GetBranchLength()
    {
        if (cache?.BranchCountCached ?? false) { return cache.BranchCount; }
        int counter = 0;
        AbstractTrainNode? current = first;
        while (current is not null)
        {
            current = current.GetNext();
            counter++;
        }

        cache?.BranchCount = counter;
        cache?.BranchCountCached = true;

        return counter;
    }

    public override AbstractTrainNode GetNodeAt(int index)
    {
        if (cache?.GetNodeAt.ContainsKey(index) ?? false) { return cache.GetNodeAt[index]; }
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
        List<string> ret = new List<string>(GetBranchLength());
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
        List<string> ret = new List<string>(count);
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
        bool ret = first?.RemoveNode(node, new Stack<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
        if (ret)
        {
            if (node.Equals(first))
            {
                first = next;
            }
            count--;

            cache?.Invalidate();
        }
        return ret;
    }

    // Returns whether all removals succeeded
    public override bool Remove(params AbstractTrainNode[] nodes)
    {
        bool ret = true;
        
        foreach (AbstractTrainNode n in nodes)
        {
            ret &= Remove(n);
        }

        return ret;
    }

    public override bool RemoveAt(int index) => Remove(GetNodeAt(index));
    public override bool RemoveAt(Index index) => Remove(GetNodeAt(index));
    public override bool RemoveRange(Range range) => Remove(GetNodesAt(range).ToArray());

    public override bool Clear()
    {
        List<AbstractTrainNode> nodes = RawBranchCollapse();
        bool ret = true;
        for (int i = nodes.Count - 1; i > -1; i--)
        {
            ret &= Remove(nodes[i]);
        }

        return ret;
    }

    public override bool Insert(int index, T? value)
    {
        return Insert(index, new ValueTrainNode<T>(value));
    }

    public override bool Insert(Index index, T? value) => Insert(index.GetOffset(GetBranchLength()), value);

    public override bool Insert(Range range, params T?[] values)
    {
        int cnt = GetBranchLength();
        int start = range.Start.GetOffset(cnt);
        int end = range.End.GetOffset(cnt);

        int arrCnt = 0;
        bool ret = true;
        for (int i = start; i < end; i++)
        {
            ret &= Insert(i, values[arrCnt]);
            arrCnt++;
        }

        return ret;
    }

    public override bool Insert<M>(int index, M? value) where M : default => Insert(index, node: new ValueTrainNode<M>(value));
    public override bool Insert<M>(Index index, M? value) where M : default => Insert(index.GetOffset(GetBranchLength()), value);

    public override bool Insert<M>(Range range, params M?[] values) where M : default
    {
        int cnt = GetBranchLength();
        int start = range.Start.GetOffset(cnt);
        int end = range.End.GetOffset(cnt);

        int arrCnt = 0;
        bool ret = true;
        for (int i = start; i < end; i++)
        {
            ret &= Insert(i, values[arrCnt]);
            arrCnt++;
        }

        return ret;
    }

    public override bool Insert(int index, AbstractTrainNode node)
    {
        bool ret = index == 0 ? _insertAtHead(node) 
            : first?.InsertNode(node, index, new Stack<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) 
            ?? throw new IndexOutOfRangeException($"Index {index} was non-zero for insertion on empty train");

        if (ret)
        {
            cache?.Invalidate();
        }
        return ret;
    }
    protected virtual bool _insertAtHead(AbstractTrainNode node)
    {
        first?.ReParent(node);

        node.ReLink(first);
        node.ReParent(null);
        node.ReTrain(this);
        first = node;

        node.OnInserted();
        node.OnAddedAsFirst();

        return true;
    }

    public override bool Insert(Index index, AbstractTrainNode node) => Insert(index.GetOffset(GetBranchLength()), node);

    public override bool Insert(Range range, params AbstractTrainNode[] nodes)
    {
        int cnt = GetBranchLength();
        int start = range.Start.GetOffset(cnt);
        int end = range.End.GetOffset(cnt);

        int arrCnt = 0;
        bool ret = true;
        for (int i = start; i < end; i++)
        {
            ret &= Insert(i, nodes[arrCnt]);
            arrCnt++;
        }

        return ret;
    }

    public override bool Contains(AbstractTrainNode item)
    {
        if (cache?.IndexOf.ContainsKey(item) ?? false) { return true; }
        int cnt = GetBranchLength();
        bool ret = true;
        for (int i = 0; i < cnt; i++)
        {
            ret |= item.Equals(GetNodeAt(i));
        }
        return ret;
    }
    public override bool Contains(T? item)
    {
        if (item is not null && (cache?.IndexOfValue.ContainsKey(item) ?? false)) { return true; }
        int cnt = GetBranchLength();
        bool ret = true;
        ValueTrainNode<T> temp = new ValueTrainNode<T>(item);
        for (int i = 0; i < cnt; i++)
        {
            ret |= temp.EquivalentTo(GetNodeAt(i));
        }
        return ret;
    }
    public override bool Contains<M>(M? item) where M : default
    {
        int cnt = GetBranchLength();
        bool ret = true;
        ValueTrainNode<M> temp = new ValueTrainNode<M>(item);
        for (int i = 0; i < cnt; i++)
        {
            ret |= temp.EquivalentTo(GetNodeAt(i));
        }
        return ret;
    }

    public override int IndexOf(AbstractTrainNode item)
    {
        if (cache?.IndexOf.ContainsKey(item) ?? false) { return cache.IndexOf[item]; }
        int cnt = GetBranchLength();
        int ret = -1;
        for (int i = 0; i < cnt; i++)
        {
            if (item.Equals(GetNodeAt(i)))
            {
                ret = i;
                cache?.IndexOf.Add(item, i);
                break;
            }
        }
        return ret;
    }

    protected static readonly ValueTrainNode<T> INDEXOF_COMPARISON_DUMMY = new ValueTrainNode<T>();
    public override int IndexOf(T? item)
    {
        if (item is not null && (cache?.IndexOfValue.ContainsKey(item) ?? false)) { return cache.IndexOfValue[item]; }
        int cnt = GetBranchLength();
        int ret = -1;
        INDEXOF_COMPARISON_DUMMY.SetValue(item);

        for (int i = 0; i < cnt; i++)
        {
            if (INDEXOF_COMPARISON_DUMMY.EquivalentTo(GetNodeAt(i)))
            {
                ret = i;
                if (item is not null) { cache?.IndexOfValue.Add(item, i); }
                break;
            }
        }
        return ret;
    }
    public override int IndexOf<M>(M? item) where M : default
    {
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
    /// <summary>
    /// Copies the Train's nodes in order onto an existing array at the index position.
    /// Includes utility nodes
    /// </summary>
    /// <param name="array">Pre-existing array of nodes</param>
    /// <param name="arrayIndex">First index that will change</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
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
    /// <summary>
    /// Copies the Train's nodes in order onto an existing array at index 0.
    /// Includes utility nodes
    /// </summary>
    /// <param name="array">Pre-existing array of nodes</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public override void CopyTo(AbstractTrainNode[] array) => CopyTo(array, 0);
    /// <summary>
    /// Copies the Train's nodes in order onto an existing array at the index position.
    /// Includes utility nodes
    /// </summary>
    /// <param name="array">Pre-existing array of nodes</param>
    /// <param name="arrayIndex">First index that will change (can be calculated from the back)</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public override void CopyTo(AbstractTrainNode[] array, Index arrayIndex) => CopyTo(array, arrayIndex.GetOffset(GetBranchLength()));

    /// <summary>
    /// Copies the Train's values in order onto an existing array at the index position.
    /// Skips utility nodes as if they don't exist
    /// </summary>
    /// <param name="array">Pre-existing array of <typeparamref name="T"/></param>
    /// <param name="arrayIndex">First index that will change</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void CopyTo(T?[] array, int arrayIndex)
    {
        List<AbstractTrainNode> branch = RawBranchCollapse();

        int cnt = array.Length;
        if (arrayIndex >= cnt || arrayIndex < 0) { throw new IndexOutOfRangeException($"Index {arrayIndex} was outside the bounds of the given array"); }
        if (cnt - arrayIndex < branch.Count) { throw new ArgumentException($"The number of elements in the source {branch.Count} is greater than the available space from {arrayIndex} to the end of the destination array."); }

        int arrIndex = arrayIndex;
        for (int L = 0; L < branch.Count; L++)
        {
            if (branch[L] is ValueTrainNode<T> vnode)
            {
                array[arrIndex] = vnode.GetValue();
                arrIndex++;
            }
        }
    }
    /// <summary>
    /// Copies the Train's values in order onto an existing array at index 0.
    /// Skips utility nodes as if they don't exist
    /// </summary>
    /// <param name="array">Pre-existing array of <typeparamref name="T"/></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void CopyTo(T?[] array) => CopyTo(array, 0);
    /// <summary>
    /// Copies the Train's values in order onto an existing array at the index position.
    /// Skips utility nodes as if they don't exist
    /// </summary>
    /// <param name="array">Pre-existing array of <typeparamref name="T"/></param>
    /// <param name="arrayIndex">First index that will change (can be calculated from the back)</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void CopyTo(T?[] array, Index arrayIndex) => CopyTo(array, arrayIndex.GetOffset(GetBranchLength()));

    public override bool ReplaceAt(int index, T? newValue)
    {
        AbstractTrainNode? node = GetNodeAt(index);
        if (node is ValueTrainNode<T> vnode)
        {
            vnode.SetValue(newValue);
            return true;
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
        bool ret = true;
        for (int i = start; i < end; i++)
        {
            ret &= ReplaceAt(i, newValues[arrcnt]);
            arrcnt++;
        }

        return ret;
    }

    public override bool Signal(TrainSignal signal)
    {
        bool ret = first?.Signal(signal, new Stack<AbstractTrainNode>()).Is(TrainOperations.SUCCESS) ?? false;
        if (ret) { cache?.Invalidate(); }
        return ret;
    }

    public override bool Signal(params TrainSignal[] signals)
    {
        bool ret = true;
        foreach (TrainSignal s in signals)
        {
            ret &= Signal(s);
        }
        return ret;
    }


    public override T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public override T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValue(index, []); set => _setValue(value, index, []); }
    public override List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT] { get => _getValues(range, []); set => _setValues(value, range, []); }
    public override AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public override AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodeAt(index); set => _setNode(value, index, []); }
    public override List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE] { get => GetNodesAt(range); set => _setNodes(value, range, []); }
    public override T? this[int index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public override T? this[Index index, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValue(index, signalsToSendBeforeAccess); set => _setValue(value, index, signalsToSendBeforeAccess); }
    public override List<T?> this[Range range, IndexerInference.Strategies.Direct inferenceStrategy = IndexerInference.DIRECT, params TrainSignal[] signalsToSendBeforeAccess] { get => _getValues(range, signalsToSendBeforeAccess); set => _setValues(value, range, signalsToSendBeforeAccess); }
    public override AbstractTrainNode? this[int index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public override AbstractTrainNode? this[Index index, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNode(index, signalsBeforeAccess); set => _setNode(value, index, signalsBeforeAccess); }
    public override List<AbstractTrainNode> this[Range range, IndexerInference.Strategies.Node inferenceStrategy = IndexerInference.NODE, params TrainSignal[] signalsBeforeAccess] { get => _getNodes(range, signalsBeforeAccess); set => _setNodes(value, range, signalsBeforeAccess); }
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