using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;
namespace TrainDataStructure.DataTypes;
public sealed class SerializablePredicate<T> : IComparable
{
    public const char SERIALIZATION_SEPARATOR = '\u4000';
    public const char SERIALIZATION_SECTION_SEPARATOR = '\u4001';
    public readonly Func<T, bool> AsPredicate;
    private readonly string AsText;
    private readonly FrozenSet<string> namespaces;
    private readonly FrozenSet<Assembly> references;

    private SerializablePredicate(string Text, FrozenSet<string> namespaces, FrozenSet<Assembly> references)
    {
        this.AsText = Text;
        this.namespaces = namespaces;
        this.references = references;
        AsPredicate = CSharpScript.EvaluateAsync<Func<T, bool>>(
            AsText,
            ScriptOptions.Default
                .AddReferences(references)
                .AddImports(namespaces)
        ).Result;
    }
    public SerializablePredicate(string AsText, string[] namespaces, Assembly[] references)
    {
        this.AsText = AsText;
        AsPredicate = CSharpScript.EvaluateAsync<Func<T, bool>>(
            AsText,
            ScriptOptions.Default
                .AddReferences(references)
                .AddImports(namespaces)
        ).Result;

        this.namespaces = namespaces.ToFrozenSet();
        this.references = references.ToFrozenSet();
    }
    public SerializablePredicate(Expression<Func<T, bool>> AsText, string[] namespaces, Assembly[] references)
    {
        this.AsText = AsText.ToString();
        AsPredicate = AsText.Compile();

        this.namespaces = namespaces.ToFrozenSet();
        this.references = references.ToFrozenSet();
    }

    public string Serialize()
    {
        var sb = new StringBuilder(AsText).Append(SERIALIZATION_SECTION_SEPARATOR);
        foreach (var str in namespaces)
        {
            sb.Append(str).Append(SERIALIZATION_SEPARATOR);
        }
        sb.Append(SERIALIZATION_SECTION_SEPARATOR);
        foreach (var str in references)
        {
            sb.Append(str.FullName).Append(SERIALIZATION_SEPARATOR);
        }

        return sb.ToString();
    }
    public override string? ToString() => AsText;
    public static SerializablePredicate<T> Parse(string serializedPredicate)
    {
        Span<string> sections = serializedPredicate.Split(SERIALIZATION_SECTION_SEPARATOR);

        string Text = sections[0];

        FrozenSet<string> namespaces = sections[1].Split(SERIALIZATION_SEPARATOR).Where(o => !o.IsWhiteSpace()).ToFrozenSet();
        FrozenSet<Assembly> references = sections[2].Split(SERIALIZATION_SEPARATOR).Where(o => !o.IsWhiteSpace()).Select(o => Assembly.Load(o)).ToFrozenSet();

        return new SerializablePredicate<T>(Text, namespaces, references);
    }

    public int CompareTo(object? obj)
    {
        return AsText.CompareTo(obj);
    }
}