using System;

namespace GGM.Serializer
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] bytes) where T : new();
        byte[] Serialize<T>(T data) where T : new();
    }
}