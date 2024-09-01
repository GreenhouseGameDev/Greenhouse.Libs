using System.Formats.Cbor;
using Greenhouse.Libs.Serialization.Structure;

namespace Greenhouse.Libs.Serialization.Reader;

public record CborDataReader(CborReader reader) : StructuredObjectDataReader(new CborStructuredReader(reader).ReadValue());
