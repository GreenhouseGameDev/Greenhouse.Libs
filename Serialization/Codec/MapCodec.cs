using Greenhouse.Libs.Serialization.Codec.Collection;
using Greenhouse.Libs.Serialization.Data;

// TODO: Figure out better name for this class.
namespace Greenhouse.Libs.Serialization.Codec;

public static class MapCodec {
    public static UnitMapCodec<A> Unit<A>(Func<A> func) where A: notnull => new(func); 
}

public interface MapEncoder<in A> where A : notnull {
    public RecordBuilder<T> Encode<T>(A input, RecordBuilder<T> builder) where T : notnull;
}

public interface MapDecoder<A> where A : notnull {
    public DataResult<A> Decode<T>(RecordDictionary<T> dictionary) where T : notnull;
    public DataResult<A> Decode<T>(CodecOps<T> ops, T element) where T : notnull {
        return Decode(RecordDictionary.Create(ops, element));
    }
}

public interface MapCodec<A> : MapEncoder<A>, MapDecoder<A> where A : notnull { }

public class UnitMapCodec<A>(Func<A> func) : MapCodec<A> where A: notnull {
    public DataResult<A> Decode<T>(RecordDictionary<T> dictionary) where T : notnull => DataResult.Success(func());
    public RecordBuilder<T> Encode<T>(A input, RecordBuilder<T> builder) where T : notnull => builder;
}
