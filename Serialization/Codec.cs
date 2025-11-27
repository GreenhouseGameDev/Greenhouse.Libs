using System.Numerics;

namespace Greenhouse.Libs.Serialization;

public readonly struct Unit;

public static class Codecs {
    public static readonly Codec<Unit> Unit = new UnitCodec<Unit>(() => new());
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
    public static readonly Codec<Guid> Guid = new PrimitiveImplCodec<Guid>(
        reader => new(Byte.FixedArray(16).ReadGeneric(reader)),
        (writer, value) => Byte.FixedArray(16).WriteGeneric(writer, value.ToByteArray())
    );
    public static readonly Codec<DateTime> DateTime = new PrimitiveImplCodec<DateTime>(
        reader => new DateTime(reader.Primitive().Long()),
        (writer, value) => writer.Primitive().Long(value.Ticks)
    );
}

/// <summary>
/// Base Codec interface, only contains read and write functions.
/// </summary>
public interface Codec {
    /// <summary>
    /// The wrapper read function for intercompatibility between codecs <br/>
    /// Use ReadGeneric instead if possible.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public object? Read(DataReader reader);
    /// <summary>
    /// The wrapper write function for intercompatibility between codecs <br/>
    /// Use WriteGeneric instead if possible.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public void Write(DataWriter writer, object? value);
}

/// <summary>
/// A typed codec, has methods for creating fields and arrays
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract record Codec<TValue> : Codec {
    /// <summary>
    /// Reads a value from a <see cref="DataReader"/>
    /// </summary>
    /// <param name="reader">The reader to read from</param>
    /// <returns>The read value</returns>
    public abstract TValue ReadGeneric(DataReader reader);
    /// <summary>
    /// Writes a value to a <see cref="DataWriter"/>
    /// </summary>
    /// <param name="writer">The writer to write to</param>
    /// <param name="value">The value to write</param>
    public abstract void WriteGeneric(DataWriter writer, TValue value);

    /// <summary>
    /// Creates a non-nullable field from the codec
    /// </summary>
    /// <typeparam name="TParent">The parent type of the field</typeparam>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">A function to get the field from it's parent</param>
    /// <returns>The field codec</returns>
    public FieldCodec<TValue, TParent> Field<TParent>(string name, Func<TParent, TValue> getter) 
        => new NotNullFieldCodec<TValue, TParent>(this, name, getter);

    /// <summary>
    /// Creates a defaulted field from the codec
    /// </summary>
    /// <typeparam name="TParent">The parent type of the field</typeparam>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">A function to get the field from it's parent</param>
    /// <returns>The field codec</returns>
    public FieldCodec<TValue, TParent> DefaultedField<TParent>(string name, Func<TParent, TValue> getter, Func<TValue> defaultValue)
        => new DefaultedFieldCodec<TValue, TParent>(this, name, getter, defaultValue);

    /// <summary>
    /// Creates an array from the codec
    /// </summary>
    /// <returns>The array codec</returns>
    public Codec<TValue[]> Array()
        => new ArrayCodec<TValue>(this);

    /// <summary>
    /// Creates a fixed-size array from the codec
    /// </summary>
    /// <returns>The array codec</returns>
    public Codec<TValue[]> FixedArray(int length)
        => new FixedArrayCodec<TValue>(this, length);

    public void Write(DataWriter writer, object? value)
        => WriteGeneric(writer, (TValue) value!);

    public object? Read(DataReader reader)
        => ReadGeneric(reader);
}

/// <summary>
/// A codec that always returns one value
/// </summary>
/// <typeparam name="TValue">The type of the value to return</typeparam>
/// <param name="Constructor">The function to construct the value</param>
public record UnitCodec<TValue>(Func<TValue> Constructor) : Codec<TValue> {
    public override TValue ReadGeneric(DataReader reader)
        => Constructor();
    
    public override void WriteGeneric(DataWriter writer, TValue value) {}
}

/// <summary>
/// Extensions for codecs.
/// </summary>
public static class CodecExtensions {
    /// <summary>
    /// Creates a nullable field from a struct
    /// </summary>
    /// <typeparam name="TValue">The value of the field</typeparam>
    /// <typeparam name="TParent">The parent of the field</typeparam>
    /// <param name="codec">The base codec to use</param>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">A function to get the field</param>
    /// <returns>The nullable field codec</returns>
    public static FieldCodec<TValue?, TParent> NullableField<TValue, TParent>(this Codec<TValue> codec, string name, Func<TParent, TValue?> getter) where TValue : struct
        => new NullableStructFieldCodec<TValue, TParent>(codec, name, getter);
    
    /// <summary>
    /// Creates a nullable field from a class
    /// </summary>
    /// <typeparam name="TValue">The value of the field</typeparam>
    /// <typeparam name="TParent">The parent of the field</typeparam>
    /// <param name="codec">The base codec to use</param>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">A function to get the field</param>
    /// <returns>The nullable field codec</returns>
    public static FieldCodec<TValue?, TParent> NullableField<TValue, TParent>(this Codec<TValue> codec, string name, Func<TParent, TValue?> getter) where TValue : class?
        => new NullableClassFieldCodec<TValue, TParent>(codec, name, getter);
}

/// <summary>
/// Codec for representing an enum as a string
/// </summary>
/// <typeparam name="TValue">The enum to represent</typeparam>
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

/// <summary>
/// Codec for representing an enum as an int
/// </summary>
/// <typeparam name="TValue">The enum to represent</typeparam>
/// <typeparam name="TParent">The integer type to encode as</typeparam>
/// <param name="Parent">The codec for the integer type to encode as</param>
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

/// <summary>
/// A codec which acts as an array of values
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <param name="Codec"></param>
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

/// <summary>
/// A codec which acts as a fixed array of values
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <param name="Codec"></param>
public record FixedArrayCodec<TElement>(Codec<TElement> Codec, int Length) : Codec<TElement[]> {
    public override TElement[] ReadGeneric(DataReader reader) {
        using var arr = reader.FixedArray(Length);
        var values = new TElement[Length];

        for (int i = 0; i < Length; i++)
            values[i] = Codec.ReadGeneric(arr.Value());

        return values;
    }

    public override void WriteGeneric(DataWriter writer, TElement[] values) {
        using var arr = writer.FixedArray(Length);
        for (int i = 0; i < Length; i++)
            Codec.WriteGeneric(arr.Value(), values[i]);
    }
}

/// <summary>
/// A field of a RecordCodec
/// </summary>
public interface FieldCodec {
    public object? Read(ObjectDataReader reader);
    public void Write(ObjectDataWriter writer, object? value);
    public object? GetFromParent(object? parent);
}

/// <summary>
/// The generic version of FieldCodec
/// </summary>
/// <typeparam name="TValue">The value of the codec</typeparam>
/// <typeparam name="TParent">The parent type for the codec</typeparam>
/// <param name="Getter">A function to get the field from the parent</param>
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

/// <summary>
/// The nullable struct generic version of FieldCodec
/// </summary>
/// <typeparam name="TValue">The value of the codec</typeparam>
/// <typeparam name="TParent">The parent type for the codec</typeparam>
/// <param name="Getter">A function to get the field from the parent</param>
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

/// <summary>
/// The nullable class generic version of FieldCodec
/// </summary>
/// <typeparam name="TValue">The value of the codec</typeparam>
/// <typeparam name="TParent">The parent type for the codec</typeparam>
/// <param name="Getter">A function to get the field from the parent</param>
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

/// <summary>
/// The defaulted generic version of FieldCodec
/// </summary>
/// <typeparam name="TValue">The value of the codec</typeparam>
/// <typeparam name="TParent">The parent type for the codec</typeparam>
/// <param name="Getter">A function to get the field from the parent</param>
public record DefaultedFieldCodec<TValue, TParent>(Codec<TValue> Codec, string Name, Func<TParent, TValue> Getter, Func<TValue> Default) : FieldCodec<TValue, TParent>(Getter) {
    public override TValue ReadGeneric(ObjectDataReader reader) {
        using var maybe = reader.NullableField(Name);
        if (maybe.IsNull())
            return Default();
        return Codec.ReadGeneric(maybe.NotNull());
    }
    
    public override void WriteGeneric(ObjectDataWriter writer, TValue value) {
        using var field = writer.NullableField(Name);
        Codec.WriteGeneric(field.NotNull(), value);
    }
}

/// <summary>
/// The non-nullable generic version of FieldCodec
/// </summary>
/// <typeparam name="TValue">The value of the codec</typeparam>
/// <typeparam name="TParent">The parent type for the codec</typeparam>
/// <param name="Getter">A function to get the field from the parent</param>
public record NotNullFieldCodec<TValue, TParent>(Codec<TValue> Codec, string Name, Func<TParent, TValue> Getter) : FieldCodec<TValue, TParent>(Getter) {
    public override TValue ReadGeneric(ObjectDataReader reader)
        => Codec.ReadGeneric(reader.Field(Name));

    public override void WriteGeneric(ObjectDataWriter writer, TValue value)
        => Codec.WriteGeneric(writer.Field(Name), value);
}

/// <summary>
/// A simple wrapper type for a variant's result
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="type"></param>
/// <param name="value"></param>
public record struct Variant<TKey, TResult>(TKey type, TResult value);

/// <summary>
/// A codec which acts as a resolver for other codecs.
/// Used for polymorphic serialization.
/// </summary>
/// <typeparam name="TKey">The type of the key</typeparam>
/// <typeparam name="TResult">The parent type for all variants</typeparam>
/// <param name="KeyCodec"The codec to parse the key></param>
/// <param name="Resolver">A function to resolve a codec with a given key</param>
/// <param name="TypeField">The name of the type field</param>
/// <param name="ValueField">The name of the value field</param>
public record VariantCodec<TKey, TResult>(Codec<TKey> KeyCodec, Func<TKey, Codec<TResult>> Resolver, string TypeField = "type", string ValueField = "value") : Codec<Variant<TKey, TResult>> {
    public override Variant<TKey, TResult> ReadGeneric(DataReader reader) {
        using var obj = reader.Object();
        var key = KeyCodec.ReadGeneric(obj.Field(TypeField));
        var resolved = Resolver(key);
        var value = resolved.ReadGeneric(obj.Field(ValueField));
        return new(key, value);
    }

    public override void WriteGeneric(DataWriter writer, Variant<TKey, TResult> value) {
        var resolved = Resolver(value.type);
        using var obj = writer.Object(2);
        KeyCodec.WriteGeneric(writer, value.type);
        resolved.WriteGeneric(writer, value.value);
    }
}

/// <summary>
/// Like the VariantCodec, but inlines all fields.
/// All variants must be record codecs
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="KeyCodec"The codec to parse the key></param>
/// <param name="Resolver">A function to resolve a codec with a given key</param>
/// <param name="TypeField">The name of the type field</param>
public record RecordVariantCodec<TKey, TResult>(Codec<TKey> KeyCodec, Func<TKey, RecordCodec<TResult>> Resolver, string TypeField = "type") : Codec<Variant<TKey, TResult>> {
    public override Variant<TKey, TResult> ReadGeneric(DataReader reader) {
        using var obj = reader.Object();
        var key = KeyCodec.ReadGeneric(obj.Field(TypeField));
        var resolved = Resolver(key);
        var value = resolved.ReadInline(obj);
        return new(key, value);
    }
    
    public override void WriteGeneric(DataWriter writer, Variant<TKey, TResult> value) {
        var resolved = Resolver(value.type);
        using var obj = writer.Object(1 + resolved.FieldCount);
        KeyCodec.WriteGeneric(writer, value.type);
        resolved.WriteGeneric(writer, value.value);
    }
}

/// <summary>
/// A codec representing an object.
/// </summary>
/// <typeparam name="TValue">The type of the codec</typeparam>
public record RecordCodec<TValue> : Codec<TValue> {
    private readonly FieldCodec[] Fields;
    private readonly Delegate Constructor;
    /// <summary>
    /// The number of fields in this record
    /// </summary>
    public int FieldCount => Fields.Length;

    private RecordCodec(FieldCodec[] fields, Delegate contructor) {
        Fields = fields;
        Constructor = contructor;
    }

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create(Func<TValue> constructor)
        => new([], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1>(FieldCodec<TParam1, TValue> codec1, Func<TParam1, TValue> constructor)
        => new([codec1], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, Func<TParam1, TParam2, TValue> constructor) 
        => new([codec1, codec2], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, Func<TParam1, TParam2, TParam3, TValue> constructor) 
        => new([codec1, codec2, codec3], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, Func<TParam1, TParam2, TParam3, TParam4, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, FieldCodec<TParam15, TValue> codec15, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14, codec15], constructor);

    /// <summary>
    /// Creates a record codec
    /// </summary>
    /// <returns>A record codec</returns>
    public static RecordCodec<TValue> Create<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TParam16>(FieldCodec<TParam1, TValue> codec1, FieldCodec<TParam2, TValue> codec2, FieldCodec<TParam3, TValue> codec3, FieldCodec<TParam4, TValue> codec4, FieldCodec<TParam5, TValue> codec5, FieldCodec<TParam6, TValue> codec6, FieldCodec<TParam7, TValue> codec7, FieldCodec<TParam8, TValue> codec8, FieldCodec<TParam9, TValue> codec9, FieldCodec<TParam10, TValue> codec10, FieldCodec<TParam11, TValue> codec11, FieldCodec<TParam12, TValue> codec12, FieldCodec<TParam13, TValue> codec13, FieldCodec<TParam14, TValue> codec14, FieldCodec<TParam15, TValue> codec15, FieldCodec<TParam16, TValue> codec16, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TParam16, TValue> constructor) 
        => new([codec1, codec2, codec3, codec4, codec5, codec6, codec7, codec8, codec9, codec10, codec11, codec12, codec13, codec14, codec15, codec16], constructor);


    public override TValue ReadGeneric(DataReader reader) {
        using var obj = reader.Object();

        return ReadInline(obj);
    }

    /// <summary>
    /// Reads a value inline from an ObjectDataReader
    /// </summary>
    /// <param name="obj">The object to read from</param>
    /// <returns>The parsed value</returns>
    public TValue ReadInline(ObjectDataReader obj) {
        object?[] values = new object[Fields.Length];

        for (int i = 0; i < Fields.Length; i++)
            values[i] = Fields[i].Read(obj);
        
        return (TValue) Constructor.DynamicInvoke(values)!;
    }

    public override void WriteGeneric(DataWriter writer, TValue value) {
        using var obj = writer.Object(Fields.Length);
        WriteInline(obj, value);
    }

    /// <summary>
    /// Writes a vale inline to an ObjectDataReader
    /// </summary>
    /// <param name="obj">The object to write to</param>
    /// <param name="value">The value to write</param>
    public void WriteInline(ObjectDataWriter obj, TValue value) {
        for (int i = 0; i < Fields.Length; i++) {
            var field = Fields[i];
            field.Write(obj, field.GetFromParent(value));
        }
    }
}
