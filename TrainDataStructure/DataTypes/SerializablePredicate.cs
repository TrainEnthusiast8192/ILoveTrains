namespace TrainDataStructure.DataTypes;
public sealed class SerializablePredicate<T> : IComparable
{
    private readonly Func<T, bool> AsPredicate;
    private readonly Expression<Func<T, bool>> AsExpression;

    public bool Invoke(T argument) => AsPredicate.Invoke(argument);

    private SerializablePredicate(Func<T, bool> predicate, Expression<Func<T, bool>> expression)
    {
        AsPredicate = predicate;
        AsExpression = expression;
    }
    public SerializablePredicate(Expression<Func<T, bool>> expression)
    {
        AsPredicate = expression.Compile();
        AsExpression = expression;
    }

    public override string ToString()
    {
        return AsExpression.ToString();
    }

    public int CompareTo(object? obj)
    {
        return 1;
    }

    public static implicit operator Func<T, bool>(SerializablePredicate<T> serPred) => serPred.AsPredicate;
    public static implicit operator Expression<Func<T, bool>>(SerializablePredicate<T> serPred) => serPred.AsExpression;
}