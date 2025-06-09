using System.Reflection;

namespace TrainDataStructure;
public static class NodeDeserializer
{
    public static AbstractTrainNode DeSerialize(string serializedNode)
    {
        Span<string> groups = serializedNode.Split(AbstractTrainNode.SERIALIZATION_SEPARATOR);
        if (groups.Length == 0) { throw new ArgumentException("String contains no separable groups"); }

        switch (groups[0])
        {
            default:
                throw new ArgumentException($"Invalid node type {groups[0]}");
            case "OrphanTrainNode":
                throw new ArgumentException($"Cannot create abstract node type {groups[0]}");
            case "AbstractTrainNode":
                throw new ArgumentException($"Cannot create abstract node type {groups[0]}");
            case "ValueTrainNode":
                // Get the generic type
                Type typeParameter = Type.GetType(groups[1]) ?? throw new ArgumentException($"Unknown type parameter {groups[1]} found");

                // Get the parse method, which must be static and must accept a single string. TryParse is also valid
                MethodInfo? parseMethod = typeParameter.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]) ?? typeParameter.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeParameter.MakeByRefType()]);
                // Pass third argument with no caller onto parseMethod
                object? value = parseMethod?.Invoke(null, [groups[2]]) ?? default;

                Type nodeType = typeof(ValueTrainNode<>).MakeGenericType([typeParameter]);

                // Build the node as a non-generic one
                AbstractTrainNode node = (AbstractTrainNode)(Activator.CreateInstance(nodeType, value) ?? throw new ArgumentException($"Unknown node type {nodeType} found"));

                // Get the SetValue method and call it with what we have
                nodeType.GetMethod("SetValue", BindingFlags.Public, [typeParameter])?.Invoke(node, [value]);

                // Take care of the GUID directly
                nodeType.GetField("INTERNAL_CONNECTIONS_GUID", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(node, Guid.Parse(groups[3]));

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