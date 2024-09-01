namespace Greenhouse.Libs.Serialization.Io;

public interface DataReader {
    public ObjectDataReader Object();
    public ArrayDataReader Array();
    public PrimitiveDataReader Primitive();
}

public abstract class ObjectDataReader : IDisposable {
    public abstract DataReader Field(string name);
    public abstract DataReader End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public abstract class ArrayDataReader : IDisposable {
    public abstract int Length();

    public abstract DataReader Value();
    public abstract DataReader End();

    public void Dispose() {
        GC.SuppressFinalize(this);
        End();
    }
}

public interface PrimitiveDataReader {
    public bool Bool();
    public byte Byte();
    public sbyte SByte();
    public short Short();
    public ushort UShort();
    public int Int();
    public uint UInt();
    public long Long();
    public ulong ULong();
    public float Float();
    public double Double();
    public char Char();
    public string String();
}
