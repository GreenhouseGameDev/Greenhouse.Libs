using Greenhouse.Libs.Serialization.Codec.Impl;

namespace Greenhouse.Libs.Serialization.Codec
{
    public static class CodecExtensions
    {
        public static IMapCodec<A> Field<A>(this ICodec<A> codec, string name) where A : notnull => new FieldMapCodec<A>(name, codec);
    }
}
