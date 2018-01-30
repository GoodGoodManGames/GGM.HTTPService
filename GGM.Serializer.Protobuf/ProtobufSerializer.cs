﻿using System;
using Google.Protobuf;

namespace GGM.Serializer.Protobuf
{
    public class ProtobufSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] bytes) where T : new()
        {
            var result = new T();
            if (!(result is IMessage))
                throw new ArgumentException("Serializer가 지원하지 않는 클래스입니다.");
            (result as IMessage).MergeFrom(bytes);
            return result;
        }

        public byte[] Serialize<T>(T data) where T : new()
        {
            if(!(data is IMessage serializeableData))
                throw new ArgumentException("Serializer가 지원하지 않는 클래스입니다.");
            return serializeableData.ToByteArray();
        }
    }
}