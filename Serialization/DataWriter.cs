namespace Greenhouse.Libs.Serialization;

public interface DataWriter {
    public ObjectDataWriter Object(int keys);
    public MapDataWriter Map(int keys);
    public ArrayDataWriter Array(int length);
    public ArrayDataWriter FixedArray(int length);
    public PrimitiveDataWriter Primitive();
}

public abstract class ObjectDataWriter : IDisposable {
    public abstract DataWriter Field(string name);
    public abstract NullableFieldDataWriter NullableField(string name);
    public abstract void End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public abstract class MapDataWriter : IDisposable {
    public abstract DataWriter Field(string name);
    public abstract void End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public abstract class NullableFieldDataWriter : IDisposable {
    public abstract DataWriter NotNull();
    public abstract void Null();
    public abstract void End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public abstract class ArrayDataWriter : IDisposable {
    public abstract DataWriter Value();
    public abstract void End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public interface PrimitiveDataWriter {
    public void Bool(bool value);
    public void Byte(byte value);
    public void SByte(sbyte value);
    public void Short(short value);
    public void UShort(ushort value);
    public void Int(int value);
    public void UInt(uint value);
    public void Long(long value);
    public void ULong(ulong value);
    public void Float(float value);
    public void Double(double value);
    public void Char(char value);
    public void String(string value);
}
