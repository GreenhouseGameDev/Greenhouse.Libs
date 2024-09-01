using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Codec.Collection;

public interface RecordBuilder<T> where T : notnull {
    CodecOps<T> Ops { get; }
    RecordBuilder<T> Add(T key, T value);
    RecordBuilder<T> WithErrorsFrom<E>(DataResult<E> error) where E : notnull;
    DataResult<T> Build();

    RecordBuilder<T> Add(T key, DataResult<T> value) {
        return value.Apply(v => Add(key, v), e => WithErrorsFrom(DataResult.Error<string>(e)));
    }
    RecordBuilder<T> Add(DataResult<T> key, DataResult<T> value) {
        return key.Apply(k => Add(k, value), e => WithErrorsFrom(DataResult.Error<string>(e)));
    }
    RecordBuilder<T> Add(string key, T value) {
        return Add(Ops.Create(key), value);
    }
    RecordBuilder<T> Add(string key, DataResult<T> value) {
        return Add(Ops.Create(key), value);
    }
}
