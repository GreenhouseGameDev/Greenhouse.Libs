using Newtonsoft.Json;
using Greenhouse.Libs.Serialization.Structure;

namespace Greenhouse.Libs.Serialization.Reader;

public record JsonDataReader(JsonReader reader) : StructuredObjectDataReader(new JsonStructuredReader(reader).ReadValue());
