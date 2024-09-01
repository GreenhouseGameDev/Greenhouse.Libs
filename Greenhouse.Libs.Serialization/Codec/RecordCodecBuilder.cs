using Greenhouse.Libs.Serialization.Codec.Collection;
using Greenhouse.Libs.Serialization.Result;

namespace Greenhouse.Libs.Serialization.Codec
{
    public static class RecordCodecBuilder
    {
        public static Instance<O> Instance<O>() => new();
        public static IMapCodec<O> Build<O>(RecordCodecBuilder<O, O> builder) where O : notnull => new RecordCodec<O>(builder);
        public static IMapCodec<O> MapCodec<O>(Func<Instance<O>, RecordCodecBuilder<O, O>> func) where O : notnull => Build(func(Instance<O>()));
    }

    internal class TestCodec : IMapCodec<int>
    {
        public DataResult<int> Decode<T>(RecordDictionary<T> dictionary) where T : notnull
        {
            throw new NotImplementedException();
        }

        public IRecordBuilder<T> Encode<T>(int input, IRecordBuilder<T> builder) where T : notnull
        {
            throw new NotImplementedException();
        }
    }

    public class Instance<O> where O : notnull
    {
        internal Instance()
        {

        }

        public Func<RecordCodecBuilder<O, A>, RecordCodecBuilder<O, R>> Lift<A, R>(RecordCodecBuilder<O, Func<A, R>> builder) where A : notnull where R : notnull
        {
            return fa =>
            {
                return new RecordCodecBuilder<O, R>(
                    o => builder.Getter.Invoke(o).Invoke(fa.Getter(o)),
                    o => new RecordCodecEncoder<A, R>(fa.Getter(o), fa.Encoder(o), builder.Encoder(o)),
                    new RecordCodecDecoder<A, R>(builder.Decoder, fa.Decoder)
                );
            };
        }
    }

    internal class RecordCodecDecoder<A, R>(IMapDecoder<Func<A, R>> instance, IMapDecoder<A> element) : IMapDecoder<R> where A : notnull where R : notnull
    {
        public DataResult<R> Decode<T>(RecordDictionary<T> dictionary) where T : notnull
        {
            return element.Decode(dictionary).Propagate(x => instance.Decode(dictionary).Select(f => f(x)));
        }
    }

    internal class RecordCodecEncoder<A, R>(A item, IMapEncoder<A> itemEncoder, IMapEncoder<Func<A, R>> finalEncoder) : IMapEncoder<R> where A : notnull where R : notnull
    {
        public IRecordBuilder<T> Encode<T>(R input, IRecordBuilder<T> builder) where T : notnull
        {
            itemEncoder.Encode(item, builder);
            finalEncoder.Encode(a => input, builder);
            return builder;
        }
    }

    public class RecordCodecBuilder<A, R>(Func<A, R> getter, Func<A, IMapEncoder<R>> encoder, IMapDecoder<R> decoder) where A : notnull where R : notnull
    {
        internal readonly Func<A, IMapEncoder<R>> Encoder = encoder;
        internal readonly IMapDecoder<R> Decoder = decoder;
        internal readonly Func<A, R> Getter = getter;
    }

    public class RecordCodec<A>(RecordCodecBuilder<A, A> builder) : IMapCodec<A> where A : notnull
    {
        private RecordCodecBuilder<A, A> Builder = builder;

        public DataResult<A> Decode<T>(RecordDictionary<T> dictionary) where T : notnull
        {
            return Builder.Decoder.Decode(dictionary);
        }

        public IRecordBuilder<T> Encode<T>(A input, IRecordBuilder<T> builder) where T : notnull
        {
            return Builder.Encoder(input).Encode(input, builder);
        }
    }
}
