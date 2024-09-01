using Greenhouse.Libs.Serialization.Codec.Impl;

namespace Greenhouse.Libs.Serialization.Codec;

public static class CodecExtensions {
    public static MapCodec<A> Field<A>(this Codec<A> codec, string name) where A : notnull => new FieldMapCodec<A>(name, codec);
}
