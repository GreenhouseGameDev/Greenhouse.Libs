using Greenhouse.Libs.Serialization.Io.Structure;

namespace Greenhouse.Libs.Serialization.Io.Reader;

public record StructuredObjectDataReader(StructuredValue Value) : DataReader {
    public ArrayDataReader Array()
        => new ArrayReader(this, (StructuredValue.Array) Value);

    public PrimitiveDataReader Primitive()
        => new PrimitiveReader((StructuredValue.Primitive) Value);

    public ObjectDataReader Object()
        => new ObjectReader(this, (StructuredValue.Object) Value);

    public class ObjectReader(StructuredObjectDataReader reader, StructuredValue.Object obj) : ObjectDataReader {
        private readonly StructuredObjectDataReader Reader = reader;
        private readonly StructuredValue.Object Obj = obj;

        public override DataReader End()
            => Reader;

        public override DataReader Field(string name)
            => new StructuredObjectDataReader(Obj.Values[name]);
    }

    public class ArrayReader(StructuredObjectDataReader reader, StructuredValue.Array arr) : ArrayDataReader {
        private readonly StructuredObjectDataReader Reader = reader;
        private readonly StructuredValue.Array Arr = arr;
        private int index = 0;

        public override DataReader End()
            => Reader;

        public override int Length()
            => Arr.Values.Length;

        public override DataReader Value()
            => new StructuredObjectDataReader(Arr.Values[index++]);
    }

    public class PrimitiveReader(StructuredValue.Primitive prim) : PrimitiveDataReader {
        private readonly StructuredValue.Primitive Prim = prim;

        public bool Bool() 
            => ((StructuredValue.Primitive.Bool) Prim).Value;

        public byte Byte() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (byte)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (byte)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (byte)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (byte)d.Value;
            throw new InvalidCastException();
        }

        public char Char() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (char)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (char)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (char)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (char)d.Value;
            throw new InvalidCastException();
        }

        public double Double() {
            if (Prim is StructuredValue.Primitive.Long l)
                return l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return d.Value;
            throw new InvalidCastException();
        }

        public float Float() {
            if (Prim is StructuredValue.Primitive.Long l)
                return l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (float)d.Value;
            throw new InvalidCastException();
        }

        public int Int() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (int)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (int)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (int)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (int)d.Value;
            throw new InvalidCastException();
        }

        public long Long() {
            if (Prim is StructuredValue.Primitive.Long l)
                return l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (long)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (long)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (long)d.Value;
            throw new InvalidCastException();
        }

        public sbyte SByte() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (sbyte)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (sbyte)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (sbyte)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (sbyte)d.Value;
            throw new InvalidCastException();
        }

        public short Short() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (short)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (short)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (short)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (short)d.Value;
            throw new InvalidCastException();
        }

        public string String()
            => ((StructuredValue.Primitive.String) Prim).Value;

        public uint UInt() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (uint)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (uint)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (uint)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (uint)d.Value;
            throw new InvalidCastException();
        }

        public ulong ULong() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (ulong)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (ulong)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (ulong)d.Value;
            throw new InvalidCastException();
        }

        public ushort UShort() {
            if (Prim is StructuredValue.Primitive.Long l)
                return (ushort)l.Value;
            if (Prim is StructuredValue.Primitive.ULong ul)
                return (ushort)ul.Value;
            if (Prim is StructuredValue.Primitive.Float f)
                return (ushort)f.Value;
            if (Prim is StructuredValue.Primitive.Double d)
                return (ushort)d.Value;
            throw new InvalidCastException();
        }
    }
}
