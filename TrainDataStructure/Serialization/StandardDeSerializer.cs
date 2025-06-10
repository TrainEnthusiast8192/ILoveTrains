namespace TrainDataStructure.Serialization;
public static class StandardDeSerializer
{
    public const char SERIALIZATION_SEPARATOR = NodeDeSerializer.SERIALIZATION_SEPARATOR;

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
                return CaseValueNode(groups);

            case "SwitchTrainNode":
                // Simple, parsable fields
                int id = int.Parse(groups[1]);
                bool fork = bool.Parse(groups[2]);

                SwitchTrainNode switchret = new SwitchTrainNode(id);
                switchret.ForkLeft = fork;

                // Take care of the GUID directly
                HandleGUID<SwitchTrainNode>(switchret, Guid.Parse(groups[2]));

                return switchret;

            case "WaitTrainNode":
                // Simple, parsable fields
                int millis = int.Parse(groups[1]);
                WaitTrainNode waitret = new WaitTrainNode(millis);

                // Take care of the GUID directly
                HandleGUID<WaitTrainNode>(waitret, Guid.Parse(groups[2]));

                return waitret;

            case "MarkerTrainNode":
                string message = groups[1];
                MarkerTrainNode markerret = new MarkerTrainNode(message);

                // Take care of the GUID directly
                HandleGUID<MarkerTrainNode>(markerret, Guid.Parse(groups[2]));

                return markerret;
        }
    }

    private static void HandleGUID<NodeType>(NodeType instance, Guid guid)
    {
        typeof(NodeType).GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(instance, guid);
    }

    private static AbstractTrainNode CaseValueNode(Span<string> groups)
    {
        // Get the generic type
        Type typeParameter = Type.GetType(groups[2])
            ?? throw new ArgumentException($"Unknown type parameter {groups[1]} found");

        // Get the parse method, which must be static and must accept a single string. TryParse is also valid
        MethodInfo? parseMethod = typeParameter.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)])
            ?? typeParameter.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeParameter.MakeByRefType()]);

        // Pass fourth argument with no caller onto parseMethod
        // We concatenate them in case the value used the same separator
        var slice = groups[3..];

        if (typeof(AbstractTrainNode).IsAssignableFrom(typeParameter))
        {
            int cnt = slice.Length;
            for (int i = 1; i < cnt; i++)
            {
                slice[i] = SERIALIZATION_SEPARATOR + slice[i];
            }
            parseMethod = typeof(NodeDeSerializer).GetMethod("DeSerialize", BindingFlags.Public | BindingFlags.Static);
        }

        string valToParse = String.Concat(slice);
        object? value = parseMethod?.Invoke(null, [valToParse]) ?? default;

        if (typeof(SerializablePredicate<>).MakeGenericType([typeParameter.GetGenericArguments()[0]]).IsAssignableFrom(typeParameter))
        {
            value = DeserializePredicate(typeParameter, valToParse);
        }

        Type nodeType = typeof(ValueTrainNode<>).MakeGenericType([typeParameter]);

        // Build the node as a non-generic one
        AbstractTrainNode node = (AbstractTrainNode)(Activator.CreateInstance(nodeType, value)
            ?? throw new TrainDeSerializationUnknownNodeTypeException($"Unknown node type {nodeType} found"));

        // Get the SetValue method and call it with what we have
        nodeType.GetMethod("SetValue", BindingFlags.Public, [typeParameter])?.Invoke(node, [value]);

        // Take care of the GUID directly
        nodeType.GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(node, Guid.Parse(groups[1]));

        return node;
    }
    public static object DeserializePredicate(Type targetType, string expressionString)
    {
        Type typeParameter = targetType.GetGenericArguments()[0];

        Type incompleteGeneric = typeof(SerializablePredicate<>);

        Type completedGeneric = incompleteGeneric.MakeGenericType([typeParameter]);

        object deserializedPredicate = Activator.CreateInstance(completedGeneric, [expressionString])
                                        ?? throw new Exception("Predicate deserialization failed");

        return deserializedPredicate;
    }
}
