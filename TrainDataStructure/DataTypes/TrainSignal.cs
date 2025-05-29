namespace TrainDataStructure.DataTypes;

public class TrainSignal
{
    protected readonly string[] codes = [EMPTY_CODE];
    protected readonly object?[] payloads = [];

    protected static void SignalEventsConstructor(TrainSignal signalOrigin) { }
    public delegate void SignalEvents(TrainSignal signalOrigin);
    public SignalEvents OnSignalSuccess = SignalEventsConstructor;

    public bool IsCancelled => token.IsCancellationRequested;
    protected readonly CancellationToken token;

    protected readonly bool transitive = false; public bool IsTransitive => transitive;
    protected readonly bool awaitable = false; public bool IsAwaitable => awaitable;

    public static readonly string EMPTY_CODE = "__EMPTY__";
    public static readonly TrainSignal EMPTY = new TrainSignal([EMPTY_CODE], [], false, false);
    protected readonly bool empty = false;
    public bool IsEmpty => empty;

    public TrainSignal Copy()
    {
        string[] newCodes = new string[codes.Length];
        codes.CopyTo(newCodes);

        object?[] newPayloads = new object?[payloads.Length];
        payloads.CopyTo(newPayloads);

        TrainSignal ret = new TrainSignal(newCodes, newPayloads, token, transitive, awaitable);

        foreach (var method in OnSignalSuccess.GetInvocationList().Skip(1))
        {
            ret.OnSignalSuccess += (SignalEvents)method;
        }

        return ret;
    }
    
    public TrainSignal(string[] codes, object?[] payloads, CancellationToken token, bool transitive = false, bool awaitable = true)
    {
        this.token = token;
        this.codes = codes;
        this.payloads = payloads;
        this.transitive = transitive;
        this.awaitable = awaitable;
        this.empty = codes.Contains(EMPTY_CODE);
    }
    public TrainSignal(string[] codes, object?[] payloads, bool transitive = false, bool awaitable = true)
    {
        this.token = CancellationToken.None;
        this.codes = codes;
        this.payloads = payloads;
        this.transitive = transitive;
        this.awaitable = awaitable;
        this.empty = codes.Contains(EMPTY_CODE);
    }

    public static TrainSignal Simple(string code)
    {
        return new TrainSignal([code], []);
    }

    public bool HasCode(string code) => codes.Contains(code);
    public bool HasMatchingCode(string regex) => codes.FirstOrDefault((s) => { return Regex.IsMatch(s, regex); }) != null;
    public string? FindCode(string regex) => codes.FirstOrDefault((s) => { return Regex.IsMatch(s, regex); });
    public string[] FindMatchingCodes(string regex) => codes.Where(s => Regex.IsMatch(s, regex)).ToArray();
    public string[] FindCodeAndGetGroups(string regex)
    {
        if (codes.Length == 0) return [];
        string? match = codes.FirstOrDefault((s) => { return Regex.IsMatch(s, regex); });
        if (match == null) return [];
        Match m = Regex.Match(match, regex);

        string[] ret = m.Groups
                        .Cast<Group>() // From GroupCollection to IEnumerable<Group>
                        .Skip(1) // Remove full match at group 0
                        .Select(g => g.Value) // From IEnumerable<Group> to IEnumerable<string>
                        .ToArray(); // From IEnumerable<string> to string[]

        return ret;
    }
    public Dictionary<string, string[]> FindMatchingCodesAndGetGroups(string regex)
    {
        int cnt = codes.Length;
        Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
        if (cnt == 0) return ret;

        for (int i = 0; i < cnt; i++)
        {
            string code = codes[i];
            Match m = Regex.Match(code, regex);
            if (m.Success)
            {
                ret.Add(code, m.Groups
                        .Cast<Group>() // From GroupCollection to IEnumerable<Group>
                        .Skip(1) // Remove full match at group 0
                        .Select(g => g.Value) // From IEnumerable<Group> to IEnumerable<string>
                        .ToArray()); // From IEnumerable<string> to string[])
            }
        }
        return ret;
    }
}