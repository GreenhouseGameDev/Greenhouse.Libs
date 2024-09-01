using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Codec;

public interface Encoder<in A> where A : notnull {
    public DataResult<T> Encode<T>(A input, CodecOps<T> ops) where T : notnull;
}

public interface Decoder<A> where A : notnull {
    public DataResult<A> Decode<T>(T input, CodecOps<T> ops) where T : notnull;
}

public interface Codec<A> : Encoder<A>, Decoder<A> where A : notnull {}
