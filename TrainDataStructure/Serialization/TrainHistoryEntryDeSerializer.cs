namespace TrainDataStructure.Serialization;
public sealed class TrainHistoryEntryDeSerializer : TrainItemDeSerializer
{
    public const char SERIALIZATION_SEPARATOR = ITrainHistoryEntry.SERIALIZATION_SEPARATOR;

    public readonly FrozenDictionary<string, Func<string, ITrainHistoryEntry>> DeSerializers;
    public static readonly TrainHistoryEntryDeSerializer Instance = new TrainHistoryEntryDeSerializer();

    private TrainHistoryEntryDeSerializer()
    {
        DeSerializers = FindDeSerializers().ToFrozenDictionary();
    }

    public static ITrainHistoryEntry DeSerialize(string serializedNode)
    {
        string firstSegment = serializedNode.Split(SERIALIZATION_SEPARATOR)[0];
        return Instance.DeSerializers.TryGetValue(firstSegment, out Func<string, ITrainHistoryEntry>? value) ? value.Invoke(serializedNode)
            : throw new TrainDeSerializationNoDeSerializerFoundException($"No deserializer found for entry: {firstSegment}");
    }

    private Dictionary<string, Func<string, ITrainHistoryEntry>> FindDeSerializers()
    {
        List<MethodInfo> methods = FindMethods<TrainHistoryEntryDeSerializerAttribute>();

        Dictionary<string, Func<string, ITrainHistoryEntry>> ret = new();


        foreach (var method in methods)
        {
            if (!method.ReturnType.Equals(typeof(ITrainHistoryEntry))) { continue; }
            var parametersCheck = method.GetParameters();
            if (parametersCheck.Length != 1) { continue; }
            if (!parametersCheck[0].ParameterType.Equals(typeof(string))) { continue; }

            TrainHistoryEntryDeSerializerAttribute attribute = method.GetCustomAttribute<TrainHistoryEntryDeSerializerAttribute>()
                ?? throw new TrainDeSerializationFinderAttributeIncoherenceException($"Attribute get failed on method {method.Name}");

            Func<string, ITrainHistoryEntry> func = new Func<string, ITrainHistoryEntry>(
                o => (ITrainHistoryEntry)(method.Invoke(null, [o])
                ?? throw new InvalidCastException($"Null reference cannot be cast to {nameof(ITrainHistoryEntry)} for {nameof(Func<string, ITrainHistoryEntry>)}"))
                );

            FrozenSet<string> validNodeTypes = attribute.EntryTypes;
            foreach (string node in validNodeTypes)
            {
                ret.Add(node, func);
            }
        }

        return ret;
    }
}

/// <summary>
/// This attribute is used to find deserializer methods declared outside the library. They will be found by reflection automatically.
/// Methods must accept a string and return ITrainHistoryEntry (neither null).
/// Accepts a string[] in the constructor. Each string is used to match the method to that entry type.
/// It must be the first segment in the serialized string (separated by the global separator char in ITrainHistoryEntry)
/// </summary>
/// <param name="EntryTypes">Array of constant first segments for each entry type being deserialized</param>
[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class TrainHistoryEntryDeSerializerAttribute(params string[] EntryTypes) : Attribute
{
    public readonly FrozenSet<string> EntryTypes = EntryTypes.ToFrozenSet();
}