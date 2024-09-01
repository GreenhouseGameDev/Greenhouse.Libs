using System.Numerics;

namespace Greenhouse.Libs.Serialization;

public readonly struct Unit;

public static class Codecs {
    public static readonly Codec<Unit> Unit = new PrimitiveImplCodec<Unit>(
        reader => new(),
        (writer, value) => {}
    );
    public static readonly Codec<bool> Bool = new PrimitiveImplCodec<bool>(
        reader => reader.Primitive().Bool(),
        (writer, value) => writer.Primitive().Bool(value)
    );
    public static readonly Codec<char> Char = new PrimitiveImplCodec<char>(
        reader => reader.Primitive().Char(),
        (writer, value) => writer.Primitive().Char(value)
    );
    public static readonly Codec<byte> Byte = new PrimitiveImplCodec<byte>(
        reader => reader.Primitive().Byte(),
        (writer, value) => writer.Primitive().Byte(value)
    );
    public static readonly Codec<sbyte> SByte = new PrimitiveImplCodec<sbyte>(
        reader => reader.Primitive().SByte(),
        (writer, value) => writer.Primitive().SByte(value)
    );
    public static readonly Codec<short> Short = new PrimitiveImplCodec<short>(
        reader => reader.Primitive().Short(),
        (writer, value) => writer.Primitive().Short(value)
    );
    public static readonly Codec<ushort> UShort = new PrimitiveImplCodec<ushort>(
        reader => reader.Primitive().UShort(),
        (writer, value) => writer.Primitive().UShort(value)
    );
    public static readonly Codec<int> Int = new PrimitiveImplCodec<int>(
        reader => reader.Primitive().Int(),
        (writer, value) => writer.Primitive().Int(value)
    );
    public static readonly Codec<uint> UInt = new PrimitiveImplCodec<uint>(
        reader => reader.Primitive().UInt(),
        (writer, value) => writer.Primitive().UInt(value)
    );
    public static readonly Codec<long> Long = new PrimitiveImplCodec<long>(
        reader => reader.Primitive().Long(),
        (writer, value) => writer.Primitive().Long(value)
    );
    public static readonly Codec<ulong> ULong = new PrimitiveImplCodec<ulong>(
        reader => reader.Primitive().ULong(),
        (writer, value) => writer.Primitive().ULong(value)
    );
    public static readonly Codec<float> Float = new PrimitiveImplCodec<float>(
        reader => reader.Primitive().Float(),
        (writer, value) => writer.Primitive().Float(value)
    );
    public static readonly Codec<double> Double = new PrimitiveImplCodec<double>(
        reader => reader.Primitive().Double(),
        (writer, value) => writer.Primitive().Double(value)
    );
    public static readonly Codec<string> String = new PrimitiveImplCodec<string>(
        reader => reader.Primitive().String(),
        (writer, value) => writer.Primitive().String(value)
    );
}

public interface Codec {
    public object? Read(DataReader reader);
    public void Write(DataWriter writer, object? value);
}

public abstract record Codec<TValue> : Codec {
    public abstract TValue ReadGeneric(DataReader reader);
    public abstract void WriteGeneric(DataWriter writer, TValue value);

    public FieldCodec<TValue, TParent> Field<TParent>(string name, Func<TParent, TValue> getter) 
        => new NotNullFieldCodec<TValue, TParent>(this, name, getter);

    public Codec<TValue[]> Array()
        => new ArrayCodec<TValue>(this);

    public void Write(DataWriter writer, object? value)
        => WriteGeneric(writer, (TValue) value!);

    public object? Read(DataReader reader)
        => ReadGeneric(reader);
}

public static class CodecExtensions {
    public static FieldCodec<TValue?, TParent> NullableField<TValue, TParent>(this Codec<TValue> codec, string name, Func<TParent, TValue?> getter) where TValue : struct
        => new NullableStructFieldCodec<TValue, TParent>(codec, name, getter);
    
    public static FieldCodec<TValue?, TParent> NullableField<TValue, TParent>(this Codec<TValue> codec, string name, Func<TParent, TValue?> getter) where TValue : class?
        => new NullableClassFieldCodec<TValue, TParent>(codec, name, getter);
}

public record StringEnumCodec<TValue> : Codec<TValue> where TValue : struct, Enum {
    public override TValue ReadGeneric(DataReader reader) {
        var value = reader.Primitive().String();
        if (Enum.TryParse<TValue>(value, out var result))
            return result;
        throw new ArgumentException($"Invalid enum! Got {value} for type {typeof(TValue)}");
    }

    public override void WriteGeneric(DataWriter writer, TValue value) {
        writer.Primitive().String(value.ToString());
    }
}

public record IntEnumCodec<TValue, TParent>(Codec<TParent> Parent) : Codec<TValue> where TValue : struct, Enum where TParent : struct, IBinaryInteger<TParent>, IBinaryNumber<TParent>, INumber<TParent>, INumberBase<TParent> {
    public override TValue ReadGeneric(DataReader reader)
        => (TValue) Enum.ToObject(typeof(TValue), Parent.ReadGeneric(reader));

    public override void WriteGeneric(DataWriter writer, TValue value) {
        Parent.WriteGeneric(writer, (TParent)Convert.ChangeType(value, typeof(TParent)));
    }
}

internal record PrimitiveImplCodec<TValue>(Func<DataReader, TValue> ReadFunc, Action<DataWriter, TValue> WriteFunc) : Codec<TValue> {
    public override TValue ReadGeneric(DataReader reader) {
        return ReadFunc(reader);
    }

    public override void WriteGeneric(DataWriter writer, TValue value) {
        WriteFunc(writer, value);
    }
}

public record ArrayCodec<TElement>(Codec<TElement> Codec) : Codec<TElement[]> {
    public override TElement[] ReadGeneric(DataReader reader) {
        using var arr = reader.Array();
        var values = new TElement[arr.Length()];

        for (int i = 0; i < values.Length; i++)
            values[i] = Codec.ReadGeneric(arr.Value());

        return values;
    }

    public override void WriteGeneric(DataWriter writer, TElement[] values) {
        using var arr = writer.Array(values.Length);
        for (int i = 0; i < values.Length; i++)
            Codec.WriteGeneric(arr.Value(), values[i]);
    }
}

public interface FieldCodec {
    public object? Read(ObjectDataReader reader);
    public void Write(ObjectDataWriter writer, object? value);
    public object? GetFromParent(object? parent);
}

public abstract record FieldCodec<TValue, TParent>(Func<TParent, TValue> Getter) : FieldCodec {
    public abstract TValue ReadGeneric(ObjectDataReader reader);

    public abstract void WriteGeneric(ObjectDataWriter writer, TValue value);

    public TValue GetFromParentGeneric(TParent parent)
        => Getter(parent);

    public object? Read(ObjectDataReader reader)
        => ReadGeneric(reader);

    public void Write(ObjectDataWriter writer, object? value)
        => WriteGeneric(writer, (TValue) value!);

    public object? GetFromParent(object? parent)
        => GetFromParentGeneric((TParent) parent!);
}

public record NullableStructFieldCodec<TValue, TParent>(Codec<TValue> Codec, string Name, Func<TParent, TValue?> Getter) : FieldCodec<TValue?, TParent>(Getter) where TValue : struct {
    public override TValue? ReadGeneric(ObjectDataReader reader) {
        using var field = reader.NullableField(Name);
        if (field.IsNull())
            return null;
        return Codec.ReadGeneric(field.NotNull());
    }
    
    public override void WriteGeneric(ObjectDataWriter writer, TValue? value) {
        using var field = writer.NullableField(Name);
        if (value == null) {
            field.Null();
            return;
        }
        Codec.WriteGeneric(field.NotNull(), value.Value);
    }
}

public record NullableClassFieldCodec<TValue, TParent>(Codec<TValue> Codec, string Name, Func<TParent, TValue?> Getter) : FieldCodec<TValue?, TParent>(Getter) where TValue : class? {
    public override TValue? ReadGeneric(ObjectDataReader reader) {
        using var maybe = reader.NullableField(Name);
        if (maybe.IsNull())
            return null;
        return Codec.ReadGeneric(maybe.NotNull());
    }
    
    public override void WriteGeneric(ObjectDataWriter writer, TValue? value) {
        using var field = writer.NullableField(Name);
        if (value == null) {
            field.Null();
            return;
        }
        Codec.WriteGeneric(field.NotNull(), value);
    }
}

public record NotNullFieldCodec<TValue, TParent>(Codec<TValue> Codec, string Name, Func<TParent, TValue> Getter) : FieldCodec<TValue, TParent>(Getter) {
    public override TValue ReadGeneric(ObjectDataReader reader)
        => Codec.ReadGeneric(reader.Field(Name));

    public override void WriteGeneric(ObjectDataWriter writer, TValue value)
        => Codec.WriteGeneric(writer.Field(Name), value);
}


public record RecordCodec<TValue> : Codec<TValue> {
    private readonly FieldCodec[] Fields;
    private readonly Delegate Constructor;

    private RecordCodec(FieldCodec[] fields, Delegate contructor) {
        Fields = fields;
        Constructor = contructor;
    }

    public static RecordCodec<TValue> Create<TParam1>(FieldCodec<TParam1, TValue> codec1, Func<TParam1, TValue> constructor)
        => new([codec1], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, Func<TParam1, TParam2, TValue> constructor) 
        => new([codec1, codec2], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, Func<TParam1, TParam2, TParam3, TValue> constructor) 
        => new([codec1, codec2, codec3], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, Func<TParam1, TParam2, TParam3, TParam4, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, FieldCodec<TParam15, TValue> codec15, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14, codec15], constructor);

    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TParam16>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, FieldCodec<TParam15, TValue> codec15, FieldCodec<TParam16, TValue> codec16, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TParam16, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14, codec15, codec16], constructor);


    public override TValue ReadGeneric(DataReader reader) {
        using var obj = reader.Object();

        object?[] values = new object[Fields.Length];

        for (int i = 0; i < Fields.Length; i++)
            values[i] = Fields[i].Read(obj);
        
        return (TValue) Constructor.DynamicInvoke(values)!;
    }

    public override void WriteGeneric(DataWriter writer, TValue value) {
        using var obj = writer.Object(Fields.Length);
        for (int i = 0; i < Fields.Length; i++) {
            var field = Fields[i];
            field.Write(obj, field.GetFromParent(value));
        }
    }
}
