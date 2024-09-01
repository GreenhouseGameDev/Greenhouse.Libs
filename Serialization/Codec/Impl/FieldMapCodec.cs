using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Codec.Impl;

public class FieldMapCodec<A>(string name, Codec<A> codec) : MapCodec<A> where A : notnull {
    public DataResult<A> Decode<T>(Collection.RecordDictionary<T> dictionary) where T : notnull => dictionary.Values.TryGetValue(dictionary.Ops.Create(name), out T? result) ? codec.Decode(result, dictionary.Ops) : DataResult.Error<A>($"Missing field \"{name}\". ${dictionary.Element}");


    public Collection.RecordBuilder<T> Encode<T>(A input, Collection.RecordBuilder<T> builder) where T : notnull => builder.Add(builder.Ops.Create(name), codec.Encode(input, builder.Ops));
}
