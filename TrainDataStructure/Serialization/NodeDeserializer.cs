using System.Collections.Frozen;

namespace TrainDataStructure.Serialization;
public sealed class NodeDeSerializer
{
    public const char SERIALIZATION_SEPARATOR = AbstractTrainNode.SERIALIZATION_SEPARATOR;

    public readonly FrozenDictionary<string, Func<string, AbstractTrainNode>> DeSerializers;
    public static readonly NodeDeSerializer Instance = new NodeDeSerializer();

    private NodeDeSerializer()
    {
        DeSerializers = FindDeSerializers().ToFrozenDictionary();
    }

    public static AbstractTrainNode DeSerialize(string serializedNode)
    {
        string firstSegment = serializedNode.Split(SERIALIZATION_SEPARATOR)[0];
        return Instance.DeSerializers.TryGetValue(firstSegment, out Func<string, AbstractTrainNode>? value) ? value.Invoke(serializedNode)
            : throw new TrainDeSerializationNoDeSerializerFoundException($"No deserializer found for node: {firstSegment}");
    }

    private static HashSet<Assembly> FindAssemblies()
    {
        var rootAssembly = Assembly.GetEntryAssembly();

        HashSet<Assembly> visited = [Assembly.GetAssembly(typeof(NodeDeSerializer))];
        var queue = new Queue<Assembly?>();

        queue.Enqueue(rootAssembly);

        while (queue.Count > 0)
        {
            var assembly = queue.Dequeue();
            if (assembly is null) { continue; }
            visited.Add(assembly);

            var references = assembly.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                if (!visited.Contains(Assembly.Load(reference.FullName)))
                {
                    queue.Enqueue(Assembly.Load(reference));
                }
            }
        }
        
        return visited;
    }
    private static List<MethodInfo> FindMethods()
    {
        var visited = FindAssemblies();

        List<MethodInfo> libMethods = [];

        foreach (Assembly assemb in visited)
        {
            var methods = assemb.GetTypes().SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                                .Where(x => x.GetCustomAttribute<TrainNodeDeSerializerAttribute>() is not null);

            libMethods.AddRange(methods);
        }
        
        return libMethods;
    }
    private static Dictionary<string, Func<string, AbstractTrainNode>> FindDeSerializers()
    {
        List<MethodInfo> methods = FindMethods();

        Dictionary<string, Func<string, AbstractTrainNode>> ret = new();


        foreach (var method in methods)
        {
            if (!method.ReturnType.Equals(typeof(AbstractTrainNode))) { continue; }
            var parametersCheck = method.GetParameters();
            if (parametersCheck.Length != 1) { continue; }
            if (!parametersCheck[0].ParameterType.Equals(typeof(string))) { continue; }

            TrainNodeDeSerializerAttribute attribute = method.GetCustomAttribute<TrainNodeDeSerializerAttribute>()
                ?? throw new TrainDeSerializationFinderAttributeIncoherenceException($"Attribute get failed on method {method.Name}");

            Func<string, AbstractTrainNode> func = new Func<string, AbstractTrainNode>(
                o => (AbstractTrainNode)(method.Invoke(null, [o])
                ?? throw new InvalidCastException($"Null reference cannot be cast to {nameof(AbstractTrainNode)} for {nameof(Func<string, AbstractTrainNode>)}"))
                );

            FrozenSet<string> validNodeTypes = attribute.NodeTypes;
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
/// Methods must accept a string and return AbstractTrainNode (neither null).
/// Accepts a string[] in the constructor. Each string is used to match the method to that node type.
/// It must be the first segment in the serialized string (separated by the global separator char)
/// </summary>
/// <param name="NodeTypes">Array of constant first segments for each node type being serialized</param>
[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)] 
public sealed class TrainNodeDeSerializerAttribute(params string[] NodeTypes) : Attribute
{
    public readonly FrozenSet<string> NodeTypes = NodeTypes.ToFrozenSet();
}


#region Exception Types
#pragma warning disable IDE0290 // Use primary constructor
public abstract class TrainDeSerializationException : Exception { public TrainDeSerializationException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidFormatException : TrainDeSerializationException { public TrainDeSerializationInvalidFormatException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidNodeTypeException : TrainDeSerializationException { public TrainDeSerializationInvalidNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationAbstractNodeTypeException : TrainDeSerializationException { public TrainDeSerializationAbstractNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationNoDeSerializerFoundException : TrainDeSerializationException { public TrainDeSerializationNoDeSerializerFoundException(string message) : base(message) { } }
public sealed class TrainDeSerializationUnknownNodeTypeException : TrainDeSerializationException { public TrainDeSerializationUnknownNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationFinderAttributeIncoherenceException : TrainDeSerializationException { public TrainDeSerializationFinderAttributeIncoherenceException(string message) : base(message) { } }
#pragma warning restore IDE0290
#endregion