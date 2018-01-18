using GGM.Context;
using GGM.Context.Attribute;

namespace GGM.Serializer.Protobuf
{
    [Managed(ManagedType.Singleton)]
    public class ProtobufSerializerFactory : ISerializerFactory
    {
        public ISerializer Create()
        {
            return new ProtobufSerializer();
        }
    }
}