namespace Greenhouse.Libs.Serialization.Codec.Collection;

public static class RecordDictionary {
    public static RecordDictionary<T> Create<T>(CodecOps<T> ops, T element) where T : notnull => new();
}

public readonly struct RecordDictionary<T> where T : notnull {
    public readonly CodecOps<T> Ops;
    public readonly T Element;
    public readonly IReadOnlyDictionary<T, T> Values;

    public RecordDictionary(CodecOps<T> ops, T element) {
        Ops = ops;
        Element = element;
        Values = ops.GetDictionary(element).ResultOrThrow(ex => new InvalidOperationException($"Could not get dictionary from element: ${element}. ${ex}"));
    }
}
