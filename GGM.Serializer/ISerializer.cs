using System;

namespace GGM.Serializer
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] bytes);
        byte[] Serialize<T>(T data);
    }
}