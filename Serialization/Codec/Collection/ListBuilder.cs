using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Codec.Collection;

public interface ListBuilder<T> where T : notnull {
    CodecOps<T> Ops { get; }
    RecordBuilder<T> Add(T value);
    RecordBuilder<T> WithErrorsFrom<E>(DataResult<E> error) where E : notnull;
    DataResult<T> Build();

    RecordBuilder<T> Add(DataResult<T> value) {
        return value.Apply(Add, e => WithErrorsFrom(DataResult.Error<string>(e)));
    }
}
