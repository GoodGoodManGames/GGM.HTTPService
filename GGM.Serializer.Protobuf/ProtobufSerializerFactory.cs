using GGM.Context;
using GGM.Context.Attribute;

namespace GGM.Serializer.Protobuf
{
    [Configuration]
    public class ProtobufSerializerFactory 
    {
        [Managed(ManagedType.Singleton)]
        public ProtobufSerializer CreateProtobufSerializer()
        {
            return new ProtobufSerializer();
        }
    }
}