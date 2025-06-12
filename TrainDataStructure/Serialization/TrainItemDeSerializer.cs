namespace TrainDataStructure.Serialization;
public abstract class TrainItemDeSerializer
{
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
    protected static List<MethodInfo> FindMethods<AttributeType>() where AttributeType : Attribute
    {
        var visited = FindAssemblies();

        List<MethodInfo> libMethods = [];

        foreach (Assembly assemb in visited)
        {
            var methods = assemb.GetTypes().SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                                .Where(x => x.GetCustomAttribute<AttributeType>() is not null);

            libMethods.AddRange(methods);
        }

        return libMethods;
    }
}

#region Exception Types
#pragma warning disable IDE0290 // Use primary constructor for each of these exception types
public abstract class TrainDeSerializationException : Exception { public TrainDeSerializationException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidFormatException : TrainDeSerializationException { public TrainDeSerializationInvalidFormatException(string message) : base(message) { } }
public sealed class TrainDeSerializationInvalidTypeException : TrainDeSerializationException { public TrainDeSerializationInvalidTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationAbstractTypeException : TrainDeSerializationException { public TrainDeSerializationAbstractTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationNoDeSerializerFoundException : TrainDeSerializationException { public TrainDeSerializationNoDeSerializerFoundException(string message) : base(message) { } }
public sealed class TrainDeSerializationUnknownTypeException : TrainDeSerializationException { public TrainDeSerializationUnknownTypeException(string message) : base(message) { } }
public sealed class TrainDeSerializationFinderAttributeIncoherenceException : TrainDeSerializationException { public TrainDeSerializationFinderAttributeIncoherenceException(string message) : base(message) { } }
#pragma warning restore IDE0290
#endregion