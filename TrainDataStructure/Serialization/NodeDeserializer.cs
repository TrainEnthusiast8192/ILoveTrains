using System.Collections.Frozen;
using System.Reflection;

namespace TrainDataStructure.Serialization;
public sealed class NodeDeserializer
{
    public const char SERIALIZATION_SEPARATOR = AbstractTrainNode.SERIALIZATION_SEPARATOR;

    public static readonly Dictionary<string, Func<string, AbstractTrainNode>> DeSerializers = new();
    private static readonly NodeDeserializer _instance = new NodeDeserializer();

    private NodeDeserializer()
    {
        FindDeSerializers();
    }

    public static AbstractTrainNode DeSerialize(string serializedNode)
    {
        string firstSegment = serializedNode.Split(SERIALIZATION_SEPARATOR)[0];
        return DeSerializers.TryGetValue(firstSegment, out Func<string, AbstractTrainNode>? value) ? value.Invoke(serializedNode)
            : throw new TrainDeSerializationNoDeSerializerFoundException($"No deserializer found for node: {serializedNode}");
    }

    private void FindDeSerializers()
    {
        var externalMethods = Assembly.GetEntryAssembly()?
                                        .GetTypes().SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                                        .Where(x => x.GetCustomAttribute<TrainNodeDeSerializerAttribute>() is not null);
        
        var libMethods = Assembly.GetAssembly(typeof(NodeDeserializer))?
                                .GetTypes().SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                                .Where(x => x.GetCustomAttribute<TrainNodeDeSerializerAttribute>() is not null);

        var methods = libMethods?.Union(externalMethods ?? []) ?? externalMethods;
        if (methods is null) { return; }

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
                DeSerializers.Add(node, func);
            }
        }
    }


    [TrainNodeDeSerializer("OrphanTrainNode", "AbstractTrainNode", "ValueTrainNode", "SwitchTrainNode", "WaitTrainNode", "MarkerTrainNode")]
    public static AbstractTrainNode StandardDeSerialize(string serializedNode)
    {
        Span<string> groups = serializedNode.Split(AbstractTrainNode.SERIALIZATION_SEPARATOR);
        if (groups.Length == 0) { throw new ArgumentException("String contains no separable groups"); }

        switch (groups[0])
        {
            default:
                throw new TrainDeSerializationInvalidNodeTypeException($"Invalid node type {groups[0]}");
            case "OrphanTrainNode":
                throw new TrainDeSerializationAbstractNodeTypeException($"Cannot create abstract node type {groups[0]}");
            case "AbstractTrainNode":
                throw new TrainDeSerializationAbstractNodeTypeException($"Cannot create abstract node type {groups[0]}");
            case "ValueTrainNode":
                // Get the generic type
                Type typeParameter = Type.GetType(groups[2]) 
                    ?? throw new ArgumentException($"Unknown type parameter {groups[1]} found");

                // Get the parse method, which must be static and must accept a single string. TryParse is also valid
                MethodInfo? parseMethod = typeParameter.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]) 
                    ?? typeParameter.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeParameter.MakeByRefType()]);

                // Pass fourth argument with no caller onto parseMethod
                // We concatenate them in case the value used the same separator
                var slice = groups.Slice(3);

                if (typeof(AbstractTrainNode).IsAssignableFrom(typeParameter))
                {
                    int cnt = slice.Length;
                    for (int i = 1; i < cnt; i++)
                    {
                        slice[i] = SERIALIZATION_SEPARATOR + slice[i];
                    }
                    parseMethod = typeof(NodeDeserializer).GetMethod("DeSerialize", BindingFlags.Public | BindingFlags.Static);
                }

                string valToParse = String.Concat(slice);
                object? value = parseMethod?.Invoke(null, [valToParse]) ?? default;

                Type nodeType = typeof(ValueTrainNode<>).MakeGenericType([typeParameter]);

                // Build the node as a non-generic one
                AbstractTrainNode node = (AbstractTrainNode)(Activator.CreateInstance(nodeType, value) 
                    ?? throw new TrainDeSerializationUnknownNodeTypeException($"Unknown node type {nodeType} found"));

                // Get the SetValue method and call it with what we have
                nodeType.GetMethod("SetValue", BindingFlags.Public, [typeParameter])?.Invoke(node, [value]);

                // Take care of the GUID directly
                nodeType.GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(node, Guid.Parse(groups[1]));

                return node;

            case "SwitchTrainNode":
                // Simple, parsable fields
                int id = int.Parse(groups[1]);
                bool fork = bool.Parse(groups[2]);

                SwitchTrainNode ret = new SwitchTrainNode(id);
                ret.ForkLeft = fork;

                // Take care of the GUID directly
                typeof(SwitchTrainNode).GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(ret, Guid.Parse(groups[3]));

                return ret;

            case "WaitTrainNode":
                // Simple, parsable fields
                int millis = int.Parse(groups[1]);
                WaitTrainNode waitret = new WaitTrainNode(millis);

                // Take care of the GUID directly
                typeof(WaitTrainNode).GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(waitret, Guid.Parse(groups[2]));

                return waitret;

            case "MarkerTrainNode":
                string message = groups[1];
                MarkerTrainNode markerret = new MarkerTrainNode(message);

                // Take care of the GUID directly
                typeof(MarkerTrainNode).GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(markerret, Guid.Parse(groups[2]));

                return markerret;
        }
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
public abstract class TrainDeSerializationException : Exception { public TrainDeSerializationException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidFormatException : TrainDeSerializationException { public TrainDeSerializationInvalidFormatException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidNodeTypeException : TrainDeSerializationException { public TrainDeSerializationInvalidNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationAbstractNodeTypeException : TrainDeSerializationException { public TrainDeSerializationAbstractNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationNoDeSerializerFoundException : TrainDeSerializationException { public TrainDeSerializationNoDeSerializerFoundException(string message) : base(message) { } }
public sealed class TrainDeSerializationUnknownNodeTypeException : TrainDeSerializationException { public TrainDeSerializationUnknownNodeTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationFinderAttributeIncoherenceException : TrainDeSerializationException { public TrainDeSerializationFinderAttributeIncoherenceException(string message) : base(message) { } }
#endregion